using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Modal;

namespace WebApplication1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public UserRepository(ChatContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //        public void AddUser(User user)
        //        {
        //            _context.User.Add(user);
        //            _context.SaveChanges();
        //        }

        //public bool DoesEmailExist(string email)
        //{
        //    return _context.User.Any(u => u.Email == email);
        //}

        //        public User getEmailAndPassword(string email, string password)
        //        {
        //            return _context.User.FirstOrDefault(u => u.Email == email && u.Password == password);
        //        }

        //        public async Task<User> GetUserByEmail(string email)
        //        {
        //            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        //        }

        public IEnumerable<UserProfile> GetUsersExcludingId(string id)
        {
            return _userManager.Users
                .Where(u => u.Id != id)
                .Select(u => new UserProfile
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email
                })
                .ToList();
        }

        public async Task<List<string>> GetUserNameId(string id)
        {
            return await _userManager.Users.Where(u => u.Id == id).Select(u => u.UserName).ToListAsync();
        }
    }
}
