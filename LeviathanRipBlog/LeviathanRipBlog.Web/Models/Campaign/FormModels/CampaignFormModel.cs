using System.ComponentModel.DataAnnotations;
namespace LeviathanRipBlog.Web.Models.Campaign.FormModels;

public class CampaignFormModel {
    
    public long? CampaignId { get; set; }
    public bool IsDeleted { get; set; }
    
    [Required]
    [MaxLength(100, ErrorMessage = "Maximum length is 100 characters")]
    [Display(Name = "Campaign Name")]
    public string Name { get; set; } = "";
    
    [Required]
    [Display(Name = "Campaign Description")]
    public string Description { get; set; } = "";
}
