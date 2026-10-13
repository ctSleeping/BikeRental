using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
}