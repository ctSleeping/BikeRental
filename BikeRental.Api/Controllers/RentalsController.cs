using BikeRental.Api.Dtos;
using BikeRental.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/rentals")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    // GET /api/rentals
    [HttpGet]
    public async Task<ActionResult<List<RentalDto>>> GetAll()
    {
        return Ok(await _rentalService.GetAllAsync());
    }

    // GET /api/rentals/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentalDto>> GetById(int id)
    {
        return Ok(await _rentalService.GetByIdAsync(id));
    }

    // POST /api/rentals -> 201 + Location
    [HttpPost]
    public async Task<ActionResult<RentalDto>> Start(CreateRentalDto dto)
    {
        var rental = await _rentalService.StartAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
    }

    // POST /api/rentals/5/return
    [HttpPost("{id:int}/return")]
    public async Task<ActionResult<RentalDto>> Return(int id)
    {
        return Ok(await _rentalService.ReturnAsync(id));
    }
}