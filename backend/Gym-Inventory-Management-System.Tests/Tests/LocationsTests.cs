using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests;

public class LocationsTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public LocationsTests(GymApiFactory factory)
    {
        _factory = factory;
    }

    private static async Task<int> CreateLocationAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/locations", TestData.Location());
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

        var response = await client.GetAsync("/api/locations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Admin, HttpStatusCode.Created)]
    [InlineData(TestUsers.Manager, HttpStatusCode.Created)]
    [InlineData(TestUsers.Staff, HttpStatusCode.Forbidden)]
    public async Task Create_DependsOnRole(string username, HttpStatusCode expected)
    {
        var client = await _factory.LoggedInAs(username);

        var response = await client.PostAsJsonAsync("/api/locations", TestData.Location());

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsSameData()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var name = TestData.Unique("Location");

        var created = await admin.PostAsJsonAsync("/api/locations", TestData.Location(name));
        var id = await TestData.ReadIdAsync(created);
        var fetched = await admin.GetAsync($"/api/locations/{id}");

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal(name, await TestData.ReadStringAsync(fetched, "name"));
    }

    [Theory]
    [InlineData("ab", "Test Street 1", "0123456789")]            // name too short (min 3)
    [InlineData("Valid name", "short", "0123456789")]            // address too short (min 6)
    [InlineData("Valid name", "Test Street 1", "not-a-phone")]   // [Phone] rejects this
    public async Task Create_WithInvalidData_Returns400(string name, string address, string phone)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.PostAsJsonAsync("/api/locations",
            new { Name = name, Address = address, ContactNumber = phone });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync("/api/locations/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_AsManager_ChangesData()
    {
        var manager = await _factory.LoggedInAs(TestUsers.Manager);
        var id = await CreateLocationAsync(manager);

        var response = await manager.PutAsJsonAsync($"/api/locations/{id}",
            new { Name = "Renamed location", Address = "New Street 22", ContactNumber = "0987654321" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Renamed location", await TestData.ReadStringAsync(response, "name"));
    }

    [Fact]
    public async Task Update_AsStaff_Returns403()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var staff = await _factory.LoggedInAs(TestUsers.Staff);
        var id = await CreateLocationAsync(admin);

        var response = await staff.PutAsJsonAsync($"/api/locations/{id}",
            new { Name = "Renamed location", Address = "New Street 22", ContactNumber = "0987654321" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.PutAsJsonAsync("/api/locations/99999",
            new { Name = "Renamed location", Address = "New Street 22", ContactNumber = "0987654321" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task Delete_NonAdmin_Returns403(string username)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var client = await _factory.LoggedInAs(username);
        var id = await CreateLocationAsync(admin);

        var response = await client.DeleteAsync($"/api/locations/{id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_Returns204_AndItemIsGone()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var id = await CreateLocationAsync(admin);

        var delete = await admin.DeleteAsync($"/api/locations/{id}");
        var fetch = await admin.GetAsync($"/api/locations/{id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, fetch.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.DeleteAsync("/api/locations/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}