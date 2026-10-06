namespace Gym_Inventory_Management_System.Models;

public class Location
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string ContactNumber { get; set; }
    public List<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public List<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public List<StockTransfer> OutgoingTransfers { get; set; } = new List<StockTransfer>();
    public List<StockTransfer> IncomingTransfers { get; set; } = new List<StockTransfer>();


}
