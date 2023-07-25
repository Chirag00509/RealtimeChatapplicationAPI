using WebApplication1.Interfaces;
using WebApplication1.Modal;
using WebApplication1.Repository;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public IEnumerable<UserProfile> GetUsersExcludingId(int id)
        {
            return _userRepository.GetUsersExcludingId(id);
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
