using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Models.QueryModels.Blog;
namespace LeviathanRipBlog.Web.Helpers;

public static class BlogHelpers {
    
    public static blog ToBlog(this BlogQueryModel model) {
        return new blog {
            id = model.id,
            campaign_id = model.campaign_id,
            title = model.title,
            blog_content = model.blog_content,
            publish_date = model.publish_date,
            session_date = model.session_date,
            is_draft = model.is_draft,
            owner_id = model.owner_id,
            is_deleted = model.is_deleted,
            created_by = model.created_by,
            created_on = model.created_on,
            updated_by = model.updated_by,
            updated_on = model.updated_on
        };
    }
}
