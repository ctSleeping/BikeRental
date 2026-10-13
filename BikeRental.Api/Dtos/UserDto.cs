using BikeRental.Api.Models;

namespace BikeRental.Api.Dtos;

// What the API shows about a user. No PasswordHash, ever.
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}