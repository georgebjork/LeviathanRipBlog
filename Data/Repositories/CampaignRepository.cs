using Dapper;
using LeviathanRipBlog.Data.Models;
namespace LeviathanRipBlog.Data.Repositories;

public interface ICampaignRepository : IBaseRepository {
    
    Task<List<campaign>> GetCampaigns();
    Task<List<campaign>> GetCampaignsByUserId(string ownerId);
    Task<campaign> GetCampaignById(long id);
}

public class CampaignRepository : BasePgRepository, ICampaignRepository {

    public ILogger<CampaignRepository> _logger { get; }
    
    public CampaignRepository(IConfiguration configuration,  ILogger<CampaignRepository> logger) : base(configuration, logger) {
        _logger = logger;
    }


    public async Task<List<campaign>> GetCampaigns() {
        var sql = @"SELECT * FROM campaign WHERE is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<campaign>(sql);
        return rv.ToList();
    }
    
    public async Task<List<campaign>> GetCampaignsByUserId(string ownerId) {
        var sql = @"SELECT * FROM campaign WHERE is_deleted = false AND owner_id = @userId";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryAsync<campaign>(sql, new { userId = ownerId });
        return rv.ToList();
    }
    
    public async Task<campaign> GetCampaignById(long id) {
        var sql = @"SELECT * FROM campaign WHERE is_deleted = false AND id = @id";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<campaign>(sql, new { id });
        return rv;
    }
}
