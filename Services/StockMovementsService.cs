using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Gym_Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class StockMovementsService : IStockMovementsService
{
    private readonly AppDbContext _context;
    public StockMovementsService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<StockMovementResponseDTO>> GetAllStockMovementsAsync()
    {
        var stockMovements = await _context.StockMovements.AsNoTracking().ToListAsync(); 
        return stockMovements.Select(s => StockMovementMapper.ToDto(s)).ToList();
    }
    public async Task<StockMovementResponseDTO?> GetStockMovementByIdAsync(int id)
    {
        var stockMovement = await _context.StockMovements.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return stockMovement != null ? StockMovementMapper.ToDto(stockMovement) : null;
    }
    public async Task<StockMovementResponseDTO?> CreateStockMovementAsync(StockMovementCreateDTO dto)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == dto.LocationId);
        if (location == null)
        {
            return null;
        }

        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == dto.ItemId);
        if (item == null)
        {
            return null;
        }

        var stockLevel = await _context.StockLevels.FirstOrDefaultAsync(sl => sl.ItemId == dto.ItemId && sl.LocationId == dto.LocationId);
        if( stockLevel == null)
        {
            stockLevel = new StockLevel{ ItemId = dto.ItemId, LocationId = dto.LocationId, Quantity = 0};
            _context.StockLevels.Add(stockLevel);
        }

        var signedQuantity = dto.Type switch
        {
            MovementType.Received => dto.Quantity,
            MovementType.Used => -dto.Quantity,
            MovementType.Damaged => -dto.Quantity,
            MovementType.Adjusted => dto.Quantity,
            _ => throw new ArgumentOutOfRangeException()
        };
        stockLevel.Quantity += signedQuantity;
        var stockMovement = StockMovementMapper.ToEntity(dto);
        _context.StockMovements.Add(stockMovement);
        await _context.SaveChangesAsync();

        return StockMovementMapper.ToDto(stockMovement);
    }
    public async Task<List<StockLevelResponseDTO>> GetItemStockLevelsByItemIdAsync(int itemId)
    {
       var stockLevels = await _context.StockLevels.AsNoTracking().Where(sl => sl.ItemId == itemId).ToListAsync();
        return stockLevels.Select(sl => StockLevelMapper.ToDto(sl)).ToList();
    }
    public async Task<List<StockLevelResponseDTO>> GetStockLevelsByLocationIdAsync(int locationId)
    {
       var stockLevels = await _context.StockLevels.AsNoTracking().Where(sl => sl.LocationId == locationId).ToListAsync();
        return stockLevels.Select(sl => StockLevelMapper.ToDto(sl)).ToList();
    }
    public async Task<List<StockLevelResponseDTO>> GetItemStockLevelsAsync(int itemId, int locationId)
    {
       var stockLevels = await _context.StockLevels.AsNoTracking().Where(sl => sl.ItemId == itemId && sl.LocationId == locationId).ToListAsync();
        return stockLevels.Select(sl => StockLevelMapper.ToDto(sl)).ToList();
    }
}
