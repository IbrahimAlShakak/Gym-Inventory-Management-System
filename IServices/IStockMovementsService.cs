using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface IStockMovementsService
{
    public Task<List<StockMovementResponseDTO>> GetAllStockMovementsAsync();
    public Task<StockMovementResponseDTO?> GetStockMovementByIdAsync(int id);
    public Task<StockMovementResponseDTO?> CreateStockMovementAsync(StockMovementCreateDTO dto);
    public Task<List<StockLevelResponseDTO>> GetItemStockLevelsByItemIdAsync(int itemId);
    public Task<List<StockLevelResponseDTO>> GetStockLevelsByLocationIdAsync(int locationId);
    public Task<List<StockLevelResponseDTO>> GetItemStockLevelsAsync(int itemId, int locationId);
}

