using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

public interface IRentalService
{
    Task<List<RentalDto>> GetAllAsync(int userId, bool isAdmin);
    Task<RentalDto> GetByIdAsync(int id, int userId, bool isAdmin);
    Task<RentalDto> StartAsync(CreateRentalDto dto, int userId);
    Task<RentalDto> ReturnAsync(int id, int userId);
}