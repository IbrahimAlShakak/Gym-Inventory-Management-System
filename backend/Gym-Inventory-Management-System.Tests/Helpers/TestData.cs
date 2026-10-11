using System.Net.Http.Json;
using System.Text.Json;

namespace Gym_Inventory_Management_System.Tests;

public static class TestUsers
{
    public const string Admin = "ibrahim";
    public const string Manager = "sara";
    public const string Staff = "omar";
}

public static class TestData
{
    // Unique values so tests never collide on names/SKUs, even in a shared database.
    public static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid().ToString("N")[..8]}";

    public static object Category(string? name = null) => new
    {
        Name = name ?? Unique("Category"),
        Description = "Test description"
    };

    public static object Location(string? name = null) => new
    {
        Name = name ?? Unique("Location"),
        Address = "Test Street 1",
        ContactNumber = "0123456789"
    };

    public static object Equipment(int categoryId, string? sku = null, string? serial = null,
        string? name = null) => new
        {
            SKU = sku ?? Unique("SKU"),
            Name = name ?? "Test Equipment",
            CategoryId = categoryId,
            SerialNumber = serial ?? Unique("SER"),
            PurchaseDate = "2024-01-01",
            WarrantyExpiry = "2027-01-01",
            MaintenanceStatus = 0,   // enum sent as its number, so we don't need the enum names
            Condition = "Good"
        };

    public static object Consumable(int categoryId, string? sku = null, string? name = null) => new
    {
        SKU = sku ?? Unique("SKU"),
        Name = name ?? "Test Consumable",
        CategoryId = categoryId,
        Unit = "Bottle",
        ReorderThreshold = 5,
        ExpiryDate = (string?)null
    };

    public static object Movement(int itemId, int locationId, int quantity, string type,
     string notes = "Test notes") => new
     {
         ItemId = itemId,
         LocationId = locationId,
         Quantity = quantity,
         Type = type,                  // "Received", "Used", "Damaged" or "Adjusted"
         MovementDate = "2026-10-11",
         Notes = notes
     };

    public static object Transfer(int itemId, int fromLocationId, int toLocationId, int quantity) => new
    {
        ItemId = itemId,
        FromLocationId = fromLocationId,
        ToLocationId = toLocationId,
        Quantity = quantity,
        TransferDate = "2026-10-11T10:00:00"
    };

    // Reads the "id" field from a JSON response.
    public static async Task<int> ReadIdAsync(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("id").GetInt32();
    }

    public static async Task<string> ReadStringAsync(HttpResponseMessage response, string property)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty(property).GetString()!;
    }

    // Creates a category as the given client (must be Admin or Manager) and returns its id.
    public static async Task<int> CreateCategoryAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/categories", Category());
        response.EnsureSuccessStatusCode();
        return await ReadIdAsync(response);
    }

    public static async Task<int> CreateLocationAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/locations", Location());
        response.EnsureSuccessStatusCode();
        return await ReadIdAsync(response);
    }

    // Creates a fresh category plus a consumable in it, and returns the consumable's id.
    public static async Task<int> CreateConsumableAsync(HttpClient client)
    {
        var categoryId = await CreateCategoryAsync(client);
        var response = await client.PostAsJsonAsync("/api/consumables", Consumable(categoryId));
        response.EnsureSuccessStatusCode();
        return await ReadIdAsync(response);
    }


    public static async Task<int> GetAnyUserIdAsync(HttpClient admin)
    {
        var response = await admin.GetAsync("/api/users/all");
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement[0].GetProperty("id").GetInt32();
    }

    public static async Task<int> ReadIntAsync(HttpResponseMessage response, string property)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty(property).GetInt32();
    }

    // Puts stock into a location by recording a "Received" movement.
    public static async Task ReceiveAsync(HttpClient client, int itemId, int locationId, int quantity)
    {
        var response = await client.PostAsJsonAsync("/api/StockMovements",
            Movement(itemId, locationId, quantity, "Received"));
        response.EnsureSuccessStatusCode();
    }

    // Current quantity of an item at a location. 0 if no stock row exists yet.
    public static async Task<int> GetQuantityAsync(HttpClient client, int itemId, int locationId)
    {
        var response = await client.GetAsync($"/api/Stock-Levels/Item/{itemId}/Location/{locationId}");
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetArrayLength() == 0
            ? 0
            : json.RootElement[0].GetProperty("quantity").GetInt32();
    }
}