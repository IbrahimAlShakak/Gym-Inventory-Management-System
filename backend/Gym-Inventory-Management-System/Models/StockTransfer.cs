namespace Gym_Inventory_Management_System.Models;

public class StockTransfer
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int FromLocationId { get; set; }
    public int ToLocationId { get; set; }
    public int Quantity { get; set; }
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }

    public User User { get; set; }  
    public Item Item { get; set; }

    public Location FromLocation { get; set; }
    public Location ToLocation { get; set; }
}