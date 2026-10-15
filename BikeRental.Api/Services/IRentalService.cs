using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

public interface IRentalService
{
    Task<List<RentalDto>> GetAllAsync();
    Task<RentalDto> GetByIdAsync(int id);
    Task<RentalDto> StartAsync(CreateRentalDto dto);
    Task<RentalDto> ReturnAsync(int id);
}