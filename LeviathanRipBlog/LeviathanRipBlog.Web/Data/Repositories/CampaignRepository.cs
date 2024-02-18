using Dapper;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Models.Campaign.QueryModels;
namespace LeviathanRipBlog.Web.Data.Repositories;

public interface ICampaignRepository : IBaseRepository {
    
    Task<List<CampaignQueryModel>> GetCampaigns();
    Task<List<CampaignQueryModel>> GetCampaignsByUserId(string ownerId);
    Task<CampaignQueryModel?> GetCampaignById(long id);
}

public class CampaignRepository : BasePgRepository, ICampaignRepository {

    public ILogger<CampaignRepository> _logger { get; }
    
    public CampaignRepository(IConfiguration configuration,  ILogger<CampaignRepository> logger) : base(configuration, logger) {
        _logger = logger;
    }


    public async Task<List<CampaignQueryModel>> GetCampaigns() {
        var sql = @"SELECT * FROM campaign WHERE is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<CampaignQueryModel>(sql);
        return rv.ToList();
    }
    
    public async Task<List<CampaignQueryModel>> GetCampaignsByUserId(string ownerId) {
        var sql = @"SELECT * FROM campaign WHERE is_deleted = false AND owner_id = @userId";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<CampaignQueryModel>(sql, new { userId = ownerId });
        return rv.ToList();
    }
    
    public async Task<CampaignQueryModel?> GetCampaignById(long id) {
        var sql = @"SELECT * FROM campaign WHERE is_deleted = false AND id = @id";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<CampaignQueryModel>(sql, new { id });
        return rv;
    }
}
