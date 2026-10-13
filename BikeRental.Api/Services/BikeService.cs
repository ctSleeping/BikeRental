using BikeRental.Api.Common;
using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Services;

// All bike logic and database work lives here.
public class BikeService : IBikeService
{
    private readonly AppDbContext _context;

    public BikeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BikeDto>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Bikes
            .Where(b => b.DeletedUtc == null)
            .OrderBy(b => b.Id);

        var totalCount = await query.CountAsync();

        var bikes = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<BikeDto>
        {
            Items = bikes.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<BikeDto> GetByIdAsync(int id)
    {
        var bike = await FindBikeAsync(id);
        return ToDto(bike);
    }

    public async Task<BikeDto> CreateAsync(CreateBikeDto dto)
    {
        var bike = new Bike
        {
            Model = dto.Model,
            Type = dto.Type!.Value,
            PricePerHour = dto.PricePerHour,
            InMaintenance = false
        };

        _context.Bikes.Add(bike);
        await _context.SaveChangesAsync();

        return ToDto(bike);
    }

    public async Task<BikeDto> UpdateAsync(int id, UpdateBikeDto dto)
    {
        var bike = await FindBikeAsync(id);

        bike.Model = dto.Model;
        bike.Type = dto.Type!.Value;
        bike.PricePerHour = dto.PricePerHour;
        bike.InMaintenance = dto.InMaintenance;

        await _context.SaveChangesAsync();

        return ToDto(bike);
    }

    public async Task DeleteAsync(int id)
    {
        var bike = await FindBikeAsync(id);

        bike.DeletedUtc = DateTime.UtcNow;   // soft delete (S9)
        await _context.SaveChangesAsync();
    }

    // Finds a non-deleted bike or throws 404 BikeNotFound (S1).
    // Used by GetById, Update and Delete, so the rule is written once.
    private async Task<Bike> FindBikeAsync(int id)
    {
        var bike = await _context.Bikes
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedUtc == null);

        if (bike == null)
        {
            throw new NotFoundApiException("BikeNotFound", $"Bike with id {id} was not found.");
        }

        return bike;
    }

    // Status is computed, never stored.
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