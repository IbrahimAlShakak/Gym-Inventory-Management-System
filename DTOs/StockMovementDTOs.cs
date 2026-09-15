using Gym_Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record StockMovementResponseDTO
    (int Id, int ItemId, int LocationId, int Quantity, MovementType Type, DateOnly MovementDate, int UserId, string Notes);

public record StockMovementCreateDTO
    ([Required(ErrorMessage = "Item Id is required.")]
        int ItemId,
    [Required(ErrorMessage = "Location Id is required.")]
        int LocationId,
    [Required(ErrorMessage = "Quantity is required.")]
        int Quantity,
    [Required(ErrorMessage = "Type is required.")]
        MovementType Type,
    [Required(ErrorMessage = "Movement Date is required.")]
        DateOnly MovementDate,
    [Required(ErrorMessage = "User Id is required.")]
        int UserId,
    [MinLength(6, ErrorMessage = "Notes must be at least 6 characters long.")]
        string Notes);