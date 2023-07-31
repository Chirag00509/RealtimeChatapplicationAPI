using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modal;

namespace WebApplication1.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserProfile> GetUsersExcludingId();

        Task<ActionResult> RegisterUser(User user);
        Task<ActionResult> LoginUser(Login login);

        Task<LoginResponse> VerifyGoogleTokenAsync(string tokenId);
        Task<List<string>> GetUserName(string id);


    }
}
