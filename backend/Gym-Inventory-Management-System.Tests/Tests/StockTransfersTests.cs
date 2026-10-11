using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests;

public class StockTransfersTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public StockTransfersTests(GymApiFactory factory)
    {
        _factory = factory;
    }

    // Admin client, a user id, a fresh item and two fresh locations.
    private async Task<(HttpClient Admin, int UserId, int ItemId, int FromId, int ToId)> SetupAsync()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);
        var userId = await TestData.GetAnyUserIdAsync(admin);
        var itemId = await TestData.CreateConsumableAsync(admin);
        var fromId = await TestData.CreateLocationAsync(admin);
        var toId = await TestData.CreateLocationAsync(admin);
        return (admin, userId, itemId, fromId, toId);
    }

    [Theory]
    [InlineData("GET", "/api/StockTransfers/All-Stock-Transfers")]
    [InlineData("GET", "/api/StockTransfers/Stock-Transfer/1")]
    [InlineData("GET", "/api/StockTransfers/Stock-Transfers/Item/1")]
    [InlineData("GET", "/api/StockTransfers/Stock-Transfers/Locations/1/2")]
    [InlineData("POST", "/api/StockTransfers/Stock-Transfer")]
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
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);
        var client = await _factory.LoggedInAs(username);

        var response = await client.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 5, userId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_MovesStockBetweenLocations()
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);

        await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 4, userId));

        Assert.Equal(6, await TestData.GetQuantityAsync(admin, itemId, fromId));
        Assert.Equal(4, await TestData.GetQuantityAsync(admin, itemId, toId));
    }

    [Fact]
    public async Task Transfer_ExactlyAllStock_LeavesZeroAtSource()
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 5, userId);

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 5, userId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(0, await TestData.GetQuantityAsync(admin, itemId, fromId));
        Assert.Equal(5, await TestData.GetQuantityAsync(admin, itemId, toId));
    }

    [Fact]
    public async Task Transfer_MoreThanAvailable_Returns400_AndNothingChanges()
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 3, userId);

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 5, userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(3, await TestData.GetQuantityAsync(admin, itemId, fromId));
        Assert.Equal(0, await TestData.GetQuantityAsync(admin, itemId, toId));
    }

    [Fact]
    public async Task Transfer_FromLocationWithNoStock_Returns400()
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 1, userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_ToSameLocation_Returns400()
    {
        var (admin, userId, itemId, fromId, _) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, fromId, 5, userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(10, await TestData.GetQuantityAsync(admin, itemId, fromId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task Transfer_WithNonPositiveQuantity_Returns400(int quantity)
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, quantity, userId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_WithMissingItem_Returns404()
    {
        var (admin, userId, _, fromId, toId) = await SetupAsync();

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(99999, fromId, toId, 1, userId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_WithMissingDestination_Returns404()
    {
        var (admin, userId, itemId, fromId, _) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, 99999, 1, userId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_WithMissingUser_Returns404()
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);

        var response = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 1, userId: 99999));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsTheTransfer()
    {
        var (admin, userId, itemId, fromId, toId) = await SetupAsync();
        await TestData.ReceiveAsync(admin, itemId, fromId, 10, userId);
        var created = await admin.PostAsJsonAsync("/api/StockTransfers/Stock-Transfer",
            TestData.Transfer(itemId, fromId, toId, 6, userId));
        var id = await TestData.ReadIdAsync(created);

        var fetched = await admin.GetAsync($"/api/StockTransfers/Stock-Transfer/{id}");

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        Assert.Equal(6, await TestData.ReadIntAsync(fetched, "quantity"));
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync("/api/StockTransfers/Stock-Transfer/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        var staff = await _factory.LoggedInAs(TestUsers.Staff);

        var response = await staff.GetAsync("/api/StockTransfers/All-Stock-Transfers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByItem_WhenItemMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync("/api/StockTransfers/Stock-Transfers/Item/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByLocations_WhenLocationMissing_Returns404()
    {
        var admin = await _factory.LoggedInAs(TestUsers.Admin);

        var response = await admin.GetAsync("/api/StockTransfers/Stock-Transfers/Locations/99999/99998");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}