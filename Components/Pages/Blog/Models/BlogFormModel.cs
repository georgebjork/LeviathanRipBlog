using System.ComponentModel.DataAnnotations;
using LeviathanRipBlog.Data.Models;
namespace LeviathanRipBlog.Components.Pages.Blog.Models;

public class BlogFormModel {
    
    [Required]
    [MaxLength(100, ErrorMessage = "Maximum length is 100 characters")]
    public string Title { get; set; } = "";
    
    [Required]
    public string Content { get; set; } = "";
    
    [Required]
    public DateTime? SessionDate { get; set; }
    
    public IFormFile File { get; set; }
    
    [Required] 
    private DateTime PublishDate { get; set; } = DateTime.UtcNow;
    
    

    private blog Blog { get; set; } = new(); 
    public string Username { get; set; } = "";
    public string UserId { get; set; } = "";
    public long CampaignId { get; set; }
    
    
    public Data.Models.blog ToNewBlog(BlogFormModel form) {
        return new Data.Models.blog {
            title = Title,
            blog_content = Content,
            publish_date = PublishDate,
            session_date = SessionDate!.Value,
            is_draft = false,
            is_deleted = false,
            created_by = Username,
            created_on = DateTime.UtcNow,
            updated_by = Username,
            updated_on = DateTime.UtcNow,
            campaign_id = CampaignId
        };
    }
    
    public BlogFormModel ToFormModel(Data.Models.blog blog) {
        return new BlogFormModel {
            Title = blog.title,
            Content = blog.blog_content,
            PublishDate = blog.publish_date,
            SessionDate = blog.session_date,
            CampaignId = blog.campaign_id,
            Blog = blog
        };
    }
    
    public Data.Models.blog ToUpdateBlog(BlogFormModel form) {
        Blog.title = Title;
        Blog.blog_content = Content;
        Blog.publish_date = PublishDate;
        Blog.session_date = SessionDate!.Value;
        Blog.updated_by = form.Username;
        Blog.updated_on = DateTime.UtcNow;
        return Blog;
    }
}
