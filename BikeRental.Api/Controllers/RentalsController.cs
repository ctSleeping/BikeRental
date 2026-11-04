using BikeRental.Api.Common;
using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/rentals")]
[Authorize]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RentalDto>>> GetAll()
    {
        return Ok(await _rentalService.GetAllAsync(User.GetUserId(), User.IsInRole("Admin")));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentalDto>> GetById(int id)
    {
        return Ok(await _rentalService.GetByIdAsync(id, User.GetUserId(), User.IsInRole("Admin")));
    }

    [HttpPost]
    public async Task<ActionResult<RentalDto>> Start(CreateRentalDto dto)
    {
        var rental = await _rentalService.StartAsync(dto, User.GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
    }

    // The body is optional: { "returnStationId": 3 } or nothing.
    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<RentalDto>> Return(
        int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ReturnRentalDto? dto)
    {
        return Ok(await _rentalService.ReturnAsync(id, User.GetUserId(), dto?.ReturnStationId));
    }
}