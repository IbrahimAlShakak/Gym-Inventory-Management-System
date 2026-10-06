using Gym_Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Gym_Inventory_Management_System.DTOs;

public record EquipmentResponseDTO
    (int Id, string SKU, string Name, int CategoryId, string SerialNumber, 
    DateOnly PurchaseDate, DateOnly WarrantyExpiry, MaintenanceStatus MaintenanceStatus, string Condition);


public record EquipmentCreateRequestDTO
 ([Required(ErrorMessage = "Stock Keeping Unit is required.")]
        [MinLength(6, ErrorMessage = "Stock Keeping Unit must be at least 6 characters long.")]
    string SKU,
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    [Required(ErrorMessage = "Category Id is required.")]
    int CategoryId,
    [Required(ErrorMessage = "Serial Number is required.")]
        [MinLength(6, ErrorMessage = "Serial Number must be at least 6 characters long.")]
    string SerialNumber,
    [Required(ErrorMessage = "Purchase Date is required.")]
    DateOnly PurchaseDate,
    [Required(ErrorMessage = "Warranty Expiry is required.")]
    DateOnly WarrantyExpiry,
    [Required(ErrorMessage = "Maintenance Status is required.")]
    MaintenanceStatus MaintenanceStatus,
    [Required(ErrorMessage = "Condition is required.")]
        [MinLength(3, ErrorMessage = "Condition must be at least 3 characters long.")]
    string Condition
);

public record EquipmentUpdateRequestDTO
 ([Required(ErrorMessage = "Stock Keeping Unit is required.")]
        [MinLength(6, ErrorMessage = "Stock Keeping Unit must be at least 6 characters long.")]
    string SKU,
    [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    string Name,
    [Required(ErrorMessage = "Category Id is required.")]
    int CategoryId,
    [Required(ErrorMessage = "Serial Number is required.")]
        [MinLength(6, ErrorMessage = "Serial Number must be at least 6 characters long.")]
    string SerialNumber,
    [Required(ErrorMessage = "Purchase Date is required.")]
    DateOnly PurchaseDate,
    [Required(ErrorMessage = "Warranty Expiry is required.")]
    DateOnly WarrantyExpiry,
    [Required(ErrorMessage = "Maintenance Status is required.")]
    MaintenanceStatus MaintenanceStatus,
    [Required(ErrorMessage = "Condition is required.")]
        [MinLength(3, ErrorMessage = "Condition must be at least 3 characters long.")]
    string Condition
);