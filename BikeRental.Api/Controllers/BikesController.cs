using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Controllers;

[ApiController]
[Route("api/bikes")]
public class BikesController : ControllerBase
{
    private readonly AppDbContext _context;

    // ASP.NET gives us the AppDbContext (dependency injection),
    // because it's registered in Program.cs.
    public BikesController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/bikes
    [HttpGet]
    public async Task<ActionResult<List<BikeDto>>> GetAll()
    {
        var bikes = await _context.Bikes
            .Where(b => b.DeletedUtc == null)   // hide soft-deleted bikes
            .OrderBy(b => b.Id)
            .ToListAsync();

        var result = bikes.Select(ToDto).ToList();

        return Ok(result);                       // 200 + JSON list
    }

    // GET /api/bikes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BikeDto>> GetById(int id)
    {
        var bike = await _context.Bikes
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedUtc == null);

        if (bike == null)
        {
            return NotFound();                   // 404
        }

        return Ok(ToDto(bike));                  // 200 + JSON object
    }

    // POST /api/bikes
    [HttpPost]
    public async Task<ActionResult<BikeDto>> Create(CreateBikeDto dto)
    {
        var bike = new Bike
        {
            Model = dto.Model,
            Type = dto.Type!.Value,
            PricePerHour = dto.PricePerHour,
            InMaintenance = false
        };

        _context.Bikes.Add(bike);                // mark as new
        await _context.SaveChangesAsync();       // write to database, bike.Id is now set

        // 201 Created + Location header pointing to GET /api/bikes/{id}
        return CreatedAtAction(nameof(GetById), new { id = bike.Id }, ToDto(bike));
    }

    // Converts a database Bike into the BikeDto the API returns.
    // Status is computed here, never stored.
    private static BikeDto ToDto(Bike bike)
    {
        return new BikeDto
        {
            Id = bike.Id,
            Model = bike.Model,
            Type = bike.Type,
            PricePerHour = bike.PricePerHour,
            Status = bike.InMaintenance ? "InMaintenance" : "Available"
        };
    }
}