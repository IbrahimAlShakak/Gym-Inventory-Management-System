namespace Gym_Inventory_Management_System.Models;

public enum MaintenanceStatus { NoNeed, NeedsMaintenance, Scheduled, InProgress }

public class Equipment : Item
{
    public required string SerialNumber { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public DateOnly WarrantyExpiry { get; set; }
    public MaintenanceStatus MaintenanceStatus { get; set; }
    public required string Condition { get; set; }

    
}
