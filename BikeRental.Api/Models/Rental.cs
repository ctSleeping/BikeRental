namespace BikeRental.Api.Models;

public class Rental
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int BikeId { get; set; }
    public Bike? Bike { get; set; }

    public DateTime StartUtc { get; set; }

    // Null while the rental is active.
    public DateTime? EndUtc { get; set; }

    // The bike's hourly price at the start of the rental.
    public decimal HourlyPrice { get; set; }

    // Calculated when the rental is returned.
    public decimal? TotalPrice { get; set; }
}