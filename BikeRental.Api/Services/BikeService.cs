using BikeRental.Api.Common;
using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Services;

public class BikeService : IBikeService
{
    private readonly AppDbContext _context;

    public BikeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BikeDto>> GetAllAsync(BikeQuery filter)
    {
        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 100);

        IQueryable<Bike> query = _context.Bikes.Where(b => b.DeletedUtc == null);

        if (filter.Type != null)
        {
            var type = filter.Type.Value;
            query = query.Where(b => b.Type == type);
        }

        if (filter.StationId != null)
        {
            var stationId = filter.StationId.Value;
            query = query.Where(b => b.StationId == stationId);
        }

        // Status is computed, so the filter uses the same rules as ToDto,
        // translated to SQL by EF.
        query = filter.Status switch
        {
            BikeStatus.InMaintenance => query.Where(b => b.InMaintenance),
            BikeStatus.Rented => query.Where(b => !b.InMaintenance && b.Rentals.Any(r => r.EndUtc == null)),
            BikeStatus.Available => query.Where(b => !b.InMaintenance && !b.Rentals.Any(r => r.EndUtc == null)),
            _ => query
        };

        var totalCount = await query.CountAsync();

        var bikes = await query
            .OrderBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Rented bikes on this page, fetched with a single query.
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
        await EnsureStationExistsAsync(dto.StationId);

        var bike = new Bike
        {
            Model = dto.Model,
            Type = dto.Type!.Value,
            PricePerHour = dto.PricePerHour,
            InMaintenance = false,
            StationId = dto.StationId
        };

        _context.Bikes.Add(bike);
        await _context.SaveChangesAsync();

        return ToDto(bike, isRented: false);
    }

    public async Task<BikeDto> UpdateAsync(int id, UpdateBikeDto dto)
    {
        var bike = await FindBikeAsync(id);
        var isRented = await IsRentedAsync(bike.Id);

        // S7: a rented bike cannot be set to maintenance.
        if (dto.InMaintenance && isRented)
        {
            throw new ConflictApiException("BikeRented", "A rented bike cannot be set to maintenance.");
        }

        await EnsureStationExistsAsync(dto.StationId);

        bike.Model = dto.Model;
        bike.Type = dto.Type!.Value;
        bike.PricePerHour = dto.PricePerHour;
        bike.InMaintenance = dto.InMaintenance;
        bike.StationId = dto.StationId;

        await _context.SaveChangesAsync();

        return ToDto(bike, isRented);
    }

    public async Task DeleteAsync(int id)
    {
        var bike = await FindBikeAsync(id);

        // S8: a rented bike cannot be deleted.
        if (await IsRentedAsync(bike.Id))
        {
            throw new ConflictApiException("BikeRented", "A rented bike cannot be deleted.");
        }

        // S9: soft delete keeps the rental history.
        bike.DeletedUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // S1: returns a non-deleted bike or throws 404.
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

    // A bike is rented if it has a rental without an end time.
    private Task<bool> IsRentedAsync(int bikeId)
    {
        return _context.Rentals.AnyAsync(r => r.BikeId == bikeId && r.EndUtc == null);
    }

    // A station id is optional, but if given it must exist.
    private async Task EnsureStationExistsAsync(int? stationId)
    {
        if (stationId != null && !await _context.Stations.AnyAsync(s => s.Id == stationId))
        {
            throw new NotFoundApiException("StationNotFound", $"Station with id {stationId} was not found.");
        }
    }

    // Status priority: InMaintenance, then Rented, otherwise Available.
    private static BikeDto ToDto(Bike bike, bool isRented)
    {
        BikeStatus status;

        if (bike.InMaintenance)
            status = BikeStatus.InMaintenance;
        else if (isRented)
            status = BikeStatus.Rented;
        else
            status = BikeStatus.Available;

        return new BikeDto
        {
            Id = bike.Id,
            Model = bike.Model,
            Type = bike.Type,
            PricePerHour = bike.PricePerHour,
            Status = status,
            StationId = bike.StationId
        };
    }
}