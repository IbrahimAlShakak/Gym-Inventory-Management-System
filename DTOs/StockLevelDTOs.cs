namespace Gym_Inventory_Management_System.DTOs;

public record StockLevelResponseDTO
    (int Id, int ItemId, int LocationId, int Quantity);
