using Microsoft.AspNetCore.Components.Authorization;
namespace LeviathanRipBlog.Helpers;

public static class AuthenticationStateHelper
{
    public static async Task<string> CurrentUserName(this AuthenticationStateProvider context) {
        var state = await context.GetAuthenticationStateAsync();
        return state.User.Identity?.Name ?? "Unknown";
    }
    
    public static async Task<string?> CurrentIdentity(this AuthenticationStateProvider context) {
        var state = await context.GetAuthenticationStateAsync();
        var id = state.User.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
        return id;
    }
}
