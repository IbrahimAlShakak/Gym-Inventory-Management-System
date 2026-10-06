using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class  ConsumableMapper
{

    public static ConsumableResponseDTO ToDto(Consumable entity) =>
       new ConsumableResponseDTO(entity.Id, entity.SKU, entity.Name, entity.CategoryId, entity.Unit, entity.ReorderThreshold,entity.ExpiryDate);

    public static Consumable ToEntity(ConsumableCreateRequestDTO dto) =>
        new Consumable
        {
            SKU = dto.SKU,
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            Unit = dto.Unit,
            ReorderThreshold = dto.ReorderThreshold,
            ExpiryDate = dto.ExpiryDate
        };
}