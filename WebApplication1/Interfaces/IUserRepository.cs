using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<UserProfile> GetUsersExcludingId(string id);
    }
}
