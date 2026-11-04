using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

// Query string parameters for GET /api/bikes. All filters are optional.
public class BikeQuery
{
    public BikeType? Type { get; set; }
    public BikeStatus? Status { get; set; }
    public int? StationId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}