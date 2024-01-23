using Dapper;
using LeviathanRipBlog.Data.Models;
namespace LeviathanRipBlog.Data.Repositories;

public interface IBlogRepository : IBaseRepository{
    
    Task<List<blog>> GetCampaignBlogs(long campaignId);
    Task<blog> GetBlog(long blogId);
    Task<List<blog>> GetRecentBlogs(int numBlogs);
}

public class BlogRepository : BasePgRepository, IBlogRepository {

    public ILogger<BlogRepository> _logger { get; }
    
    public BlogRepository(IConfiguration configuration, ILogger<BlogRepository> logger) : base(configuration, logger) {
        _logger = logger;
    }


    public async Task<List<blog>> GetCampaignBlogs(long campaignId) {
        var sql = @"SELECT * FROM blog WHERE campaign_id = @campaignId AND is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<blog>(sql, new {campaignId});
        return rv.ToList();
    }
    public async Task<blog> GetBlog(long blogId) {
        var sql = @"SELECT * FROM blog WHERE id = @blogId AND is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<blog>(sql, new {blogId});
        return rv;
    }

    public async Task<List<blog>> GetRecentBlogs(int numBlogs)
    {
        var sql = @"SELECT blog.*, campaign.name AS ""campaign_name""
                    FROM blog
                    INNER JOIN campaign ON (blog.campaign_id = campaign.id)
                    WHERE blog.is_deleted = false ORDER BY blog.id DESC LIMIT 5";
        
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<blog>(sql, new {num = numBlogs});
        return rv.ToList();
    }
}
