using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class StockTransfersService : IStockTransfersService
{
    private readonly AppDbContext _context;
    private readonly IStockLevelHelperService _stockLevelHelperService;
    public StockTransfersService(AppDbContext context, IStockLevelHelperService stockLevelHelperService )
    {
        _context = context;
        _stockLevelHelperService = stockLevelHelperService;
    }

    public async Task<List<StockTransferResponseDTO>> GetAllStockTransfersAsync()
    {
       var stockTransfers = await _context.StockTransfers.AsNoTracking().ToListAsync();
        return stockTransfers.Select(st => StockTransferMapper.ToDTO(st)).ToList();
    }
    public async Task<StockTransferResponseDTO?> GetStockTransferByIdAsync(int id)
    {
        var stockTransfer = await _context.StockTransfers.AsNoTracking().FirstOrDefaultAsync(st => st.Id == id);
        if (stockTransfer == null)
        {
            return null;
        }
        return StockTransferMapper.ToDTO(stockTransfer);
    }

    public async Task<List<StockTransferResponseDTO>?> GetAllStockTransfersForItemAsync(int itemId)
    {
       var stockTransfers = await _context.StockTransfers.AsNoTracking().Where(st => st.ItemId == itemId).ToListAsync();
        if (stockTransfers == null || !stockTransfers.Any())
        {
            return null;
        }
        return stockTransfers.Select(st => StockTransferMapper.ToDTO(st)).ToList();
    }

    public async Task<List<StockTransferResponseDTO>?> GetAllStockTransfersForLocationsAsync(int FromLocationId, int ToLocationId)
    {
        var stockTransfers = await _context.StockTransfers.AsNoTracking()
            .Where(st => st.FromLocationId == FromLocationId && st.ToLocationId == ToLocationId).ToListAsync();
        if (stockTransfers == null || !stockTransfers.Any())
        {
            return null;
        }
        return stockTransfers.Select(st => StockTransferMapper.ToDTO(st)).ToList();
    }

    public async Task<StockTransferResponseDTO?> CreateStockTransferAsync(StockTransferCreateDTO dto)
    {
        var fromStockLevel = await _stockLevelHelperService.GetOrCreateStockLevelAsync(dto.ItemId, dto.FromLocationId);
        var toStockLevel = await _stockLevelHelperService.GetOrCreateStockLevelAsync(dto.ItemId, dto.ToLocationId);
        
        if ( fromStockLevel != null && toStockLevel != null)
        {

            if (fromStockLevel.Quantity < dto.Quantity)
            {
                return null; // or a proper error result, once you tackle error handling
            }

            fromStockLevel.Quantity -= dto.Quantity;
            toStockLevel.Quantity += dto.Quantity;
            var stockTransfer = StockTransferMapper.ToEntity(dto);
            _context.StockTransfers.Add(stockTransfer);
            await _context.SaveChangesAsync();
            return StockTransferMapper.ToDTO(stockTransfer);
        }

        return null;
        
    }
}
