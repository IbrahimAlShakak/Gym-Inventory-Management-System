using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _context;
    public EquipmentService(AppDbContext context)
    {
        _context = context; 
    }

    public async Task<List<EquipmentResponseDTO>> GetAllEquipmentAsync()
    {
        var equipmentList = await _context.Equipment.AsNoTracking().Include(e => e.Category).ToListAsync();
        return equipmentList.Select(e => EquipmentMapper.ToDto(e)).ToList();
    }

    public async Task<EquipmentResponseDTO> GetEquipmentByIdAsync(int id)
    {
        var equipment = await _context.Equipment.AsNoTracking().Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
        if (equipment == null)
        {
            throw new NotFoundException($"Equipment not found with the specified ID = {id}");
        }
        return EquipmentMapper.ToDto(equipment);
    }
    

    public async Task<EquipmentResponseDTO> CreateEquipmentAsync(EquipmentCreateRequestDTO dto)
    {
        var isSkuExists = await _context.Items.AnyAsync(e => e.SKU == dto.SKU);
        if (isSkuExists)
        {
            throw new ConflictException($"Equipment with the specified SKU = {dto.SKU} already exists");
        }
        var isSerialNumberExists = await _context.Equipment.AnyAsync(e => e.SerialNumber == dto.SerialNumber);
        if (isSerialNumberExists)
        {
            throw new ConflictException($"Equipment with the specified Serial Number = {dto.SerialNumber} already exists");
        }
        var isCategoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!isCategoryExists)
        {
            throw new NotFoundException($"Category not found with the specified ID = {dto.CategoryId}");    
        }
        var equipment = EquipmentMapper.ToEntity(dto);
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return EquipmentMapper.ToDto(equipment);
    }

    public async Task<EquipmentResponseDTO> UpdateEquipmentAsync(int id, EquipmentUpdateRequestDTO dto)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment != null)
        {
            var isSkuExists = await _context.Items.AnyAsync(e => e.SKU == dto.SKU);
            if (isSkuExists)
            {
                throw new ConflictException($"Equipment with the specified SKU = {dto.SKU} already exists");
            }
            var isSerialNumberExists = await _context.Equipment.AnyAsync(e => e.SerialNumber == dto.SerialNumber);
            if (isSerialNumberExists)
            {
                throw new ConflictException($"Equipment with the specified Serial Number = {dto.SerialNumber} already exists");
            }
            var isCategoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!isCategoryExists)
            {
                throw new NotFoundException($"Category not found with the specified ID = {dto.CategoryId}");
            }
            equipment.SKU = dto.SKU;
            equipment.Name = dto.Name;
            equipment.CategoryId = dto.CategoryId;
            equipment.SerialNumber = dto.SerialNumber;
            equipment.PurchaseDate = dto.PurchaseDate;
            equipment.WarrantyExpiry = dto.WarrantyExpiry;
            equipment.MaintenanceStatus = dto.MaintenanceStatus;
            equipment.Condition = dto.Condition;
            await _context.SaveChangesAsync();
            return EquipmentMapper.ToDto(equipment);

        }
        throw new NotFoundException($"Equipment not found with the specified ID = {id}");
    }

    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment != null)
        {
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
            return true;
        }
        throw new NotFoundException($"Equipment not found with the specified ID = {id}");
    }

}