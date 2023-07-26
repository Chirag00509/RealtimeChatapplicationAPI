using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserProfile> GetUsersExcludingId(int id);

        Registration RegisterUser(User user);

        LoginResponse LoginUser(Login login);
    }

    public enum Registration
    {
        Success,
        EmailAlreadyExists,
    }
}
