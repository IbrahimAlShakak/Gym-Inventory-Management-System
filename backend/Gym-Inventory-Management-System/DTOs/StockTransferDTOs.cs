using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record StockTransferResponseDTO
    (int Id, int ItemId, int FromLocationId, int ToLocationId, int Quantity, DateTime TransferDate, int UserId);

public record StockTransferCreateDTO
    (
    int ItemId,
    int FromLocationId,
    int ToLocationId, 
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")] int Quantity,
    DateTime TransferDate
    );
