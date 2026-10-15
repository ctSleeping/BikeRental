using System.ComponentModel.DataAnnotations;

namespace BikeRental.Api.Dtos;

public class CreateRentalDto
{
    [Required]
    public int? BikeId { get; set; }

    // No authentication yet, so the renting user is given in the request.
    [Required]
    public int? UserId { get; set; }
}