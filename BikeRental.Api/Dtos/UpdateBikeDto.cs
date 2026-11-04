using System.ComponentModel.DataAnnotations;
using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

public class UpdateBikeDto
{
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public BikeType? Type { get; set; }

    [Range(0.01, 1000)]
    public decimal PricePerHour { get; set; }

    // Status cannot be set directly (S6); only the maintenance flag can be changed.
    public bool InMaintenance { get; set; }

    public int? StationId { get; set; }
}