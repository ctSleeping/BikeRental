using BikeRental.Api.Common;
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

    // Projects directly to the DTO, so PasswordHash is never loaded.
    public async Task<List<UserDto>> GetAllAsync()
    {
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

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new NotFoundApiException("UserNotFound", $"User with id {id} was not found.");
        }

        return user;
    }
}