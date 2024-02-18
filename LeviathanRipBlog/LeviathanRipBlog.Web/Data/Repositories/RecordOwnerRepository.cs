using Dapper;
using LeviathanRipBlog.Web.Data.Models;
namespace LeviathanRipBlog.Web.Data.Repositories;

public interface IRecordOwnerRepository : IBaseRepository
{
    Task<Dictionary<long, string>> GetCampaignOwners();
    Task<Dictionary<long, string>> GetBlogOwners();
}


public class RecordOwnerRepository : BasePgRepository, IRecordOwnerRepository
{
    public RecordOwnerRepository(IConfiguration configuration, ILogger<RecordOwnerRepository> logger) : base(configuration, logger)
    {
    }

    public async Task<Dictionary<long, string>> GetCampaignOwners()
    {
        var sql = "SELECT id, owner_id FROM public.campaign WHERE is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var campaignOwners = await conn.QueryAsync<campaign>(sql);
        return campaignOwners.ToDictionary(x => x.id, x => x.owner_id);
    }
    public async Task<Dictionary<long, string>> GetBlogOwners() {
        var sql = "SELECT id, owner_id FROM public.blog WHERE is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var campaignOwners = await conn.QueryAsync<campaign>(sql);
        return campaignOwners.ToDictionary(x => x.id, x => x.owner_id);
    }
}