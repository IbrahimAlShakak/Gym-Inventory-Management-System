using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gym_Inventory_Management_System.Tests;

public class RegistrationTests : IClassFixture<GymApiFactory>
{
    private readonly HttpClient _client;

    public RegistrationTests(GymApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // Builds a valid registration body. Each test passes its own username,
    // because all tests in this class share one database.
    private static object ValidBody(string username, string password = "Password123!") => new
    {
        Name = "Test User",
        Email = $"{username}@example.com",
        Birthdate = "2000-01-01",
        Address = "Test Street 1",
        ContactNumber = "0123456789",
        Username = username,
        Password = password
    };

    [Fact]
    public async Task Register_WithValidData_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", ValidBody("newuser1"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(string.IsNullOrWhiteSpace(json.RootElement.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Register_NewUser_IsAlwaysStaff()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", ValidBody("newuser2"));

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Staff", json.RootElement.GetProperty("role").GetString());
    }

    [Fact]
    public async Task Register_ThenLogin_Works()
    {
        await _client.PostAsJsonAsync("/api/auth/register", ValidBody("newuser3", "MyOwnPass1!"));

        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new { Username = "newuser3", Password = "MyOwnPass1!" });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Returns409()
    {
        await _client.PostAsJsonAsync("/api/auth/register", ValidBody("newuser4"));

        var second = await _client.PostAsJsonAsync("/api/auth/register", ValidBody("newuser4"));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Theory]
    [InlineData("abc", "Password123!")]   // username too short (min 4)
    [InlineData("validname", "12345")]    // password too short (min 6)
    [InlineData("", "Password123!")]      // username missing
    public async Task Register_WithInvalidData_Returns400(string username, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", ValidBody(username, password));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}