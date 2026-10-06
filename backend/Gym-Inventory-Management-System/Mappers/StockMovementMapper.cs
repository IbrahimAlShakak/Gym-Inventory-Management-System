using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class StockMovementMapper
{
    public static StockMovementResponseDTO ToDto(StockMovement entity) =>
        new StockMovementResponseDTO
        (entity.Id, entity.ItemId, entity.LocationId, entity.Quantity, entity.Type,
            entity.MovementDate, entity.UserId, entity.Notes);

    public static StockMovement ToEntity(StockMovementCreateDTO dto) =>
        new StockMovement 
        { ItemId = dto.ItemId, LocationId = dto.LocationId, Quantity = dto.Quantity, Type = dto.Type,
            MovementDate = dto.MovementDate, UserId = dto.UserId, Notes = dto.Notes };
}

