using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;

public static class StockLevelMapper
{
    public static StockLevelResponseDTO ToDto (StockLevel entity) =>
        new StockLevelResponseDTO(entity.Id, entity.ItemId, entity.LocationId, entity.Quantity);

}
