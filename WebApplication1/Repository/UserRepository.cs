using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    }
}
