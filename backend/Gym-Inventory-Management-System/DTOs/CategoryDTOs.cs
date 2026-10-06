using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record CategoryResponseDTO(int Id, string Name, string Description);
public record CategoryCreateRequestDTO(
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
string Name,
    [Required(ErrorMessage = "Description is required.")]
        [MinLength(6, ErrorMessage = "Description must be at least 6 characters long.")]
    string Description);
public record CategoryUpdateRequestDTO(
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    [Required(ErrorMessage = "Description is required.")]
        [MinLength(6, ErrorMessage = "Description must be at least 6 characters long.")]
    string Description);
