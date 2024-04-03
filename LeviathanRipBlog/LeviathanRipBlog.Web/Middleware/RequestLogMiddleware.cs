using Dapper.Contrib.Extensions;
using LeviathanRipBlog.Web.Data.Repositories;
using LeviathanRipBlog.Web.Services;

namespace LeviathanRipBlog.Web.Middleware;

public class RequestLogMiddleware(RequestDelegate next, IUsernameRetriever usernameRetriever, IRequestLogRepository repository)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestLog = new request_log
        {
            path = context.Request.Path,
            method = context.Request.Method,
            timestamp = DateTime.UtcNow,
            ip_address = context.Connection.RemoteIpAddress?.ToString(),
            user_agent = context.Request.Headers["User-Agent"].FirstOrDefault(),
            username = usernameRetriever.Username
        };
        
        await repository.Insert(requestLog);
        
        await next(context);
    }
}


[Table("request_log")]
public record request_log
{
    public int id { get; set; }
    public string path { get; set; }
    public string method { get; set; }
    public DateTime timestamp { get; set; }
    public string? ip_address { get; set; }
    public string? user_agent { get; set; }
    public string? username { get; set; }
}