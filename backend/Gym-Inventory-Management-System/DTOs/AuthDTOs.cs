using Gym_Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record RegisterRequestDTO([Required(ErrorMessage = "Name is required.")]
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
[MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")] string Password
    );

public record LoginRequestDTO([Required(ErrorMessage = "Username is required.")] string Username,
    [Required(ErrorMessage = "Password is required.")] string Password);

public record AuthResponseDTO(string Token, DateTime ExpiresAt, string Username, UserRole Role);


