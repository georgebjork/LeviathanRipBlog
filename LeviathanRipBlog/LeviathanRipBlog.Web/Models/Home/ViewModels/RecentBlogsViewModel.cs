using LeviathanRipBlog.Web.Models.QueryModels.Blog;
namespace LeviathanRipBlog.Web.Models.ViewModels.Home;

public class RecentBlogsViewModel {

    public List<BlogQueryModel> Blogs { get; set; } = new();
}
