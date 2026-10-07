using System.Net;

namespace Gym_Inventory_Management_System.Tests;

public class SmokeTests : IClassFixture<GymApiFactory>
{
    private readonly HttpClient _client;

    public SmokeTests(GymApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/users/all");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

