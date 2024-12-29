using System.Security.Claims;

namespace Dating.Shared;

public static class ClaimsPrincipleExtension
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirst(ClaimTypes.Name)!.Value;
        return username!;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return userId;
    }
}