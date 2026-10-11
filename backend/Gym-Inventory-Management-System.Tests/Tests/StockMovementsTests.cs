using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests;

public class StockMovementsTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public StockMovementsTests(GymApiFactory factory)
    {
        _factory = factory;
    }

    // Admin client + a fresh item + a fresh location.
    private async Task<(HttpClient Admin, int ItemId, int LocationId)> SetupAsync()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var itemId = await TestData.CreateConsumableAsync(admin);
        var locationId = await TestData.CreateLocationAsync(admin);
        return (admin, itemId, locationId);
    }

    [Theory]
    [InlineData("GET", "/api/StockMovements/All-Stock-Movements")]
    [InlineData("GET", "/api/StockMovements/Stock-Movement/1")]
    [InlineData("POST", "/api/StockMovements")]
    [InlineData("GET", "/api/Stock-Levels/Item/1")]
    [InlineData("GET", "/api/Stock-Levels/Location/1")]
    [InlineData("GET", "/api/Stock-Levels/Item/1/Location/1")]
    public async Task Endpoints_WithoutToken_Return401(string method, string url)
    {
        var client = _factory.CreateClient();

        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(TestUsers.Admin)]
    [InlineData(TestUsers.Manager)]
    [InlineData(TestUsers.Staff)]
    public async Task Create_AnyRole_Returns201(string username)
    {
        var (_, itemId, locationId) = await SetupAsync();
        var client = await _factory.LoggedInAs(username);

        var response = await client.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 10, "Received"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_RecordsTheLoggedInUser_EvenIfBodyClaimsAnotherUser()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var staff = await _factory.LoggedInAs(TestUsers.Staff);
        var itemId = await TestData.CreateConsumableAsync(admin);
        var locationId = await TestData.CreateLocationAsync(admin);

        var byAdmin = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Received"));
        var adminUserId = await TestData.ReadIntAsync(byAdmin, "userId");

        // Staff tries to pretend to be the admin by sending the admin's id in the body.
        var byStaff = await staff.PostAsJsonAsync("/api/StockMovements", new
        {
            ItemId = itemId,
            LocationId = locationId,
            Quantity = 5,
            Type = "Received",
            MovementDate = "2026-10-11",
            Notes = "Test notes",
            UserId = adminUserId
        });

        Assert.Equal(HttpStatusCode.Created, byStaff.StatusCode);
        Assert.NotEqual(adminUserId, await TestData.ReadIntAsync(byStaff, "userId"));
    }

    [Fact]
    public async Task Received_IncreasesStock()
    {
        var (admin, itemId, locationId) = await SetupAsync();

        await TestData.ReceiveAsync(admin, itemId, locationId, 10);
        await TestData.ReceiveAsync(admin, itemId, locationId, 5);

        Assert.Equal(15, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Used_DecreasesStock()
    {
        var (admin, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 10);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 4, "Used"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(6, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Damaged_DecreasesStock()
    {
        var (admin, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 10);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 3, "Damaged"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(7, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Used_ExactlyAllStock_LeavesZero()
    {
        var (admin, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 5);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Used"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(0, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Used_MoreThanAvailable_Returns400_AndStockIsUnchanged()
    {
        var (admin, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 5);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 10, "Used"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(5, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Damaged_WhenNoStockExists_Returns400()
    {
        var (admin, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 1, "Damaged"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Create_WithNonPositiveQuantity_Returns400(int quantity)
    {
        var (admin, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, quantity, "Received"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithShortNotes_Returns400()
    {
        var (admin, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Received", notes: "abc"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithUnknownType_Returns400()
    {
        var (admin, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Banana"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingItem_Returns404()
    {
        var (admin, _, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(99999, locationId, 5, "Received"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingLocation_Returns404()
    {
        var (admin, itemId, _) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, 99999, 5, "Received"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsTheMovement()
    {
        var (admin, itemId, locationId) = await SetupAsync();
        var created = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 7, "Received"));
        var id = await TestData.ReadIdAsync(created);

        var fetched = await admin.GetAsync($"/api/StockMovements/Stock-Movement/{id}");

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal(7, await TestData.ReadIntAsync(fetched, "quantity"));
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync("/api/StockMovements/Stock-Movement/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        var staff = await _factory.LoggedInAs(TestUsers.Staff);

        var response = await staff.GetAsync("/api/StockMovements/All-Stock-Movements");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/Stock-Levels/Item/99999")]
    [InlineData("/api/Stock-Levels/Location/99999")]
    [InlineData("/api/Stock-Levels/Item/99999/Location/99999")]
    public async Task StockLevels_ForMissingIds_Return404(string url)
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}