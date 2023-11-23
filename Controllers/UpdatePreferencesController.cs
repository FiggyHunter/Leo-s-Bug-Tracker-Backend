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
        var user = _userService.UpdateRoleAvatar(userUpdateDetails);
        var token = _userService.GenerateToken(user);
        return Ok(token);
        }
    }
}
