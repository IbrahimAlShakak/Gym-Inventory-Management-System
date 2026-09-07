namespace Gym_Inventory_Management_System.Models;

public enum MovementType { Received, Used, Adjusted, Damaged }

public class StockMovement
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int LocationId { get; set; }
    public int Quantity { get; set; }
    public MovementType Type { get; set; }
    public DateOnly MovementDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int UserId { get; set; }
    public required string Notes { get; set; }
}
