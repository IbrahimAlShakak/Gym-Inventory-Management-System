namespace Gym_Inventory_Management_System.Models;

public enum UserRole { Admin, Staff }

public class User : Person
{
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; } 


}
