namespace LeviathanRipBlog.Middleware;


public class BlockIdentityPathMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/Account", StringComparison.OrdinalIgnoreCase) 
            && !context.Request.Path.StartsWithSegments("/Account/Logout", StringComparison.OrdinalIgnoreCase)
                && !context.Request.Path.StartsWithSegments("/Account/Manage", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 404;
            return;
        }

        await next(context);
    }
}
