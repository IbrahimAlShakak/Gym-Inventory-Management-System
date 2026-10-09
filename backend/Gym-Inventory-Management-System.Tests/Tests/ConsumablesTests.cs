using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests;

public class ConsumablesTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public ConsumablesTests(GymApiFactory factory)
    {
        _factory = factory;
    }

    private static async Task<int> CreateConsumableAsync(HttpClient client, int categoryId)
    {
        var response = await client.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId));
        response.EnsureSuccessStatusCode();
        return await TestData.ReadIdAsync(response);
    }

    [Theory]
    [InlineData(TestUsers.Admin)]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task GetAll_AnyRole_Returns200(string username)
    {
        var client = await _factory.LoggedInAs(username);

        var response = await client.GetAsync("/api/consumables");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Admin)]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task LowStock_AnyRole_Returns200(string username)
    {
        var client = await _factory.LoggedInAs(username);

        var response = await client.GetAsync("/api/consumables/low-stock");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Admin, HttpStatusCode.Created)]
    [InlineData(TestUsers.Manager, HttpStatusCode.Created)]
    [InlineData(TestUsers.Staff, HttpStatusCode.Forbidden)]
    public async Task Create_DependsOnRole(string username, HttpStatusCode expected)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var client = await _factory.LoggedInAs(username);

        var response = await client.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId));

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsSameData()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var sku = TestData.Unique("SKU");

        var created = await admin.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId, sku));
        var id = await TestData.ReadIdAsync(created);
        var fetched = await admin.GetAsync($"/api/consumables/{id}");

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal(sku, await TestData.ReadStringAsync(fetched, "sku"));
    }

    [Fact]
    public async Task Create_WithMissingCategory_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId: 99999));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateSku_Returns409()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var sku = TestData.Unique("SKU");
        await admin.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId, sku));

        var second = await admin.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId, sku));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Theory]
    [InlineData("abc", "Test Consumable", "Bottle")]       // SKU too short (min 6)
    [InlineData("SKU-123456", "ab", "Bottle")]             // name too short (min 3)
    [InlineData("SKU-123456", "Test Consumable", "ab")]    // unit too short (min 3)
    public async Task Create_WithInvalidData_Returns400(string sku, string name, string unit)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);

        var response = await admin.PostAsJsonAsync("/api/consumables", new
        {
            SKU = sku,
            Name = name,
            CategoryId = categoryId,
            Unit = unit,
            ReorderThreshold = 5,
            ExpiryDate = (string?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_KeepingOwnSku_Returns200()
    {
        var manager = await _factory.LoggedInAs(TestUsers.Manager);
        var categoryId = await TestData.CreateCategoryAsync(manager);
        var sku = TestData.Unique("SKU");
        var created = await manager.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId, sku));
        var id = await TestData.ReadIdAsync(created);

        // Same SKU, only the name changes.
        var response = await manager.PutAsJsonAsync($"/api/consumables/{id}",
            TestData.Consumable(categoryId, sku, name: "Renamed consumable"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Renamed consumable", await TestData.ReadStringAsync(response, "name"));
    }

    [Fact]
    public async Task Update_WithAnotherItemsSku_Returns409()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var first = await admin.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId));
        var second = await admin.PostAsJsonAsync("/api/consumables", TestData.Consumable(categoryId));
        var firstSku = await TestData.ReadStringAsync(first, "sku");
        var secondId = await TestData.ReadIdAsync(second);

        var response = await admin.PutAsJsonAsync($"/api/consumables/{secondId}",
            TestData.Consumable(categoryId, sku: firstSku));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Update_AsStaff_Returns403()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var staff = await _factory.LoggedInAs(TestUsers.Staff);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var id = await CreateConsumableAsync(admin, categoryId);

        var response = await staff.PutAsJsonAsync($"/api/consumables/{id}", TestData.Consumable(categoryId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);

        var response = await admin.PutAsJsonAsync("/api/consumables/99999", TestData.Consumable(categoryId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task Delete_NonAdmin_Returns403(string username)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var id = await CreateConsumableAsync(admin, categoryId);
        var client = await _factory.LoggedInAs(username);

        var response = await client.DeleteAsync($"/api/consumables/{id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_Returns204_AndItemIsGone()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var id = await CreateConsumableAsync(admin, categoryId);

        var delete = await admin.DeleteAsync($"/api/consumables/{id}");
        var fetch = await admin.GetAsync($"/api/consumables/{id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, fetch.StatusCode);
    }
}