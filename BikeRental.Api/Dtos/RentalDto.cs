namespace BikeRental.Api.Dtos;

// What the API sends OUT for a rental.
// Matches the example JSON on the course page field by field.
public class RentalDto
{
    public int Id { get; set; }
    public BikeSummaryDto Bike { get; set; } = new();
    public DateTime StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
    public decimal HourlyPrice { get; set; }
    public decimal? TotalPrice { get; set; }

    // "Active" or "Completed" – computed from EndUtc, never stored.
    public string Status { get; set; } = string.Empty;
}

// The small bike object inside a rental: { "id": 12, "model": "Helkama Jopo" }
public class BikeSummaryDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
}