using LeviathanRipBlog.Services;

namespace LeviathanRipBlog.API;

using Microsoft.AspNetCore.Builder;

public static class ApiEndpoints
{
    
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hello", () =>
        {
            return Results.Ok("Hello, World!");   
        }).CacheOutput();
        
        
        app.MapGet("/api/img/{imgId}", async (string imgId, IDocumentStorage documentStorage, HttpContext httpContext) =>
        {
            var (imageData, mimeType) = await documentStorage.RetrieveDocument(imgId);
            if (imageData.Length == 0 || string.IsNullOrWhiteSpace(mimeType))
            {
                return Results.NotFound();
            }

            // Set the cache-control header
            httpContext.Response.Headers.CacheControl = "public,max-age=43200"; // 12 hours

            return Results.File(imageData, mimeType);
        })
        .CacheOutput(options => options.Expire(TimeSpan.FromHours(12)));
    }
}
