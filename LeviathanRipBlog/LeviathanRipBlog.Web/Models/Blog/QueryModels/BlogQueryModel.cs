namespace LeviathanRipBlog.Web.Models.QueryModels.Blog;

public class BlogQueryModel {
    
    public int id { get; set; }
    public int campaign_id { get; set; }
    public string title { get; set; }
    public string blog_content { get; set; }
    public DateTime publish_date { get; set; }
    public DateTime session_date { get; set; }
    public bool is_draft { get; set; }
    public bool is_deleted { get; set; }
    public string owner_id { get; set; }
    public string created_by { get; set; }
    public DateTime created_on { get; set; }
    public string updated_by { get; set; }
    public DateTime updated_on { get; set; }
    public string document_identifier { get; set; }
    public string document_name { get; set; }
    public string campaign_name { get; set; }
}
