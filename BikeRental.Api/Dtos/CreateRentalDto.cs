using System.ComponentModel.DataAnnotations;

namespace BikeRental.Api.Dtos;

// What the client sends IN to start a rental.
public class CreateRentalDto
{
    [Required]
    public int? BikeId { get; set; }

    // Level 3 only: there is no login yet, so the request says who rents.
    // At level 4 this field is removed and the user comes from the JWT token.
    [Required]
    public int? UserId { get; set; }
}