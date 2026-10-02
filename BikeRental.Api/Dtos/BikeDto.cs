using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

// What the API sends OUT when it shows a bike.
public class BikeDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public BikeType Type { get; set; }
    public decimal PricePerHour { get; set; }
    public string Status { get; set; } = string.Empty;
}