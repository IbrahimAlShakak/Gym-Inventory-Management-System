using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class LocationsService : ILocationsService
{
    private readonly AppDbContext _context;

    public LocationsService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<LocationResponseDTO>> GetAllLocationsAsync()
    {
        var locations = await _context.Locations.AsNoTracking().ToListAsync();
        return locations.Select(c => LocationMapper.ToDto(c)).ToList();
    }

    public async Task<LocationResponseDTO> GetLocationByIdAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            throw new NotFoundException($"Location not found with the specified ID = {id}");
        }
        return LocationMapper.ToDto(location);
    }

    public async Task<LocationResponseDTO> CreateLocationAsync(LocationCreateRequestDTO dto)
    {
        var newLocation = LocationMapper.ToEntity(dto);
        _context.Locations.Add(newLocation);
        await _context.SaveChangesAsync();
        return LocationMapper.ToDto(newLocation);
    }

    public async Task<LocationResponseDTO> UpdateLocationAsync(int id, LocationUpdateRequestDTO dto)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            location.Name = dto.Name;
            location.Address = dto.Address;
            location.ContactNumber = dto.ContactNumber;
            await _context.SaveChangesAsync();
            return LocationMapper.ToDto(location);
        }
        throw new NotFoundException($"Location not found with the specified ID = {id}");
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            throw new NotFoundException($"Location not found with the specified ID = {id}");
        }
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return true;
    }
    
}

