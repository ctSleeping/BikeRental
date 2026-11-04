using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}