using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Modal
{
    public class SaveData : IdentityUser
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
