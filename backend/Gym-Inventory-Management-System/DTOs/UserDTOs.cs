using Gym_Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record UserResponseDTO(int Id, string Name, string Email, DateOnly Birthdate,
    string Address, string ContactNumber, string Username, UserRole Role);

public record UserCreateRequestDTO([Required(ErrorMessage = "Name is required.")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters long.")] string Name,
    [Required(ErrorMessage = "Email is required.")]
        [MinLength(6, ErrorMessage = "Email must be at least 6 characters long.")] string Email,
    [Required(ErrorMessage = "Birthdate is required.")] DateOnly Birthdate,
    [Required(ErrorMessage = "Address is required.")]
        [MinLength(6, ErrorMessage = "Address must be at least 6 characters long.")] string Address,
    [Required(ErrorMessage = "Contact number is required.")]
        [MinLength(10, ErrorMessage = "Contact number must be at least 10 characters long.")] string ContactNumber,
    [Required(ErrorMessage = "Username is required.")]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long.")] string Username,
    [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")] string Password);

public record UserUpdateRequestDTO([MinLength(4, ErrorMessage = "Name must be at least 4 characters long.")] string Name,
        [MinLength(6, ErrorMessage = "Email must be at least 6 characters long.")] string Email,
        DateOnly Birthdate,
        [MinLength(6, ErrorMessage = "Address must be at least 6 characters long.")] string Address,
        [MinLength(10, ErrorMessage = "Contact number must be at least 10 characters long.")] string ContactNumber);

public record UpdatePasswordRequestDTO(
    [Required(ErrorMessage = "New password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    string NewPassword);