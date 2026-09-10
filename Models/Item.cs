namespace Gym_Inventory_Management_System.Models;

public abstract class Item
{
    public int Id { get; set; }
    public required string SKU { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public DateOnly CreatedAt{ get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public required Category Category { get; set; }
    public List<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public List<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public List<StockTransfer> StockTransfers { get; set; } = new List<StockTransfer>();
}
