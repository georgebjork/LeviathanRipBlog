using Dapper;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Data.Repositories;
using LeviathanRipBlog.Web.Models.QueryModels.Blog;
namespace LeviathanRipBlog.Data.Repositories;

public interface IBlogRepository : IBaseRepository{
    
    Task<List<BlogQueryModel>> GetCampaignBlogs(long campaignId);
    Task<BlogQueryModel?> GetBlogById(long blogId);
    Task<List<BlogQueryModel>> GetRecentBlogs(int numBlogs);
}

public class BlogRepository : BasePgRepository, IBlogRepository {

    public ILogger<BlogRepository> _logger { get; }
    
    public BlogRepository(IConfiguration configuration, ILogger<BlogRepository> logger) : base(configuration, logger) {
        _logger = logger;
    }


    public async Task<List<BlogQueryModel>> GetCampaignBlogs(long campaignId) {
        var sql = @"
            SELECT b.*, bd.document_identifier, c.name AS ""campaign_name""
            FROM blog b
            INNER JOIN blog_documents bd ON bd.blog_id = b.id 
            INNER JOIN campaign c ON (b.campaign_id = c.id)
            WHERE b.is_deleted = false AND b.campaign_id = @campaignId";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<BlogQueryModel>(sql, new {campaignId});
        return rv.ToList();
    }
    public async Task<BlogQueryModel?> GetBlogById(long blogId) {
        var sql = @"SELECT b.*, bd.document_identifier, bd.document_name, c.name AS ""campaign_name""
            FROM blog b
            INNER JOIN blog_documents bd ON bd.blog_id = b.id 
            INNER JOIN campaign c ON (b.campaign_id = c.id)
            WHERE b.is_deleted = false AND b.id = @blogId";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<BlogQueryModel>(sql, new {blogId});
        return rv;
    }

    public async Task<List<BlogQueryModel>> GetRecentBlogs(int numBlogs)
    {
        var sql = @"
            SELECT b.*, bd.document_identifier, c.name AS ""campaign_name""
            FROM blog b
            INNER JOIN blog_documents bd ON bd.blog_id = b.id 
            INNER JOIN campaign c ON (b.campaign_id = c.id)
            WHERE b.is_deleted = false ORDER BY b.id DESC LIMIT 5";
        
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<BlogQueryModel>(sql, new {num = numBlogs});
        return rv.ToList();
    }
}
