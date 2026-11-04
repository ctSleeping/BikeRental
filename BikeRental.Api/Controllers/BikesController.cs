using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/bikes")]
public class BikesController : ControllerBase
{
    private readonly IBikeService _bikeService;

    public BikesController(IBikeService bikeService)
    {
        _bikeService = bikeService;
    }

    [HttpGet]
    // GET /api/bikes?type=City&status=Available&stationId=1&page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<BikeDto>>> GetAll([FromQuery] BikeQuery query)
    {
        return Ok(await _bikeService.GetAllAsync(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BikeDto>> GetById(int id)
    {
        return Ok(await _bikeService.GetByIdAsync(id));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<BikeDto>> Create(CreateBikeDto dto)
    {
        var created = await _bikeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BikeDto>> Update(int id, UpdateBikeDto dto)
    {
        return Ok(await _bikeService.UpdateAsync(id, dto));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _bikeService.DeleteAsync(id);
        return NoContent();
    }
}