using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        // Select runs in the database: only these columns are read,
        // so PasswordHash never even leaves the database.
        return await _context.Users
            .OrderBy(u => u.Id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }
}