using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/stations")]
public class StationsController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationsController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StationDto>>> GetAll()
    {
        return Ok(await _stationService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StationDto>> GetById(int id)
    {
        return Ok(await _stationService.GetByIdAsync(id));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<StationDto>> Create(SaveStationDto dto)
    {
        var created = await _stationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<StationDto>> Update(int id, SaveStationDto dto)
    {
        return Ok(await _stationService.UpdateAsync(id, dto));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _stationService.DeleteAsync(id);
        return NoContent();
    }
}