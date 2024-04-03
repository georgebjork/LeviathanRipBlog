using Dapper;
using LeviathanRipBlog.Web.Middleware;

namespace LeviathanRipBlog.Web.Data.Repositories;

public interface IRequestLogRepository : IBaseRepository
{
    public Task<List<request_log>> GetLogs();
}

public class RequestLogRepository(IConfiguration configuration, ILogger<RequestLogRepository> logger) : BasePgRepository(configuration, logger), IRequestLogRepository
{
    public async Task<List<request_log>> GetLogs()
    {
        var sql = "SELECT * FROM request_log ORDER BY request_log.id DESC LIMIT 1000";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<request_log>(sql);
        return rv.ToList();
    }
}