using System.Security.Claims;

namespace BikeRental.Api.Common;

public static class ClaimsPrincipalExtensions
{
    // Reads the logged-in user's id from the validated JWT token.
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(value, out var id))
        {
            throw new UnauthorizedApiException("InvalidToken", "Token does not contain a valid user id.");
        }

        return id;
    }
}