using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<UserProfile> GetUsersExcludingId(int id);
        bool DoesEmailExist(string email);

        User getEmailAndPassword(string email, string password);
        void AddUser(User user);
    }
}
