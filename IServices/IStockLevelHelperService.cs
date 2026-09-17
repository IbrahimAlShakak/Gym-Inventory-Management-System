using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.IServices;

public interface IStockLevelHelperService
{
    public Task<StockLevel?> GetOrCreateStockLevelAsync(int itemId, int locationId);
}

