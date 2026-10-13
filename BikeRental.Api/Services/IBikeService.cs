using BikeRental.Api.Dtos;

namespace BikeRental.Api.Services;

// The "contract": what the bike service can do, without saying how.
// The controller depends on this interface, not on the class.
public interface IBikeService
{
    Task<PagedResult<BikeDto>> GetAllAsync(int page, int pageSize);
    Task<BikeDto> GetByIdAsync(int id);
    Task<BikeDto> CreateAsync(CreateBikeDto dto);
    Task<BikeDto> UpdateAsync(int id, UpdateBikeDto dto);
    Task DeleteAsync(int id);
}