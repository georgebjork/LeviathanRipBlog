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
            ip_address = GetIpAddress(context),
            user_agent = context.Request.Headers["User-Agent"].FirstOrDefault(),
            username = usernameRetriever.Username
        };
        
        //await repository.Insert(requestLog);
        
        await next(context);
    }

    private string? GetIpAddress(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress;
        string? ipv4Address = null;

        if (ipAddress != null)
        {
            return ipAddress.AddressFamily switch
            {
                // If the address is IPv6 and is an IPv4-mapped IPv6 address, convert it to IPv4.
                System.Net.Sockets.AddressFamily.InterNetworkV6 when ipAddress.IsIPv4MappedToIPv6 => ipAddress
                    .MapToIPv4()
                    .ToString(),
                // Directly take the IPv4 address.
                System.Net.Sockets.AddressFamily.InterNetwork => ipAddress.ToString(), _ => null
            };
        }

        return ipv4Address;
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
