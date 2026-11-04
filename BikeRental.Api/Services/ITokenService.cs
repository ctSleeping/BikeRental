using BikeRental.Api.Dtos;
using BikeRental.Api.Models;

namespace BikeRental.Api.Services;

public interface ITokenService
{
    LoginResponseDto CreateToken(User user);
}