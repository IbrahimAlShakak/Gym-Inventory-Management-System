namespace Gym_Inventory_Management_System.Models;

public abstract class Item
{
    public int Id { get; set; }
    public required string SKU { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public DateOnly CreatedAt{ get; set; } = DateOnly.FromDateTime(DateTime.Today);
}
