using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserProfile> GetUsersExcludingId(int id);

        Registration RegisterUser(User user);

        Task<ActionResult> LoginUser(Login login);

        Task<LoginResponse> VerifyGoogleTokenAsync(string tokenId);

    }

    public enum Registration
    {
        Success,
        EmailAlreadyExists,
    }
}
