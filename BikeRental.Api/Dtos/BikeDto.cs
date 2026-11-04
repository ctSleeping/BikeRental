using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

public class BikeDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public BikeType Type { get; set; }
    public decimal PricePerHour { get; set; }

    // Computed, not stored.
    public BikeStatus Status { get; set; }

    public int? StationId { get; set; }
}