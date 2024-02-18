using System.Security.Claims;
namespace LeviathanRipBlog.Web.Helpers;

public static class HttpContextHelpers
{
    public static string CurrentUserName(this IHttpContextAccessor context) => context.HttpContext?.User.Identity?.Name ?? "unknown";
    public static string CurrentUserName(this HttpContext context) {
        return context.User.Identity?.Name ?? "unknown";
    }
    
    public static string CurrentUserId(this IHttpContextAccessor context) => context.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
    public static string CurrentUserId(this HttpContext context) {
        return context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
    }
}