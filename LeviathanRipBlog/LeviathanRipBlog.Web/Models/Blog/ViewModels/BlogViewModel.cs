using LeviathanRipBlog.Web.Models.QueryModels.Blog;
namespace LeviathanRipBlog.Web.Models.Blog.ViewModels;

public class BlogViewModel {
    public BlogQueryModel Blog { get; set; } = new();
}
