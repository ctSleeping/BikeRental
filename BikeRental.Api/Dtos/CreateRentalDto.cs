using System.ComponentModel.DataAnnotations;

namespace BikeRental.Api.Dtos;

// The renting user is read from the JWT token, not from the request.
public class CreateRentalDto
{
    [Required]
    public int? BikeId { get; set; }
}