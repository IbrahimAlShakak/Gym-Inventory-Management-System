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

    // Admin client + a user id + a fresh item + a fresh location.
    private async Task<(HttpClient Admin, int UserId, int ItemId, int LocationId)> SetupAsync()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var userId = await TestData.GetAnyUserIdAsync(admin);
        var itemId = await TestData.CreateConsumableAsync(admin);
        var locationId = await TestData.CreateLocationAsync(admin);
        return (admin, userId, itemId, locationId);
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
        var (_, userId, itemId, locationId) = await SetupAsync();
        var client = await _factory.LoggedInAs(username);

        var response = await client.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 10, "Received", userId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Received_IncreasesStock()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();

        await TestData.ReceiveAsync(admin, itemId, locationId, 10, userId);
        await TestData.ReceiveAsync(admin, itemId, locationId, 5, userId);

        Assert.Equal(15, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Used_DecreasesStock()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 10, userId);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 4, "Used", userId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(6, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Damaged_DecreasesStock()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 10, userId);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 3, "Damaged", userId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(7, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Used_ExactlyAllStock_LeavesZero()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 5, userId);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Used", userId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(0, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Used_MoreThanAvailable_Returns400_AndStockIsUnchanged()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, locationId, 5, userId);

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 10, "Used", userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(5, await TestData.GetQuantityAsync(admin, itemId, locationId));
    }

    [Fact]
    public async Task Damaged_WhenNoStockExists_Returns400()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 1, "Damaged", userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Create_WithNonPositiveQuantity_Returns400(int quantity)
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, quantity, "Received", userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithShortNotes_Returns400()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Received", userId, notes: "abc"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithUnknownType_Returns400()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Banana", userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingItem_Returns404()
    {
        var (admin, userId, _, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(99999, locationId, 5, "Received", userId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingLocation_Returns404()
    {
        var (admin, userId, itemId, _) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, 99999, 5, "Received", userId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingUser_Returns404()
    {
        var (admin, _, itemId, locationId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 5, "Received", userId: 99999));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsTheMovement()
    {
        var (admin, userId, itemId, locationId) = await SetupAsync();
        var created = await admin.PostAsJsonAsync("/api/StockMovements",
            TestData.Movement(itemId, locationId, 7, "Received", userId));
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