using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

public interface IBikeService
{
    Task<PagedResult<BikeDto>> GetAllAsync(BikeQuery filter);
    Task<BikeDto> GetByIdAsync(int id);
    Task<BikeDto> CreateAsync(CreateBikeDto dto);
    Task<BikeDto> UpdateAsync(int id, UpdateBikeDto dto);
    Task DeleteAsync(int id);
}