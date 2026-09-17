using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class StockLevelHelperService : IStockLevelHelperService
{
    private readonly AppDbContext _context;
    public StockLevelHelperService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<StockLevel?> GetOrCreateStockLevelAsync(int itemId, int locationId)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
        if (location == null)
        {
            return null;
        }

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null)
        {
            return null;
        }

        var stockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ItemId == itemId && sl.LocationId == locationId);
        if (stockLevel == null)
        {
            stockLevel = new StockLevel
            {
                ItemId = itemId,
                LocationId = locationId,
                Quantity = 0
            };
            _context.StockLevels.Add(stockLevel);
        }
        return stockLevel;
    }
}

