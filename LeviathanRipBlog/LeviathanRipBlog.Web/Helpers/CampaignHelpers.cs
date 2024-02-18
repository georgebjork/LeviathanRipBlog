using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Models.Campaign.QueryModels;
namespace LeviathanRipBlog.Web.Helpers;

public static class CampaignHelpers {
    
    public static campaign ToCampaign(this CampaignQueryModel model) {
        return new campaign {
            id = model.id,
            name = model.name,
            description = model.description,
            owner_id = model.owner_id,
            is_deleted = model.is_deleted,
            created_by = model.created_by,
            created_on = model.created_on,
            updated_by = model.updated_by,
            updated_on = model.updated_on
        };
    }
}
