using Dapper.Contrib.Extensions;
namespace LeviathanRipBlog.Data.Models;

[Table("blog")]
public class blog
{
    [Key]
    public long id { get; set; }
    public long campaign_id { get; set; }
    public string title { get; set; } = "";
    public string blog_content { get; set; } = "";
    public DateTime publish_date { get; set; }
    public DateTime session_date { get; set; }
    public bool is_draft { get; set; }
    public string owner_id { get; set; } = "";
    public bool is_deleted { get; set; }
    public string created_by { get; set; } = "";
    public DateTime created_on { get; set; } 
    public string updated_by { get; set; } = "";
    public DateTime updated_on { get; set; }
}

[Table("campaign")]
public class campaign
{
    [Key]
    public long id { get; set; }
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public string owner_id { get; set; } = "";
    public bool is_deleted { get; set; }
    public string created_by { get; set; } = "";
    public DateTime created_on { get; set; }
    public string updated_by { get; set; } = "";
    public DateTime updated_on { get; set; }
}

[Table("blog_documents")]
public class blog_documents
{
    [Key]
    public long id { get; set; }
    public long blog_id { get; set; }
    public string document_name { get; set; } = "";
    public string document_identifier { get; set; } = "";
    public bool is_deleted { get; set; }
    public string created_by { get; set; } = "";
    public DateTime created_on { get; set; }
    public string updated_by { get; set; } = "";
    public DateTime updated_on { get; set; }
}

