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
  [Route("auth/name")]
  [ApiController]
  public class UpdateNameController : ControllerBase
  {
    private IUserService _userService;
    private IConfiguration _configuration;
    public UpdateNameController(IConfiguration configuration, IUserService userService)
    {
      _configuration = configuration;
      _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult NameUpdate([FromBody] UserUpdate userUpdateDetails)
    {
      try
      {
        var user =_userService.UpdateName(userUpdateDetails);

        return Ok(_userService.GenerateToken( user));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return BadRequest("Request failed.");
      }
    }
  }
}
