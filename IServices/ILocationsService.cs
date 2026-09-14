using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface ILocationsService
{
    public Task<List<LocationResponseDTO>> GetAllLocationsAsync();
    public Task<LocationResponseDTO?> GetLocationByIdAsync(int id);
    public Task<LocationResponseDTO> CreateLocationAsync(LocationCreateRequestDTO dto);
    public Task<LocationResponseDTO?> UpdateLocationAsync(int id, LocationUpdateRequestDTO dto);
    public Task<bool> DeleteLocationAsync(int id);
}
