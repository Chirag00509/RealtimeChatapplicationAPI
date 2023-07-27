using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var currentUser = HttpContext.User;

            if (!ModelState.IsValid) 
            {
                return BadRequest(new { message = "Invalid request parameters" });
            }

            var id = Convert.ToInt32(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var users = _userService.GetUsersExcludingId(id).ToList();

            return Ok(users);
        }

        [HttpPost("/api/register")]
        public async Task<ActionResult<User>> PostUser(User user)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(new { message = "Registration failed due to validation errors." });
            }

            return await _userService.RegisterUser(user);
            
        }

        [HttpPost("/api/login")]

        public async Task<ActionResult<Login>> login(Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Login failed due to validation errors." });
            }

            return await _userService.LoginUser(login);
        }

        [HttpPost("/api/SocialLogin")]
        public async Task<ActionResult> SocialLogin(TokenRequest token)
        {
            var user = await _userService.VerifyGoogleTokenAsync(token.TokenId);

            return Ok(user);
        }

    }
}

