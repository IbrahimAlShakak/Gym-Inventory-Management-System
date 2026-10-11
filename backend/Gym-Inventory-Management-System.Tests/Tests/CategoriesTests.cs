using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests;

public class CategoriesTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public CategoriesTests(GymApiFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData(TestUsers.Admin)]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task GetAll_AnyRole_Returns200(string username)
    {
        var client = await _factory.LoggedInAs(username);

        var response = await client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Admin, HttpStatusCode.Created)]
    [InlineData(TestUsers.Manager, HttpStatusCode.Created)]
    [InlineData(TestUsers.Staff, HttpStatusCode.Forbidden)]
    public async Task Create_DependsOnRole(string username, HttpStatusCode expected)
    {
        var client = await _factory.LoggedInAs(username);

        var response = await client.PostAsJsonAsync("/api/categories", TestData.Category());

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsSameData()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var name = TestData.Unique("Category");

        var created = await admin.PostAsJsonAsync("/api/categories", TestData.Category(name));
        var id = await TestData.ReadIdAsync(created);
        var fetched = await admin.GetAsync($"/api/categories/{id}");

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal(name, await TestData.ReadStringAsync(fetched, "name"));
    }

    [Theory]
    [InlineData("ab", "Valid description")]   // name too short (min 3)
    [InlineData("", "Valid description")]     // name missing
    [InlineData("Valid name", "short")]       // description too short (min 6)
    public async Task Create_WithInvalidData_Returns400(string name, string description)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.PostAsJsonAsync("/api/categories",
            new { Name = name, Description = description });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync("/api/categories/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_AsManager_ChangesData()
    {
        var manager = await _factory.LoggedInAs(TestUsers.Manager);
        var id = await TestData.CreateCategoryAsync(manager);

        var response = await manager.PutAsJsonAsync($"/api/categories/{id}",
            new { Name = "Renamed category", Description = "New description" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Renamed category", await TestData.ReadStringAsync(response, "name"));
    }

    [Fact]
    public async Task Update_AsStaff_Returns403()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var staff = await _factory.LoggedInAs(TestUsers.Staff);
        var id = await TestData.CreateCategoryAsync(admin);

        var response = await staff.PutAsJsonAsync($"/api/categories/{id}",
            new { Name = "Renamed category", Description = "New description" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.PutAsJsonAsync("/api/categories/99999",
            new { Name = "Renamed category", Description = "New description" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task Delete_NonAdmin_Returns403(string username)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var client = await _factory.LoggedInAs(username);
        var id = await TestData.CreateCategoryAsync(admin);

        var response = await client.DeleteAsync($"/api/categories/{id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_Returns204_AndItemIsGone()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var id = await TestData.CreateCategoryAsync(admin);

        var delete = await admin.DeleteAsync($"/api/categories/{id}");
        var fetch = await admin.GetAsync($"/api/categories/{id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, fetch.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.DeleteAsync("/api/categories/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}