namespace Gym_Inventory_Management_System.Models;

public abstract class Person
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateOnly Birthdate { get; set; }
    public required string Address { get; set; }
    public required string ContactNumber { get; set; }


}
