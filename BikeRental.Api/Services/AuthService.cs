using BikeRental.Api.Common;
using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        // S11: an email can only be registered once.
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            throw new ConflictApiException("EmailTaken", "Email is already registered.");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = email,
            Role = UserRole.Customer
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Same error for unknown email and wrong password.
        if (user == null ||
            _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedApiException("InvalidCredentials", "Invalid email or password.");
        }

        return _tokenService.CreateToken(user);
    }
}