using BikeRental.Api.Common;
using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        return Ok(await _userService.GetByIdAsync(User.GetUserId()));
    }
}