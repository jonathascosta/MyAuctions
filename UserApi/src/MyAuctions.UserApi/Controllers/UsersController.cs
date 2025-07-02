using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserApi.Dtos;
using UserApi.Services;

namespace UserApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest dto)
    {
        try
        {
            await _userService.RegisterAsync(dto);
            return CreatedAtAction(null, null);
        }
        catch (ApplicationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest dto)
    {
        try
        {
            var token = await _userService.LoginAsync(dto);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            return Ok(new { token, login = dto.Username, role });
        }
        catch (ApplicationException)
        {
            return Unauthorized();
        }
    }
}