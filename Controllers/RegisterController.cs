
using BugTrackerAPI.Models;
using BugTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BugTrackerAPI.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private IUserService _userService;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] UserRegister userRegister)
        {

            try
            {
                var user = _userService.RegisterUser(userRegister);
                if (user != null)
                {
                    var token = _userService.GenerateToken(user);
                    return Ok(token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.Message == "Incorrect password.")
                    return BadRequest("Incorrect Credentials");
                if (ex.Message == "User not found.")
                    return BadRequest("Incorrect Credentials");
                if (ex.Message == "User already exists!")
                    return BadRequest("User already exists!");
            }
            return StatusCode(500, "Internal Server Error");
        }

        
    }
}
