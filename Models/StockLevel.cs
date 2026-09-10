namespace Gym_Inventory_Management_System.Models;

public class StockLevel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int LocationId { get; set; }
    public int Quantity { get; set; }

    public required Item Item { get; set; }
    public required Location Location { get; set; }
}