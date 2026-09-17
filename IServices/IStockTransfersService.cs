using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface IStockTransfersService
{
    public Task<List<StockTransferResponseDTO>> GetAllStockTransfersAsync();
    public Task<StockTransferResponseDTO?> GetStockTransferByIdAsync(int id);
    public Task<List<StockTransferResponseDTO>?> GetAllStockTransfersForItemAsync(int itemId);
    public Task<List<StockTransferResponseDTO>?> GetAllStockTransfersForLocationsAsync(int FromLocationId, int ToLocationId);
    public Task<StockTransferResponseDTO?> CreateStockTransferAsync(StockTransferCreateDTO stockTransferRequestDTO);
}
