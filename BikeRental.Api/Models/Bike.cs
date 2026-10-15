namespace BikeRental.Api.Models;

public class Bike
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public BikeType Type { get; set; }
    public decimal PricePerHour { get; set; }
    public bool InMaintenance { get; set; }

    // Set when the bike is soft-deleted; null for active bikes.
    public DateTime? DeletedUtc { get; set; }

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}