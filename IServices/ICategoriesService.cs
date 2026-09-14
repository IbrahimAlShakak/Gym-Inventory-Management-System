using Gym_Inventory_Management_System.DTOs;

namespace Gym_Inventory_Management_System.IServices;

public interface ICategoriesService
{
    public Task<List<CategoryResponseDTO>> GetAllCategoriesAsync();
    public Task<CategoryResponseDTO?> GetCategoryByIdAsync(int id);
    public Task<CategoryResponseDTO> CreateCategoryAsync(CategoryCreateRequestDTO dto);
    public Task<CategoryResponseDTO?> UpdateCategoryAsync(int id, CategoryUpdateRequestDTO dto);
    public Task<bool> DeleteCategoryAsync(int id);
}
