using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Exceptions;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System.Services;

public class CategoriesService : ICategoriesService
{
    private readonly AppDbContext _context;
    public CategoriesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponseDTO>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        return categories.Select(c => CategoryMapper.ToDto(c)).ToList();
    }

    public async Task<CategoryResponseDTO> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new NotFoundException("Category not found.");
        }
        return CategoryMapper.ToDto(category);
    }

    public async Task<CategoryResponseDTO> CreateCategoryAsync(CategoryCreateRequestDTO dto)
    {
        var category = CategoryMapper.ToEntity(dto);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CategoryMapper.ToDto(category);
    }

    public async Task<CategoryResponseDTO> UpdateCategoryAsync(int id, CategoryUpdateRequestDTO dto)
    {
        var existingCategory = await _context.Categories.FindAsync(id);
        if (existingCategory == null)
        {
            throw new NotFoundException("Category not found.");
        }
        existingCategory.Name = dto.Name;
        existingCategory.Description = dto.Description;
        await _context.SaveChangesAsync();
        return CategoryMapper.ToDto(existingCategory);
    }
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new NotFoundException("Category not found.") ;
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
