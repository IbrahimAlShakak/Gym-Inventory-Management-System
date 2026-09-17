using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServicesl;
using Gym_Inventory_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class ConsumablesService : IConsumablesService
{
    private readonly AppDbContext _context;
    public ConsumablesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsumableResponseDTO>> GetAllConsumablesAsync()
    {
        var consumablesList = await _context.Consumables.AsNoTracking().Include(c => c.Category).ToListAsync();
        if (consumablesList == null || !consumablesList.Any())
        {
            throw new NotFoundException("No consumables found in the database.");
        }
        return consumablesList.Select(c => ConsumableMapper.ToDto(c)).ToList();
    }

    public async Task<ConsumableResponseDTO> GetConsumableByIdAsync(int id)
    {
        var consumable = await _context.Consumables.AsNoTracking().Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
        if (consumable == null)
        {
            throw new NotFoundException("Consumable not found.");
        }
        return ConsumableMapper.ToDto(consumable);
    }

    public async Task<ConsumableResponseDTO> CreateConsumableAsync(ConsumableCreateRequestDTO dto)
    {
        var isCategoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!isCategoryExists)
        {
            throw new NotFoundException("Category not found.");
        }
        var consumable = ConsumableMapper.ToEntity(dto);
        _context.Consumables.Add(consumable);
        await _context.SaveChangesAsync();
        return ConsumableMapper.ToDto(consumable);
    }

    public async Task<ConsumableResponseDTO> UpdateConsumableAsync(int id, ConsumableUpdateRequestDTO dto)
    {
        var consumable = await _context.Consumables.FindAsync(id);
        if (consumable != null)
        {
            var isCategoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!isCategoryExists)
            {
                throw new NotFoundException("Category not found.");
            }
            consumable.SKU = dto.SKU;
            consumable.Name = dto.Name;
            consumable.CategoryId = dto.CategoryId;
            consumable.Unit = dto.Unit;
            consumable.ReorderThreshold = dto.ReorderThreshold;
            consumable.ExpiryDate = dto.ExpiryDate;
            await _context.SaveChangesAsync();
            return ConsumableMapper.ToDto(consumable);
        }
        throw new NotFoundException($"Consumable not found with the specified ID = {id}");
    }

    public async Task<bool> DeleteConsumableAsync(int id)
    {
        var consumable = await _context.Consumables.FindAsync(id);
        if (consumable != null)
        {
            _context.Consumables.Remove(consumable);
            await _context.SaveChangesAsync();
            return true;
        }
        throw new NotFoundException($"Consumable not found with the specified ID = {id}");
    }

    public async Task<List<ConsumableResponseDTO>> GetLowStockConsumablesAsync()
    {
        var lowStockConsumables = await _context.Consumables.AsNoTracking()
            .Include(c => c.StockLevels)
            .Where(c => c.StockLevels.Any(s => s.Quantity <= c.ReorderThreshold))
            .ToListAsync();
        return lowStockConsumables.Select(c => ConsumableMapper.ToDto(c)).ToList();
    }
}