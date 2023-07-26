using WebApplication1.Interfaces;
using WebApplication1.Modal;
using WebApplication1.Repository;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


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

        public LoginResponse LoginUser(Login login)
        {
            var convertHashPassword = hashPassword(login.Password);

            var user = _userRepository.getEmailAndPassword(login.Email, convertHashPassword);   

            if(user == null) 
            {
                return null;
            }

            var token = getToken(user.Id, user.Email, convertHashPassword);

            return new LoginResponse
            {
                Token = token,
                Profile = new UserProfile
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            };
        }

        public Registration RegisterUser(User user)
        {
            var emailExists = _userRepository.DoesEmailExist(user.Email);

            if (emailExists)
            {
                return Registration.EmailAlreadyExists;
            }

            user.Password = hashPassword(user.Password);
            _userRepository.AddUser(user);
            return Registration.Success;
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
