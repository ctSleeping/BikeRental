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

        // ONE extra query for the whole page: which of these bikes are rented?
        // (Asking once per bike would mean 10 queries for a page of 10.)
        var bikeIds = bikes.Select(b => b.Id).ToList();

        var rentedBikeIds = await _context.Rentals
            .Where(r => r.EndUtc == null && bikeIds.Contains(r.BikeId))
            .Select(r => r.BikeId)
            .ToListAsync();

        return new PagedResult<BikeDto>
        {
            Items = bikes.Select(b => ToDto(b, rentedBikeIds.Contains(b.Id))).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<BikeDto> GetByIdAsync(int id)
    {
        var bike = await FindBikeAsync(id);
        var isRented = await IsRentedAsync(bike.Id);
        return ToDto(bike, isRented);
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

        return ToDto(bike, isRented: false);   // a brand-new bike can't be rented
    }

    public async Task<BikeDto> UpdateAsync(int id, UpdateBikeDto dto)
    {
        var bike = await FindBikeAsync(id);
        var isRented = await IsRentedAsync(bike.Id);

        // S7: a rented bike can't be put into maintenance
        if (dto.InMaintenance && isRented)
        {
            throw new ConflictApiException("BikeRented", "A rented bike cannot be set to maintenance.");
        }

        bike.Model = dto.Model;
        bike.Type = dto.Type!.Value;
        bike.PricePerHour = dto.PricePerHour;   // affects only NEW rentals (HourlyPrice is copied)
        bike.InMaintenance = dto.InMaintenance; // S6: the only status-related field the client can change

        await _context.SaveChangesAsync();

        return ToDto(bike, isRented);
    }

    public async Task DeleteAsync(int id)
    {
        var bike = await FindBikeAsync(id);

        // S8: a rented bike can't be deleted
        if (await IsRentedAsync(bike.Id))
        {
            throw new ConflictApiException("BikeRented", "A rented bike cannot be deleted.");
        }

        bike.DeletedUtc = DateTime.UtcNow;   // S9: soft delete, rental history stays
        await _context.SaveChangesAsync();
    }

    // Finds a non-deleted bike or throws 404 BikeNotFound (S1).
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

    // A bike is rented if it has a rental with no end time.
    private Task<bool> IsRentedAsync(int bikeId)
    {
        return _context.Rentals.AnyAsync(r => r.BikeId == bikeId && r.EndUtc == null);
    }

    // Status is computed, never stored:
    // InMaintenance wins, then Rented, otherwise Available.
    private static BikeDto ToDto(Bike bike, bool isRented)
    {
        string status;

        if (bike.InMaintenance)
            status = "InMaintenance";
        else if (isRented)
            status = "Rented";
        else
            status = "Available";

        return new BikeDto
        {
            Id = bike.Id,
            Model = bike.Model,
            Type = bike.Type,
            PricePerHour = bike.PricePerHour,
            Status = status
        };
    }
}