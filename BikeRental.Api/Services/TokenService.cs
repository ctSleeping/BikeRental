using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace BikeRental.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public LoginResponseDto CreateToken(User user)
    {
        // Claims stored in the token and read back on every request.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // The token is signed with the secret key, so it cannot be modified
        // without the signature check failing.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresUtc = expires
        };
    }
}