using BikeRental.Api.Common;
using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Services;

public class RentalService : IRentalService
{
    private readonly AppDbContext _context;

    public RentalService(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/rentals – level 3: everyone sees all rentals (no login yet)
    public async Task<List<RentalDto>> GetAllAsync()
    {
        var rentals = await _context.Rentals
            .Include(r => r.Bike)                 // also load the bike, for bike.model
            .OrderByDescending(r => r.StartUtc)   // newest first
            .ToListAsync();

        return rentals.Select(ToDto).ToList();
    }

    // GET /api/rentals/5
    public async Task<RentalDto> GetByIdAsync(int id)
    {
        var rental = await FindRentalAsync(id);
        return ToDto(rental);
    }

    // POST /api/rentals – start a rental
    public async Task<RentalDto> StartAsync(CreateRentalDto dto)
    {
        // S1: the bike must exist and not be soft-deleted
        var bike = await _context.Bikes
            .FirstOrDefaultAsync(b => b.Id == dto.BikeId && b.DeletedUtc == null);

        if (bike == null)
        {
            throw new NotFoundApiException("BikeNotFound", $"Bike with id {dto.BikeId} was not found.");
        }

        // S2: the bike must not be in maintenance...
        if (bike.InMaintenance)
        {
            throw new ConflictApiException("BikeNotAvailable", "Bike is in maintenance.");
        }

        // S2: ...and must not already be rented (= has a rental with no EndUtc)
        var bikeIsRented = await _context.Rentals
            .AnyAsync(r => r.BikeId == bike.Id && r.EndUtc == null);

        if (bikeIsRented)
        {
            throw new ConflictApiException("BikeNotAvailable", "Bike is already rented.");
        }

        // The user must exist (level 3: user comes from the request)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null)
        {
            throw new NotFoundApiException("UserNotFound", $"User with id {dto.UserId} was not found.");
        }

        // S3: a user can have only one active rental at a time
        var userHasActiveRental = await _context.Rentals
            .AnyAsync(r => r.UserId == user.Id && r.EndUtc == null);

        if (userHasActiveRental)
        {
            throw new ConflictApiException("UserHasActiveRental", "User already has an active rental.");
        }

        // All rules passed -> create the rental
        var rental = new Rental
        {
            Bike = bike,
            User = user,
            StartUtc = DateTime.UtcNow,
            EndUtc = null,                    // null = active
            HourlyPrice = bike.PricePerHour,  // price is fixed at the start
            TotalPrice = null                 // calculated on return
        };

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        return ToDto(rental);
    }

    // POST /api/rentals/5/return – end a rental
    public async Task<RentalDto> ReturnAsync(int id)
    {
        var rental = await FindRentalAsync(id);

        // S4: only an active rental can be returned
        if (rental.EndUtc != null)
        {
            throw new ConflictApiException("RentalNotActive", "Rental is not active.");
        }

        rental.EndUtc = DateTime.UtcNow;
        rental.TotalPrice = CalculatePrice(rental.StartUtc, rental.EndUtc.Value, rental.HourlyPrice);

        // All changes saved with ONE SaveChangesAsync call (course rule)
        await _context.SaveChangesAsync();

        return ToDto(rental);
    }

    // hours = max(1, ceil(duration in hours)); total = hours × hourly price
    // Examples at 3.50 €/h:  10 min -> 1 h -> 3.50   |   2 h 20 min -> 3 h -> 10.50
    public static decimal CalculatePrice(DateTime startUtc, DateTime endUtc, decimal hourlyPrice)
    {
        var durationHours = (endUtc - startUtc).TotalHours;
        var billableHours = Math.Max(1, (int)Math.Ceiling(durationHours));
        return billableHours * hourlyPrice;
    }

    // Finds a rental (with its bike) or throws 404 RentalNotFound
    private async Task<Rental> FindRentalAsync(int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.Bike)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental == null)
        {
            throw new NotFoundApiException("RentalNotFound", $"Rental with id {id} was not found.");
        }

        return rental;
    }

    private static RentalDto ToDto(Rental rental)
    {
        return new RentalDto
        {
            Id = rental.Id,
            Bike = new BikeSummaryDto
            {
                Id = rental.BikeId,
                Model = rental.Bike?.Model ?? string.Empty
            },
            StartUtc = rental.StartUtc,
            EndUtc = rental.EndUtc,
            HourlyPrice = rental.HourlyPrice,
            TotalPrice = rental.TotalPrice,
            Status = rental.EndUtc == null ? "Active" : "Completed"
        };
    }
}