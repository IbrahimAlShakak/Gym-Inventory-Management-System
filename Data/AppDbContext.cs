using Gym_Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace Gym_Inventory_Management_System.Context;

public class AppDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Consumable> Consumables { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<StockLevel> StockLevels { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<StockTransfer> StockTransfers { get; set; }
    public DbSet<Item> Items { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>().UseTptMappingStrategy();

        modelBuilder.Entity<Person>().UseTptMappingStrategy();

        modelBuilder.Entity<StockMovement>()
       .Property(e => e.Type)
       .HasConversion(
           v => v.ToString(),
           v => (MovementType)Enum.Parse(typeof(MovementType), v));

        modelBuilder.Entity<User>()
       .Property(e => e.Role)
       .HasConversion(
           v => v.ToString(),
           v => (UserRole)Enum.Parse(typeof(UserRole), v));

        modelBuilder.Entity<Equipment>()
       .Property(e => e.MaintenanceStatus)
       .HasConversion(
           v => v.ToString(),
           v => (MaintenanceStatus)Enum.Parse(typeof(MaintenanceStatus), v));

        modelBuilder.Entity<Item>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(e => e.CategoryId);

        modelBuilder.Entity<StockLevel>()
            .HasOne(e => e.Item)
            .WithMany(c => c.StockLevels)
            .HasForeignKey(e => e.ItemId);

        modelBuilder.Entity<StockLevel>()
            .HasOne(e => e.Location)
            .WithMany(c => c.StockLevels)
           .HasForeignKey(e => e.LocationId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(e => e.Item)
            .WithMany(c => c.StockMovements)
           .HasForeignKey(e => e.ItemId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(e => e.Location)
            .WithMany(c => c.StockMovements)
           .HasForeignKey(e => e.LocationId);

        modelBuilder.Entity<StockMovement>()
            .HasOne(e => e.User)
            .WithMany(c => c.StockMovements)
           .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<StockTransfer>()
            .HasOne(e => e.User)
            .WithMany(c => c.StockTransfers)
           .HasForeignKey(e => e.UserId);
        modelBuilder.Entity<StockTransfer>()
            .HasOne(e => e.Item)
            .WithMany(c => c.StockTransfers)
           .HasForeignKey(e => e.ItemId);

        modelBuilder.Entity<StockTransfer>()
            .HasOne(t => t.FromLocation)
            .WithMany(l => l.OutgoingTransfers)
            .HasForeignKey(t => t.FromLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockTransfer>()
            .HasOne(t => t.ToLocation)
            .WithMany(l => l.IncomingTransfers)
            .HasForeignKey(t => t.ToLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

