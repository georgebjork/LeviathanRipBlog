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
        var sql = @"
            SELECT b.*, bd.document_identifier 
            FROM blog b
            INNER JOIN blog_documents bd ON bd.blog_id = b.id 
            WHERE b.campaign_id = @campaignId AND b.is_deleted = false";
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
        var sql = @"
            SELECT b.*, bd.document_identifier, c.name AS ""campaign_name""
            FROM blog b
            INNER JOIN blog_documents bd ON bd.blog_id = b.id 
            INNER JOIN campaign c ON (b.campaign_id = c.id)
            WHERE b.is_deleted = false ORDER BY b.id DESC LIMIT 5";
        
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<blog>(sql, new {num = numBlogs});
        return rv.ToList();
    }
}
