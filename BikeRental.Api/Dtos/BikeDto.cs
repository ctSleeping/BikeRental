using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

public class BikeDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public BikeType Type { get; set; }
    public decimal PricePerHour { get; set; }

    // Available, Rented or InMaintenance. Computed, not stored.
    public string Status { get; set; } = string.Empty;

    public int? StationId { get; set; }
}