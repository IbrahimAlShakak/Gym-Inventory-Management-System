using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record LocationResponseDTO(int Id, string Name, string Address, string ContactNumber);
public record LocationCreateRequestDTO(
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    [Required(ErrorMessage = "Address is required.")]
        [MinLength(6, ErrorMessage = "Address must be at least 6 characters long.")]
    string Address,
    [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid contact number.")]
    string ContactNumber);
public record LocationUpdateRequestDTO(
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    [Required(ErrorMessage = "Address is required.")]
        [MinLength(6, ErrorMessage = "Address must be at least 6 characters long.")]
    string Address,
    [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid contact number.")]
    string ContactNumber);
