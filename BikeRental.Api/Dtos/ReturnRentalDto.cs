namespace BikeRental.Api.Dtos;

// Optional body for returning a rental. Without a station,
// the bike is returned to its pickup station.
public class ReturnRentalDto
{
    public int? ReturnStationId { get; set; }
}