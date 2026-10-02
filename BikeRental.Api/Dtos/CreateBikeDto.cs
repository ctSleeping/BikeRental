using System.ComponentModel.DataAnnotations;
using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

// What the client sends IN when creating a bike.
public class CreateBikeDto
{
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public BikeType? Type { get; set; }

    [Range(0.01, 1000)]
    public decimal PricePerHour { get; set; }
}