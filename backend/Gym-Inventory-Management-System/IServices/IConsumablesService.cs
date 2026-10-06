using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface IConsumablesService
{
    public Task<List<ConsumableResponseDTO>> GetAllConsumablesAsync();
    public Task<ConsumableResponseDTO> GetConsumableByIdAsync(int id);
    public Task<ConsumableResponseDTO> CreateConsumableAsync(ConsumableCreateRequestDTO dto);
    public Task<ConsumableResponseDTO> UpdateConsumableAsync(int id, ConsumableUpdateRequestDTO dto);
    public Task<bool> DeleteConsumableAsync(int id);
    public Task<List<ConsumableResponseDTO>> GetLowStockConsumablesAsync();
}

