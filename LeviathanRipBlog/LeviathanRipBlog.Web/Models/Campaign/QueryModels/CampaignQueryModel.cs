namespace LeviathanRipBlog.Web.Models.Campaign.QueryModels;

public class CampaignQueryModel {
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
