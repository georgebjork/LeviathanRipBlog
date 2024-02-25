using LeviathanRipBlog.Web.Models.Campaign.QueryModels;
using LeviathanRipBlog.Web.Models.QueryModels.Blog;
namespace LeviathanRipBlog.Web.Models.Campaign.ViewModels;

public class CampaignViewModel {
    
    public CampaignQueryModel Campaign { get; set; } = new();
    public List<BlogQueryModel> Blogs { get; set; } = new();
}
