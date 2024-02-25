using LeviathanRipBlog.Web.Models.Campaign.QueryModels;
namespace LeviathanRipBlog.Web.Models.Campaign.ViewModels;

public class CampaignListViewModel {
    public List<CampaignQueryModel> Campaigns { get; set; } = new();
}
