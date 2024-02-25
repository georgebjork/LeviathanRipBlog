using LeviathanRipBlog.Web.Data.Repositories;
namespace LeviathanRipBlog.Web.Services.Authorization;

public interface IRecordOwnershipService
{
    Task<bool> IsCampaignOwner(string userId, long campaignId);
    Task<bool> IsBlogOwner(string userId, long blogId);
}

public class RecordOwnershipService : IRecordOwnershipService
{
    private readonly ILogger<RecordOwnershipService> _logger;
    private readonly IRecordOwnerRepository _repository;
    
    // Memory Cache
    private Dictionary<long, string> campaignOwners = new();
    private Dictionary<long, string> blogOwners = new();

    public RecordOwnershipService(ILogger<RecordOwnershipService> logger, IRecordOwnerRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }


    public async Task<bool> IsCampaignOwner(string userId, long campaignId)
    {
        // Ensure the cache is initialized and not empty. If it is, or the specific campaignId is not found,
        // it attempts to refresh the cache.
        if (campaignOwners.Count == 0 || !campaignOwners.ContainsKey(campaignId))
        {
            campaignOwners = await _repository.GetCampaignOwners();
        }
    
        // After ensuring the cache is up-to-date, try to get the owner of the campaignId.
        // This eliminates the need for checking twice by making sure the cache has been refreshed if needed.
        if (campaignOwners.TryGetValue(campaignId, out var owner))
        {
            return owner == userId;
        }

        // If the campaignId is still not found in the cache after the update, return false.
        return false;
    }
    
    public async Task<bool> IsBlogOwner(string userId, long blogId) {
        // Ensure the cache is initialized and not empty. If it is, or the specific blogId is not found,
        // it attempts to refresh the cache.
        if (blogOwners.Count == 0 || !blogOwners.ContainsKey(blogId))
        {
            blogOwners = await _repository.GetBlogOwners();
        }
    
        // After ensuring the cache is up-to-date, try to get the owner of the blogId.
        // This eliminates the need for checking twice by making sure the cache has been refreshed if needed.
        if (blogOwners.TryGetValue(blogId, out var owner))
        {
            return owner == userId;
        }

        // If the campaignId is still not found in the cache after the update, return false.
        return false;
    }

}