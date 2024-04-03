using LeviathanRipBlog.Web.Middleware;

namespace LeviathanRipBlog.Web.Models.RequestLog;

public class RequestLogViewModel
{
    public List<request_log>? Logs { get; set; } = [];
}