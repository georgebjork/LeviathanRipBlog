
using System.ComponentModel.DataAnnotations;
using LeviathanRipBlog.Services;
namespace LeviathanRipBlog.Components.Pages.Campaign.Models;

public class CampaignFormModel {

    [Required]
    [MaxLength(100, ErrorMessage = "Maximum length is 100 characters")]
    [Display(Name = "Campaign Name")]
    public string Name { get; set; } = "";
    
    [Required]
    public string Description { get; set; } = "";
    
    public string Username { get; set; } = "";
    public string OwnerId { get; set; } = "";
    
    
    public Data.Models.campaign ToNewCampaign(CampaignFormModel form) {
        return new Data.Models.campaign {
            name = Name,
            description = Description,
            owner_id = OwnerId,
            created_by = form.Username,
            created_on = DateTime.UtcNow,
            updated_by = form.Username,
            updated_on = DateTime.UtcNow,
            is_deleted = false
        };
    }
    
    public Data.Models.campaign ToUpdateCampaign(CampaignFormModel form, Data.Models.campaign campaign) {
        campaign.name = Name;
        campaign.description = Description;
        campaign.updated_by = form.Username;
        campaign.updated_on = DateTime.UtcNow;
        return campaign;
    }
    
    public CampaignFormModel ToFormModel(Data.Models.campaign campaign) {
        return new CampaignFormModel {
            Name = campaign.name,
            Description = campaign.description,
            Username = Username,
            OwnerId = OwnerId
        };
    }
}
