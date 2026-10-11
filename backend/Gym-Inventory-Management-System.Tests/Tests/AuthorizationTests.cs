using System.Net;

namespace Gym_Inventory_Management_System.Tests;

public class AuthorizationTests : IClassFixture<GymApiFactory>
{
    private readonly HttpClient _client;

    public AuthorizationTests(GymApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("GET", "/api/users/all")]
    [InlineData("GET", "/api/users/1")]
    [InlineData("POST", "/api/users")]
    [InlineData("PUT", "/api/users/update/1")]
    [InlineData("PUT", "/api/users/promote/1")]
    [InlineData("PUT", "/api/users/1/password")]
    [InlineData("DELETE", "/api/users/1")]
    public async Task UsersEndpoints_WithoutToken_Return401(string method, string url)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), url);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}