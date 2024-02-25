using System.ComponentModel.DataAnnotations;
namespace LeviathanRipBlog.Web.Models.Blog.FormModels;

public class BlogFormModel {
    
    [Required]
    [Display(Name = "Title")]
    [MaxLength(100, ErrorMessage = "Maximum length is 100 characters")]
    public string Title { get; set; } = "";
    
    [Required]
    public string Content { get; set; } = "";
    
    [Required]
    [Display(Name = "Session Date")]
    public DateTime SessionDate { get; set; } = DateTime.UtcNow;
    
    public IFormFile? File { get; set; }
    
    public long CampaignId { get; set; }
    public long BlogId { get; set; }
    public string? DocumentIdentifier { get; set; } = "";
    public string? DocumentName { get; set; } = "";
    public bool IsDeleted { get; set; }
}
