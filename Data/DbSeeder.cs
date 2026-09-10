using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Categories.Any())
        {
            return;
        }

        // 1. Categories (10)
        var categoryNames = new[]
        {
            "Supplements", "Cardio Equipment", "Strength Equipment",
            "Free Weights", "Cleaning Supplies", "Apparel",
            "Recovery Tools", "Hydration", "Accessories", "Safety Equipment"
        };
        var categories = categoryNames
            .Select(name => new Category { Name = name, Description = $"{name} category" })
            .ToList();
        context.Categories.AddRange(categories);

        // 2. Locations (10)
        var locationNames = new[]
        {
            "Main Gym", "Downtown Branch", "North Branch", "South Branch",
            "East Branch", "West Branch", "Airport Branch", "Mall Branch",
            "University Branch", "Central Storage"
        };
        var locations = locationNames
            .Select((name, i) => new Location
            {
                Name = name,
                Address = $"{100 + i} {name} St, Amman",
                ContactNumber = $"06-{1000000 + i}"
            })
            .ToList();
        context.Locations.AddRange(locations);

        // 3. Consumables (10)
        var consumableNames = new[]
        {
            "Whey Protein Powder", "Creatine Monohydrate", "BCAA Powder",
            "Water Bottles", "Gym Towels", "Resistance Bands",
            "Chalk Blocks", "Cleaning Spray", "Disinfectant Wipes", "Energy Bars"
        };
        var consumables = consumableNames
            .Select((name, i) => new Consumable
            {
                SKU = $"SUP-{i + 1:000}",
                Name = name,
                Category = categories[i % categories.Count],
                Unit = i % 2 == 0 ? "kg" : "pieces",
                ReorderThreshold = 5 + i,
                ExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(6 + i))
            })
            .ToList();
        context.Consumables.AddRange(consumables);

        // 4. Equipment (10)
        var equipmentNames = new[]
        {
            "Treadmill Pro 2000", "Elliptical Trainer", "Rowing Machine",
            "Leg Press Machine", "Cable Crossover", "Smith Machine",
            "Bench Press Rack", "Spin Bike", "Lat Pulldown Machine", "Squat Rack"
        };
        var equipmentList = equipmentNames
            .Select((name, i) => new Equipment
            {
                SKU = $"EQP-{i + 1:000}",
                Name = name,
                Category = categories[i % categories.Count],
                SerialNumber = $"SN-{i + 1:0000}",
                PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-(i + 1))),
                WarrantyExpiry = DateOnly.FromDateTime(DateTime.Today.AddYears(2)),
                MaintenanceStatus = (MaintenanceStatus)(i % 4),
                Condition = i % 3 == 0 ? "Good" : "Excellent"
            })
            .ToList();
        context.Equipment.AddRange(equipmentList);

        // 5. Users (10)
        var userNames = new[]
        {
            "Ibrahim Admin", "Sara Manager", "Omar Staff", "Layla Staff",
            "Yousef Staff", "Nour Staff", "Ahmad Staff", "Rana Staff",
            "Khaled Staff", "Dana Staff"
        };
        var users = userNames
            .Select((name, i) => new User
            {
                Name = name,
                Email = $"user{i + 1}@gym.local",
                Birthdate = new DateOnly(1990 + i, 1, 1),
                Address = "N/A",
                ContactNumber = $"07-{9000000 + i}",
                Username = name.Split(' ')[0].ToLower(),
                PasswordHash = "placeholder-hash",
                Role = i == 0 ? UserRole.Admin : UserRole.Staff
            })
            .ToList();
        context.Users.AddRange(users);

        // Save so everything above gets real Ids before we reference them
        context.SaveChanges();

        // 6. Stock levels — every consumable gets a level at 2 different locations (20 rows)
        var stockLevels = new List<StockLevel>();
        for (int i = 0; i < consumables.Count; i++)
        {
            stockLevels.Add(new StockLevel
            {
                Item = consumables[i],
                Location = locations[i % locations.Count],
                Quantity = 10 + i * 3
            });
            stockLevels.Add(new StockLevel
            {
                Item = consumables[i],
                Location = locations[(i + 1) % locations.Count],
                Quantity = 5 + i * 2
            });
        }
        context.StockLevels.AddRange(stockLevels);

        // 7. Stock movements (10) — mix of movement types across consumables
        var movementTypes = Enum.GetValues<MovementType>();
        var stockMovements = new List<StockMovement>();
        for (int i = 0; i < 10; i++)
        {
            stockMovements.Add(new StockMovement
            {
                Item = consumables[i % consumables.Count],
                Location = locations[i % locations.Count],
                Quantity = 5 + i,
                Type = movementTypes[i % movementTypes.Length],
                User = users[i % users.Count],
                Notes = $"Seeded movement #{i + 1}"
            });
        }
        context.StockMovements.AddRange(stockMovements);

        // 8. Stock transfers (10) — between two different locations each time
        var stockTransfers = new List<StockTransfer>();
        for (int i = 0; i < 10; i++)
        {
            var fromLocation = locations[i % locations.Count];
            var toLocation = locations[(i + 1) % locations.Count];

            stockTransfers.Add(new StockTransfer
            {
                Item = consumables[i % consumables.Count],
                FromLocation = fromLocation,
                ToLocation = toLocation,
                Quantity = 2 + i,
                User = users[i % users.Count]
            });
        }
        context.StockTransfers.AddRange(stockTransfers);

        context.SaveChanges();
    }
}