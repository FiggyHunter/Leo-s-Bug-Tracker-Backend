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
    [Route("auth/update")]
    [ApiController]
    public class UpdatePreferencesController : ControllerBase
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        public UpdatePreferencesController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Update([FromBody] UserUpdate userUpdateDetails)
        {
        Console.WriteLine(userUpdateDetails);
        _userService.UpdateRoleAvatar(userUpdateDetails);
        return Ok("ok");
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
