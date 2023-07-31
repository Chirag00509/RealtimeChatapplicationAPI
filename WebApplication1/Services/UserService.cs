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
using Microsoft.AspNetCore.Identity;
using Azure;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<string>> GetUserName(string id)
        {
            return await _userRepository.GetUserNameId(id);
        }

        public IEnumerable<UserProfile> GetUsersExcludingId()
        {
            var id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(id);
            return _userRepository.GetUsersExcludingId(id);
        }

        public async Task<ActionResult> LoginUser(Login login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            //if (checkGoogleUser.Type == "google")
            //{
            //    return new BadRequestObjectResult(new { message = "Please login with google" });
            //}
            var signInResult = await _userManager.CheckPasswordAsync(user, login.Password);

            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "Login failed due to incorrect credentials" });
            }
            if(!signInResult)
            {
                return new NotFoundObjectResult(new { message = "Login failed due to incorrect credentials" });
            }

            var token = getToken(user.Id, user.UserName, user.Email);

            var loginResponse = new LoginResponse
            {
                Token = token,
                Profile = new UserProfile
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email
                }
            };

            var id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"{id}"); 

            return new OkObjectResult(loginResponse);
        }

        public async Task<ActionResult> RegisterUser(User user)
        {
            var userExists = await _userManager.FindByNameAsync(user.Name);

            if (userExists != null)
            {
                return new ConflictObjectResult(new { message = "Registration failed because the email is already registered." });
            }

            var users = new IdentityUser
            {
                Email = user.Email,
                UserName = user.Name
            };

            var result = await _userManager.CreateAsync(users, user.Password);

            if (!result.Succeeded) 
            {
            }

            var Profile = new UserProfile
            {  
                Id = users.Id,
                Name = users.UserName,
                Email = users.Email,
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
                    var user = await _userManager.FindByLoginAsync("Google", payload.Subject);

                    if (user == null)
                    {
                        user = await _userManager.FindByEmailAsync(payload.Email);

                        if (user == null)
                        {
                            user = new IdentityUser
                            {
                                UserName = payload.FamilyName,
                                Email = payload.Email, 
                            };

                            Console.WriteLine(user);

                            var userLoginInfo = new UserLoginInfo("Goggle", payload.Subject, "Goggle");
                            var result = await _userManager.CreateAsync(user);

                            if (result.Succeeded)
                            {
                                await _userManager.AddLoginAsync(user, userLoginInfo);
                            }
                            else
                            {

                            }
                        }
                    }

                    var users = await _userManager.FindByEmailAsync(payload.Email);

                    if(users == null)
                    {
                        return null;
                    }

                    var generatedToken = getToken(users.Id, users.UserName, users.Email);

                    var loginResponse = new LoginResponse
                    {
                        Token = generatedToken,
                        Profile = new UserProfile
                        {
                            Id = users.Id,
                            Name = users.UserName,
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

        private string getToken(string id, string name, string email)
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
    }
}
