using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record StockTransferResponseDTO
    (int Id, int ItemId, int FromLocationId, int ToLocationId, int Quantity, DateTime TransferDate, int UserId);

public record StockTransferCreateDTO
    ([Required(ErrorMessage = "ItemId is required.")]  int ItemId,
    [Required(ErrorMessage = "FromLocationId is required.")] int FromLocationId,
    [Required(ErrorMessage = "ToLocationId is required.")] int ToLocationId, 
    [Required(ErrorMessage = "Quantity is required."), Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")] int Quantity, DateTime TransferDate,
    [Required(ErrorMessage = "UserId is required.")] int UserId);
