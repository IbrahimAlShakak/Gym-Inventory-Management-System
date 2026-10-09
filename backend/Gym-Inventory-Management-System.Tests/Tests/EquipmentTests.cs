using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests;

public class EquipmentTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public EquipmentTests(GymApiFactory factory)
    {
        _factory = factory;
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

        var response = await client.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId));

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingCategory_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId: 99999));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateSku_Returns409()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var sku = TestData.Unique("SKU");
        await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId, sku: sku));

        var second = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId, sku: sku));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateSerialNumber_Returns409()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var serial = TestData.Unique("SER");
        await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId, serial: serial));

        var second = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId, serial: serial));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Theory]
    [InlineData("abc", "Test Equipment", "SER-123456")]   // SKU too short (min 6)
    [InlineData("SKU-123456", "ab", "SER-123456")]        // name too short (min 3)
    [InlineData("SKU-123456", "Test Equipment", "123")]   // serial too short (min 6)
    public async Task Create_WithInvalidData_Returns400(string sku, string name, string serial)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);

        var response = await admin.PostAsJsonAsync("/api/equipment",
            TestData.Equipment(categoryId, sku, serial, name));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_KeepingOwnSku_Returns200()
    {
        var manager = await _factory.LoggedInAs(TestUsers.Manager);
        var categoryId = await TestData.CreateCategoryAsync(manager);
        var sku = TestData.Unique("SKU");
        var serial = TestData.Unique("SER");
        var created = await manager.PostAsJsonAsync("/api/equipment",
            TestData.Equipment(categoryId, sku, serial));
        var id = await TestData.ReadIdAsync(created);

        // Same SKU and serial, only the name changes.
        var response = await manager.PutAsJsonAsync($"/api/equipment/{id}",
            TestData.Equipment(categoryId, sku, serial, name: "Renamed equipment"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Renamed equipment", await TestData.ReadStringAsync(response, "name"));
    }

    [Fact]
    public async Task Update_WithAnotherItemsSku_Returns409()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var first = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId));
        var second = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId));
        var firstSku = await TestData.ReadStringAsync(first, "sku");
        var secondId = await TestData.ReadIdAsync(second);

        var response = await admin.PutAsJsonAsync($"/api/equipment/{secondId}",
            TestData.Equipment(categoryId, sku: firstSku));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task Delete_NonAdmin_Returns403(string username)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var created = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId));
        var id = await TestData.ReadIdAsync(created);
        var client = await _factory.LoggedInAs(username);

        var response = await client.DeleteAsync($"/api/equipment/{id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_Returns204_AndItemIsGone()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var categoryId = await TestData.CreateCategoryAsync(admin);
        var created = await admin.PostAsJsonAsync("/api/equipment", TestData.Equipment(categoryId));
        var id = await TestData.ReadIdAsync(created);

        var delete = await admin.DeleteAsync($"/api/equipment/{id}");
        var fetch = await admin.GetAsync($"/api/equipment/{id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, fetch.StatusCode);
    }
}