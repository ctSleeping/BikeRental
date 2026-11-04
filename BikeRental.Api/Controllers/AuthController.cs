using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        return Created("/api/users/me", user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        return Ok(await _authService.LoginAsync(dto));
    }
}