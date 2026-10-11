using System.Security.Claims;

namespace Gym_Inventory_Management_System.Helpers;

public static class ClaimsPrincipalExtensions
{
    // Reads the logged-in user's id from the JWT. [Authorize] guarantees the claim exists.
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}