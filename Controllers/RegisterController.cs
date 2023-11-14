using BugTrackerAPI.Entities;
using BugTrackerAPI.Models;
using BugTrackerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BugTrackerAPI.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        public RegisterController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] UserRegister userRegister)
        {
            try
            {
                _userService.RegisterUser(userRegister);
                //var token = Generate(user);
                Console.WriteLine("try");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.Message == "Incorrect password.")
                    return BadRequest("Incorrect Credentials");
                if (ex.Message == "User not found.")
                    return BadRequest("Incorrect Credentials");
            }
            return StatusCode(500, "Internal Server Error");
        }

        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role),
                new Claim("Avatar", user.Avatar),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
