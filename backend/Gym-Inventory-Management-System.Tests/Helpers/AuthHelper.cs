using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gym_Inventory_Management_System.Tests.Helpers;

public static class AuthHelper
{
    public const string Password = "Password123!";

    // Logs in and returns the JWT token string.
    public static async Task<string> GetTokenAsync(this HttpClient client, string username)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { Username = username, Password = Password });

        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("token").GetString()!;
    }

    // Returns a NEW client that already sends the token with every request.
    public static async Task<HttpClient> LoggedInAs(this GymApiFactory factory, string username)
    {
        var client = factory.CreateClient();
        var token = await client.GetTokenAsync(username);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}