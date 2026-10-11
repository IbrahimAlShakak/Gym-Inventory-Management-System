using Gym_Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record StockMovementResponseDTO
    (int Id, int ItemId, int LocationId, int Quantity, MovementType Type, DateOnly MovementDate, int UserId, string Notes);

public record StockMovementCreateDTO
    (
        int ItemId,
        int LocationId,
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        int Quantity,
        MovementType Type,
        DateOnly MovementDate,
        int UserId,
        [MinLength(6, ErrorMessage = "Notes must be at least 6 characters long.")]
        string Notes
    );