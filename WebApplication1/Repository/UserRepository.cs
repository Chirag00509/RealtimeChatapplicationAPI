using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Modal;

namespace WebApplication1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatContext _context;

        public UserRepository(ChatContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }

        public bool DoesEmailExist(string email)
        {
            return _context.User.Any(u => u.Email == email);
        }

        public User getEmailAndPassword(string email, string password)
        {
            return _context.User.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public IEnumerable<UserProfile> GetUsersExcludingId(int id)
        {
            return _context.User
                .Where(u => u.Id != id)
                .Select(u => new UserProfile
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToList();
        }
    }
}
