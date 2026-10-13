using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

// Only handles HTTP: reads the request, calls the service,
// picks the status code. No database code, no rules.
[ApiController]
[Route("api/bikes")]
public class BikesController : ControllerBase
{
    private readonly IBikeService _bikeService;

    public BikesController(IBikeService bikeService)
    {
        _bikeService = bikeService;
    }

    // GET /api/bikes?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<BikeDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await _bikeService.GetAllAsync(page, pageSize));
    }

    // GET /api/bikes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BikeDto>> GetById(int id)
    {
        return Ok(await _bikeService.GetByIdAsync(id));
    }

    // POST /api/bikes -> 201 + Location
    [HttpPost]
    public async Task<ActionResult<BikeDto>> Create(CreateBikeDto dto)
    {
        var created = await _bikeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/bikes/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BikeDto>> Update(int id, UpdateBikeDto dto)
    {
        return Ok(await _bikeService.UpdateAsync(id, dto));
    }

    // DELETE /api/bikes/5 -> 204
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _bikeService.DeleteAsync(id);
        return NoContent();
    }
}