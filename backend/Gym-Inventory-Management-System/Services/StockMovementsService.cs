using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Gym_Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class StockMovementsService : IStockMovementsService
{
    private readonly AppDbContext _context;
    private readonly IStockLevelHelperService _stockLevelHelperService;
    public StockMovementsService(AppDbContext context, IStockLevelHelperService stockLevelHelperService)
    {
        _context = context;
        _stockLevelHelperService = stockLevelHelperService;
    }
    public async Task<List<StockMovementResponseDTO>> GetAllStockMovementsAsync()
    {
        var stockMovements = await _context.StockMovements
            .Include(s => s.Item)
            .Include(s => s.Location)
            .Include(s => s.User)
            .AsNoTracking()
            .ToListAsync();
        return stockMovements.Select(s => StockMovementMapper.ToDto(s)).ToList();
    }
    public async Task<StockMovementResponseDTO> GetStockMovementByIdAsync(int id)
    {
        var stockMovement = await _context.StockMovements.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (stockMovement == null)
        {
            throw new NotFoundException($"Stock movement not found with the specified ID = {id}");
        }
        return StockMovementMapper.ToDto(stockMovement);
    }
    public async Task<StockMovementResponseDTO> CreateStockMovementAsync(StockMovementCreateDTO dto)
    {
        var isUserExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!isUserExists)
        {
            throw new NotFoundException($"User not found with the specified ID = {dto.UserId}");
        }

        var stockLevel = await _stockLevelHelperService.GetOrCreateStockLevelAsync(dto.ItemId, dto.LocationId);

        var signedQuantity = dto.Type switch
        {
            MovementType.Received => dto.Quantity,
            MovementType.Used => -dto.Quantity,
            MovementType.Damaged => -dto.Quantity,
            MovementType.Adjusted => dto.Quantity,
            _ => throw new ArgumentOutOfRangeException()
        };

        var newQuantity = stockLevel.Quantity + signedQuantity;
        if (newQuantity < 0)
        {
            throw new InvalidOperationBusinessException(
                $"Insufficient stock. Available quantity: {stockLevel.Quantity}, requested quantity: {dto.Quantity}");
        }

        stockLevel.Quantity = newQuantity;
        var stockMovement = StockMovementMapper.ToEntity(dto);
        _context.StockMovements.Add(stockMovement);
        await _context.SaveChangesAsync();

        return StockMovementMapper.ToDto(stockMovement);
    }
    public async Task<List<StockLevelResponseDTO>> GetItemStockLevelsByItemIdAsync(int itemId)
    {
        var itemExists = await _context.Items.AnyAsync(i => i.Id == itemId);
        if (!itemExists)
        {
            throw new NotFoundException($"Item not found with the specified ID = {itemId}");
        }
        var stockLevels = await _context.StockLevels.AsNoTracking().Where(sl => sl.ItemId == itemId).ToListAsync();
        return stockLevels.Select(sl => StockLevelMapper.ToDto(sl)).ToList();
    }
    public async Task<List<StockLevelResponseDTO>> GetStockLevelsByLocationIdAsync(int locationId)
    {
        var locationExists = await _context.Locations.AnyAsync(l => l.Id == locationId);
        if (!locationExists)
        {
            throw new NotFoundException($"Location not found with the specified ID = {locationId}");
        }
        var stockLevels = await _context.StockLevels.AsNoTracking().Where(sl => sl.LocationId == locationId).ToListAsync();
        return stockLevels.Select(sl => StockLevelMapper.ToDto(sl)).ToList();
    }
    public async Task<List<StockLevelResponseDTO>> GetItemStockLevelsAsync(int itemId, int locationId)
    {
        var itemExists = await _context.Items.AnyAsync(i => i.Id == itemId);
        if (!itemExists)
        {
            throw new NotFoundException($"Item not found with the specified ID = {itemId}");
        }

        var locationExists = await _context.Locations.AnyAsync(l => l.Id == locationId);
        if (!locationExists)
        {
            throw new NotFoundException($"Location not found with the specified ID = {locationId}");
        }

        var stockLevels = await _context.StockLevels.AsNoTracking().Where(sl => sl.ItemId == itemId && sl.LocationId == locationId).ToListAsync();
        return stockLevels.Select(sl => StockLevelMapper.ToDto(sl)).ToList();
    }
}

