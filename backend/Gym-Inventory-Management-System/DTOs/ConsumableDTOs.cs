using System.ComponentModel.DataAnnotations;
namespace Gym_Inventory_Management_System.DTOs;

public record ConsumableResponseDTO
    (int Id, string SKU, string Name, int CategoryId, string Unit, int ReorderThreshold, DateOnly? ExpiryDate);


public record ConsumableCreateRequestDTO
 ([Required(ErrorMessage = "Stock Keeping Unit is required.")]
        [MinLength(6, ErrorMessage = "Stock Keeping Unit must be at least 6 characters long.")]
    string SKU,
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    int CategoryId,
    [Required(ErrorMessage = "Unit is required.")]
        [MinLength(3, ErrorMessage = "Unit must be at least 3 characters long.")]
    string Unit,
    int ReorderThreshold,
    DateOnly? ExpiryDate);

public record ConsumableUpdateRequestDTO
 ([Required(ErrorMessage = "Stock Keeping Unit is required.")]
        [MinLength(6, ErrorMessage = "Stock Keeping Unit must be at least 6 characters long.")]
    string SKU,
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    int CategoryId,
    [Required(ErrorMessage = "Unit is required.")]
        [MinLength(3, ErrorMessage = "Unit must be at least 3 characters long.")]
    string Unit,
    int ReorderThreshold,
    DateOnly? ExpiryDate);

