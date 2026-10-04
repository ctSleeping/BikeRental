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

    // GET /api/bikes?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<BikeDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Fix nonsense values instead of crashing.
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        // The base query: not-deleted bikes, in a fixed order.
        // Nothing is sent to the database yet.
        var query = _context.Bikes
            .Where(b => b.DeletedUtc == null)
            .OrderBy(b => b.Id);

        // Query 1: how many bikes there are in total.
        var totalCount = await query.CountAsync();

        // Query 2: only the bikes on the requested page.
        var bikes = await query
            .Skip((page - 1) * pageSize)   // skip earlier pages
            .Take(pageSize)                // take one page
            .ToListAsync();

        var result = new PagedResult<BikeDto>
        {
            Items = bikes.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);                 // 200 + paging wrapper
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

    // PUT /api/bikes/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BikeDto>> Update(int id, UpdateBikeDto dto)
    {
        var bike = await _context.Bikes
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedUtc == null);

        if (bike == null)
        {
            return NotFound();                   // 404
        }

        // Change the loaded object. EF notices the changes automatically.
        bike.Model = dto.Model;
        bike.Type = dto.Type!.Value;
        bike.PricePerHour = dto.PricePerHour;
        bike.InMaintenance = dto.InMaintenance;

        await _context.SaveChangesAsync();       // writes only the changed columns

        return Ok(ToDto(bike));                  // 200 + updated bike
    }

    // DELETE /api/bikes/5  (soft delete: the row stays in the database)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var bike = await _context.Bikes
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedUtc == null);

        if (bike == null)
        {
            return NotFound();                   // 404
        }

        bike.DeletedUtc = DateTime.UtcNow;       // mark as deleted instead of removing
        await _context.SaveChangesAsync();

        return NoContent();                      // 204, success with no body
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