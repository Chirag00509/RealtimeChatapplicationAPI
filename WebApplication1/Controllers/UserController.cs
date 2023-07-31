using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Interfaces;
using Microsoft.AspNetCore.Identity;

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

        [HttpPost("/api/register")]
        public async Task<ActionResult<IdentityResult>> PostUser(User user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Registration failed due to validation errors." });
            }

            return await _userService.RegisterUser(user);


        }

        // GET: api/User
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUser()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request parameters" });
            }
            Console.WriteLine("Hello");
           
            var users = _userService.GetUsersExcludingId().ToList();

            Console.WriteLine(users);

            if(users == null)
            {
                return NotFound(new { messsage = "Message not found" });
            }

            return Ok(users);
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

        [HttpGet("/api/User/{id}")]

        public async Task<ActionResult> GetUserNameById(string id)
        {
            var user = await _userService.GetUserName(id);

            return Ok(user);
        }
    }
}

