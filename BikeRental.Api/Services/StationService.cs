using BikeRental.Api.Common;
using BikeRental.Api.Data;
using BikeRental.Api.Dtos;
using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Services;

public class StationService : IStationService
{
    private readonly AppDbContext _context;

    public StationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StationDto>> GetAllAsync()
    {
        var stations = await _context.Stations
            .OrderBy(s => s.Id)
            .ToListAsync();

        return stations.Select(ToDto).ToList();
    }

    public async Task<StationDto> GetByIdAsync(int id)
    {
        var station = await FindStationAsync(id);
        return ToDto(station);
    }

    public async Task<StationDto> CreateAsync(SaveStationDto dto)
    {
        var station = new Station
        {
            Name = dto.Name,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        return ToDto(station);
    }

    public async Task<StationDto> UpdateAsync(int id, SaveStationDto dto)
    {
        var station = await FindStationAsync(id);

        station.Name = dto.Name;
        station.Address = dto.Address;
        station.Latitude = dto.Latitude;
        station.Longitude = dto.Longitude;

        await _context.SaveChangesAsync();

        return ToDto(station);
    }

    public async Task DeleteAsync(int id)
    {
        var station = await FindStationAsync(id);

        // S10: a station that has bikes cannot be deleted.
        var hasBikes = await _context.Bikes
            .AnyAsync(b => b.StationId == id && b.DeletedUtc == null);

        if (hasBikes)
        {
            throw new ConflictApiException("StationHasBikes", "A station with bikes cannot be deleted.");
        }

        _context.Stations.Remove(station);
        await _context.SaveChangesAsync();
    }

    private async Task<Station> FindStationAsync(int id)
    {
        var station = await _context.Stations.FirstOrDefaultAsync(s => s.Id == id);

        if (station == null)
        {
            throw new NotFoundApiException("StationNotFound", $"Station with id {id} was not found.");
        }

        return station;
    }

    private static StationDto ToDto(Station station)
    {
        return new StationDto
        {
            Id = station.Id,
            Name = station.Name,
            Address = station.Address,
            Latitude = station.Latitude,
            Longitude = station.Longitude
        };
    }
}