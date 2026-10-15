namespace BikeRental.Api.Dtos;

public class RentalDto
{
    public int Id { get; set; }
    public BikeSummaryDto Bike { get; set; } = new();
    public DateTime StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
    public decimal HourlyPrice { get; set; }
    public decimal? TotalPrice { get; set; }

    // Active or Completed, based on EndUtc.
    public string Status { get; set; } = string.Empty;
}

public class BikeSummaryDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
}