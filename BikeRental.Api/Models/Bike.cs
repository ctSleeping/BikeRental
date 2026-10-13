namespace BikeRental.Api.Models;

public class Bike
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public BikeType Type { get; set; }
    public decimal PricePerHour { get; set; }
    public bool InMaintenance { get; set; }
    public DateTime? DeletedUtc { get; set; }

    // NEW: all rentals of this bike (the rental history)
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}