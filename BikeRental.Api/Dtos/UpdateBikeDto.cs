using System.ComponentModel.DataAnnotations;
using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

// What the client sends IN when updating a bike.
public class UpdateBikeDto
{
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public BikeType? Type { get; set; }

    [Range(0.01, 1000)]
    public decimal PricePerHour { get; set; }

    // The only status-related thing the client may change (rule S6).
    public bool InMaintenance { get; set; }
}