namespace BikeRental.Api.Models;

public class Station
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}