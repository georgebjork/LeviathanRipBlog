using LeviathanRipBlog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LeviathanRipBlog.Web.Controllers.BaseControllers;
using LeviathanRipBlog.Web.Models.ViewModels.Home;
using LeviathanRipBlog.Web.Services.Blog;

namespace LeviathanRipBlog.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService blogService;

        public HomeController(ILogger<HomeController> logger, IBlogService blog_service) {
            _logger = logger;
            blogService = blog_service;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [Route("/recent-blogs")]
        public async Task<IActionResult> GetRecentBlogs()
        {
            // 5 most recent blogs
            var blogs = await blogService.GetRecentBlogs(5);
            var vm = new RecentBlogsViewModel
            {
                Blogs = blogs
            };
            return PartialView("HomePage/_RecentBlogs", vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
