using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class StockTransfersService : IStockTransfersService
{
    private readonly AppDbContext _context;
    private readonly IStockLevelHelperService _stockLevelHelperService;
    public StockTransfersService(AppDbContext context, IStockLevelHelperService stockLevelHelperService)
    {
        _context = context;
        _stockLevelHelperService = stockLevelHelperService;
    }

    public async Task<List<StockTransferResponseDTO>> GetAllStockTransfersAsync()
    {
        var stockTransfers = await _context.StockTransfers
             .Include(st => st.Item)
             .Include(st => st.FromLocation)
             .Include(st => st.ToLocation)
             .Include(st => st.User)
             .AsNoTracking().ToListAsync();
        return stockTransfers.Select(st => StockTransferMapper.ToDTO(st)).ToList();
    }
    public async Task<StockTransferResponseDTO> GetStockTransferByIdAsync(int id)
    {
        var stockTransfer = await _context.StockTransfers
            .Include(st => st.Item)
            .Include(st => st.FromLocation)
            .Include(st => st.ToLocation)
            .Include(st => st.User)
            .AsNoTracking().FirstOrDefaultAsync(st => st.Id == id);
        if (stockTransfer == null)
        {
            throw new NotFoundException($"Stock transfer with ID {id} not found.");
        }
        return StockTransferMapper.ToDTO(stockTransfer);
    }

    public async Task<List<StockTransferResponseDTO>> GetAllStockTransfersForItemAsync(int itemId)
    {
        var itemExists = await _context.Items.AnyAsync(i => i.Id == itemId);
        if (!itemExists)
        {
            throw new NotFoundException($"Item not found with the specified ID = {itemId}");
        }
        var stockTransfers = await _context.StockTransfers
            .Include(st => st.Item)
            .Include(st => st.FromLocation)
            .Include(st => st.ToLocation)
            .Include(st => st.User)
            .AsNoTracking().Where(st => st.ItemId == itemId).ToListAsync();
        return stockTransfers.Select(st => StockTransferMapper.ToDTO(st)).ToList();
    }

    public async Task<List<StockTransferResponseDTO>> GetAllStockTransfersForLocationsAsync(int FromLocationId, int ToLocationId)
    {
        var itemExists = await _context.Locations.AnyAsync(i => i.Id == FromLocationId);
        if (!itemExists)
        {
            throw new NotFoundException($"Location not found with the specified ID = {FromLocationId}");
        }
        var toLocationExists = await _context.Locations.AnyAsync(l => l.Id == ToLocationId);
        if (!toLocationExists)
        {
            throw new NotFoundException($"Location not found with the specified ID = {ToLocationId}");
        }
        var stockTransfers = await _context.StockTransfers
            .Include(st => st.Item)
            .Include(st => st.FromLocation)
            .Include(st => st.ToLocation)
            .Include(st => st.User)
            .AsNoTracking()
            .Where(st => st.FromLocationId == FromLocationId && st.ToLocationId == ToLocationId).ToListAsync();
        return stockTransfers.Select(st => StockTransferMapper.ToDTO(st)).ToList();
    }

    public async Task<StockTransferResponseDTO> CreateStockTransferAsync(StockTransferCreateDTO dto)
    {
        if (dto.FromLocationId == dto.ToLocationId)
        {
            throw new InvalidOperationBusinessException("Source and destination locations cannot be the same.");
        }

        var isUserExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!isUserExists)
        {
            throw new NotFoundException($"User not found with the specified ID = {dto.UserId}");
        }

        var fromStockLevel = await _stockLevelHelperService.GetOrCreateStockLevelAsync(dto.ItemId, dto.FromLocationId);
        var toStockLevel = await _stockLevelHelperService.GetOrCreateStockLevelAsync(dto.ItemId, dto.ToLocationId);

        if (fromStockLevel.Quantity < dto.Quantity)
        {
            throw new InvalidOperationBusinessException($"Insufficient stock in the source location. Available quantity: {fromStockLevel.Quantity}, requested transfer quantity: {dto.Quantity}");

        }

        fromStockLevel.Quantity -= dto.Quantity;
        toStockLevel.Quantity += dto.Quantity;
        var stockTransfer = StockTransferMapper.ToEntity(dto);
        _context.StockTransfers.Add(stockTransfer);
        await _context.SaveChangesAsync();
        return StockTransferMapper.ToDTO(stockTransfer);

    }
}
