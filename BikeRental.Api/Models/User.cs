namespace BikeRental.Api.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Only a hash is ever stored, never the password itself.
    // Not used until level 4 (login); seed users get a placeholder.
    // Never returned from the API.
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Customer;

    // Navigation property: all rentals made by this user.
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}