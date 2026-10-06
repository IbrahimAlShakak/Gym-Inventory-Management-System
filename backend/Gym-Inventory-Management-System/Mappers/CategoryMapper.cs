using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class CategoryMapper
{
    public static CategoryResponseDTO ToDto(Category entity) =>
       new CategoryResponseDTO(entity.Id, entity.Name, entity.Description);

    public static Category ToEntity(CategoryCreateRequestDTO dto) =>
        new Category { Name = dto.Name, Description = dto.Description };
}

