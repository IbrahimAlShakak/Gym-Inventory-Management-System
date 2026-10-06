using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class EquipmentMapper
{
    public static EquipmentResponseDTO ToDto(Equipment entity) =>
       new EquipmentResponseDTO(entity.Id, entity.SKU, entity.Name, entity.CategoryId, entity.SerialNumber, entity.PurchaseDate,
           entity.WarrantyExpiry, entity.MaintenanceStatus, entity.Condition);

    public static Equipment ToEntity(EquipmentCreateRequestDTO dto) =>
        new Equipment {
            SKU = dto.SKU,
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            SerialNumber = dto.SerialNumber,
            PurchaseDate = dto.PurchaseDate,
            WarrantyExpiry = dto.WarrantyExpiry,
            MaintenanceStatus = dto.MaintenanceStatus,
            Condition = dto.Condition };
}
