using BugTrackerAPI.Entities;
using BugTrackerAPI.Models;
using BugTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackerAPI.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IUserService _userService;
        public LoginController(IUserService userService)
        {   
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            try
            {
                User user = _userService.LoginUser(userLogin);
                var token = _userService.GenerateToken(user);
                return Ok(token);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Incorrect password.")
                  return BadRequest("Incorrect Credentials");
                if (ex.Message == "User not found.")
                    return BadRequest("Incorrect Credentials");
            }
            return StatusCode(500, "Internal Server Error");
        }
    }
}
