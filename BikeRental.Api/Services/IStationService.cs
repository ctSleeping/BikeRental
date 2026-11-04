using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

public interface IStationService
{
    Task<List<StationDto>> GetAllAsync();
    Task<StationDto> GetByIdAsync(int id);
    Task<StationDto> CreateAsync(SaveStationDto dto);
    Task<StationDto> UpdateAsync(int id, SaveStationDto dto);
    Task DeleteAsync(int id);
}