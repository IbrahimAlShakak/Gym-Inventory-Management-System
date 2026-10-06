namespace Gym_Inventory_Management_System.Models;

public class Consumable : Item
{
    public required string Unit { get; set; }
    public int ReorderThreshold { get; set; }
    public DateOnly? ExpiryDate { get; set; }
}
