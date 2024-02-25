using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Data.Repositories;
using LeviathanRipBlog.Web.Helpers;
using LeviathanRipBlog.Web.Models.Campaign.FormModels;
using LeviathanRipBlog.Web.Models.Campaign.QueryModels;
namespace LeviathanRipBlog.Web.Services.Campaign;

public interface ICampaignService
{
    Task<List<CampaignQueryModel>> GetCampaigns();
    Task<List<CampaignQueryModel>> GetCampaignsByUserId(string ownerId);
    Task<CampaignQueryModel?> GetCampaignById(long id);
    
    Task<long> CreateCampaign(CampaignFormModel form);
    Task<bool> UpdateCampaign(CampaignFormModel form);
}


public class CampaignService : ICampaignService {
    
    private readonly ICampaignRepository campaignRepository;
    private readonly ILogger<CampaignService> _logger;
    private readonly IUsernameRetriever usernameRetriever;
    
    public CampaignService(ICampaignRepository campaign_repository, ILogger<CampaignService> logger, IUsernameRetriever username_retriever) {
        campaignRepository = campaign_repository;
        _logger = logger;
        usernameRetriever = username_retriever;
    }
    
    public async Task<List<CampaignQueryModel>> GetCampaigns() {
        var campaigns = await campaignRepository.GetCampaigns();
        return campaigns;
    }
    
    public async Task<List<CampaignQueryModel>> GetCampaignsByUserId(string ownerId) {
        var campaigns = await campaignRepository.GetCampaignsByUserId(ownerId);
        return campaigns;
    }
    
    public async Task<CampaignQueryModel?> GetCampaignById(long id) {
        var campaign = await campaignRepository.GetCampaignById(id);
        
        if(campaign is not null) _logger.LogInformation("User {Username} retrieved campaign {CampaignId}", usernameRetriever.Username, id);
        else _logger.LogWarning("User {Username} attempted to retrieve campaign {CampaignId} but it was not found", usernameRetriever.Username, id);
        
        return campaign;
    }
    public async Task<long> CreateCampaign(CampaignFormModel form) {
        
        var userId = usernameRetriever.UserId;
        var username = usernameRetriever.Username;
        var date = DateTime.UtcNow;

        var record = new campaign {
            name = form.Name,
            description = form.Description,
            owner_id = userId,
            is_deleted = false,
            created_by = username,
            created_on = date,
            updated_by = username,
            updated_on = date,
        };

        var rv = await campaignRepository.Insert(record);
        return rv;
    }
    public async Task<bool> UpdateCampaign(CampaignFormModel form) {
        var campaign = await campaignRepository.GetCampaignById((long)form.CampaignId!);
        
        if(campaign is null) {
            _logger.LogWarning("User {Username} attempted to update campaign {CampaignId} but it was not found", usernameRetriever.Username, form.CampaignId);
            return false;
        }

        campaign.is_deleted = form.IsDeleted;
        campaign.name = form.Name;
        campaign.description = form.Description;
        campaign.updated_by = usernameRetriever.Username;
        campaign.updated_on = DateTime.UtcNow;
        
        var rv = await campaignRepository.Update(campaign.ToCampaign());
        return rv;
    }
}
