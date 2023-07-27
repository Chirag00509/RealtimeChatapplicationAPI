using WebApplication1.Interfaces;
using WebApplication1.Modal;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public IEnumerable<UserProfile> GetUsersExcludingId(int id)
        {
            return _userRepository.GetUsersExcludingId(id);
        }

        public async Task<ActionResult> LoginUser(Login login)
        {
            var checkGoogleUser = await _userRepository.GetUserByEmail(login.Email);

            if (checkGoogleUser.Type == "google") 
            {
                return new BadRequestObjectResult(new { message = "Please login with google" });
            }

            var convertHashPassword = hashPassword(login.Password);

            var user = _userRepository.getEmailAndPassword(login.Email, convertHashPassword);   

            if(user == null) 
            {
                return new NotFoundObjectResult(new { message = "Login failed due to incorrect credentials" });
            }


            var token = getToken(user.Id, user.Name, user.Email);

            var loginResponse = new LoginResponse
            {
                Token = token,
                Profile = new UserProfile
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            };

            return new OkObjectResult(loginResponse);
        }

        public async Task<ActionResult> RegisterUser(User user)
        {
            var emailExists =  _userRepository.DoesEmailExist(user.Email);

            if (emailExists)
            {
                return new ConflictObjectResult(new { message = "Registration failed because the email is already registered." }) ;
            }

            user.Password = hashPassword(user.Password);
            user.Type = "normal";
            _userRepository.AddUser(user);
            var Profile = new UserProfile
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };

            return new OkObjectResult(Profile);
        }

        public async Task<LoginResponse> VerifyGoogleTokenAsync(string tokenId)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
            settings.Audience = new List<string> { "1016420993521-j4vm79o7vt2iocq60himn9hgpmp4gqbt.apps.googleusercontent.com" }; // Replace with your actual Google Client ID

            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(tokenId, settings);

                if (payload.EmailVerified)
                {
                    var emailCheck = _userRepository.DoesEmailExist(payload.Email);

                    if(!emailCheck)
                    {
                        var newUser = new User
                        {
                            Name = payload.GivenName,
                            Email = payload.Email,
                            Type = "google"
                        };

                        _userRepository.AddUser(newUser);
                    }

                    var users = await _userRepository.GetUserByEmail(payload.Email);

                    var generatedToken = getToken(users.Id, users.Name, users.Email);

                    var loginResponse = new LoginResponse
                    {
                        Token = generatedToken,
                        Profile = new UserProfile
                        {
                            Id = users.Id,
                            Name = users.Name,
                            Email = users.Email
                        }
                    };

                    return loginResponse;
                }

                return null;

            }
            catch (InvalidJwtException ex)
            {
                // The token is invalid. Handle the error.
                throw new Exception("Invalid token: " + ex.Message);
            }
        }

        private string getToken(int id, string name, string email)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);


            string Token = new JwtSecurityTokenHandler().WriteToken(token);

            return Token;
        }

        private string hashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }

        }
    }
}
