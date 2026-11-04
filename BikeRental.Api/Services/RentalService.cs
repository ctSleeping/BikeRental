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

    // Customers see their own rentals, admins see all.
    public async Task<List<RentalDto>> GetAllAsync(int userId, bool isAdmin)
    {
        var query = _context.Rentals.Include(r => r.Bike).AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(r => r.UserId == userId);
        }

        var rentals = await query
            .OrderByDescending(r => r.StartUtc)
            .ToListAsync();

        return rentals.Select(ToDto).ToList();
    }

    public async Task<RentalDto> GetByIdAsync(int id, int userId, bool isAdmin)
    {
        var rental = await FindRentalAsync(id, userId, isAdmin);
        return ToDto(rental);
    }

    public async Task<RentalDto> StartAsync(CreateRentalDto dto, int userId)
    {
        // S1: the bike must exist and not be deleted.
        var bike = await _context.Bikes
            .FirstOrDefaultAsync(b => b.Id == dto.BikeId && b.DeletedUtc == null);

        if (bike == null)
        {
            throw new NotFoundApiException("BikeNotFound", $"Bike with id {dto.BikeId} was not found.");
        }

        // S2: the bike must not be in maintenance or already rented.
        if (bike.InMaintenance)
        {
            throw new ConflictApiException("BikeNotAvailable", "Bike is in maintenance.");
        }

        var bikeIsRented = await _context.Rentals
            .AnyAsync(r => r.BikeId == bike.Id && r.EndUtc == null);

        if (bikeIsRented)
        {
            throw new ConflictApiException("BikeNotAvailable", "Bike is already rented.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundApiException("UserNotFound", $"User with id {userId} was not found.");
        }

        // S3: a user can have only one active rental.
        var userHasActiveRental = await _context.Rentals
            .AnyAsync(r => r.UserId == user.Id && r.EndUtc == null);

        if (userHasActiveRental)
        {
            throw new ConflictApiException("UserHasActiveRental", "User already has an active rental.");
        }

        var rental = new Rental
        {
            Bike = bike,
            User = user,
            StartUtc = DateTime.UtcNow,
            EndUtc = null,
            HourlyPrice = bike.PricePerHour,
            TotalPrice = null
        };

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        return ToDto(rental);
    }

    // S5: only the owner can return a rental.
    public async Task<RentalDto> ReturnAsync(int id, int userId)
    {
        var rental = await FindRentalAsync(id, userId, isAdmin: false);

        // S4: only an active rental can be returned.
        if (rental.EndUtc != null)
        {
            throw new ConflictApiException("RentalNotActive", "Rental is not active.");
        }

        rental.EndUtc = DateTime.UtcNow;
        rental.TotalPrice = CalculatePrice(rental.StartUtc, rental.EndUtc.Value, rental.HourlyPrice);

        await _context.SaveChangesAsync();

        return ToDto(rental);
    }

    // Every started hour is charged in full, with a minimum of one hour.
    public static decimal CalculatePrice(DateTime startUtc, DateTime endUtc, decimal hourlyPrice)
    {
        var durationHours = (endUtc - startUtc).TotalHours;
        var billableHours = Math.Max(1, (int)Math.Ceiling(durationHours));
        return billableHours * hourlyPrice;
    }

    // S5: another user's rental is treated as not found.
    private async Task<Rental> FindRentalAsync(int id, int userId, bool isAdmin)
    {
        var rental = await _context.Rentals
            .Include(r => r.Bike)
            .FirstOrDefaultAsync(r => r.Id == id && (isAdmin || r.UserId == userId));

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