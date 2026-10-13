namespace BikeRental.Api.Models;

public class Rental
{
    public int Id { get; set; }

    // Foreign key + navigation property to the user who rents
    public int UserId { get; set; }
    public User? User { get; set; }

    // Foreign key + navigation property to the rented bike
    public int BikeId { get; set; }
    public Bike? Bike { get; set; }

    public DateTime StartUtc { get; set; }

    // null = rental is still active
    public DateTime? EndUtc { get; set; }

    // The bike's price per hour at the moment the rental started.
    // Copied here so later price changes don't affect this rental.
    public decimal HourlyPrice { get; set; }

    // null while active, calculated when the bike is returned
    public decimal? TotalPrice { get; set; }
}