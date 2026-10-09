using Gym_Inventory_Management_System.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace Gym_Inventory_Management_System.Tests.Tests;

public class AuthAndRoleTests : IClassFixture<GymApiFactory>
{
    private readonly GymApiFactory _factory;

    public AuthAndRoleTests(GymApiFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("ibrahim")]
    [InlineData("sara")]
    [InlineData("omar")]
    public async Task Login_WithValidCredentials_ReturnsToken(string username)
    {
        var client = _factory.CreateClient();

        var token = await client.GetTokenAsync(username);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { Username = "ibrahim", Password = "wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithMissingPassword_Returns400()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { Username = "ibrahim" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("ibrahim", HttpStatusCode.OK)]       // Admin
    [InlineData("sara", HttpStatusCode.OK)]          // Manager
    [InlineData("omar", HttpStatusCode.Forbidden)]   // Staff
    public async Task GetAllUsers_DependsOnRole(string username, HttpStatusCode expected)
    {
        var client = await _factory.LoggedInAs(username);

        var response = await client.GetAsync("/api/users/all");

        Assert.Equal(expected, response.StatusCode);
    }
}