using LeviathanRipBlog.Services;
using LeviathanRipBlog.Web.Controllers.BaseControllers;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Models.Blog.FormModels;
using LeviathanRipBlog.Web.Models.Blog.ViewModels;
using LeviathanRipBlog.Web.Services;
using LeviathanRipBlog.Web.Services.Blog;
using LeviathanRipBlog.Web.Services.Documents;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LeviathanRipBlog.Web.Controllers;

public class BlogController : BaseController {
    
    private readonly IBlogService blogService;
    private readonly IDocumentStorage documentStorage;
    private readonly IAuthorizationService authorizationService;
    private readonly IUsernameRetriever usernameRetriever;
    private readonly ILogger<BlogController> _logger;
    
    public BlogController(IBlogService blog_service, IAuthorizationService authorization_service, IUsernameRetriever username_retriever, ILogger<BlogController> logger, IDocumentStorage document_storage) {
        blogService = blog_service;
        authorizationService = authorization_service;
        usernameRetriever = username_retriever;
        _logger = logger;
        documentStorage = document_storage;
    }
    
    [Route("campaign/{CampaignId:long}/blog/{BlogId:long}")]
    public async Task<IActionResult> ViewBlog(long CampaignId, long BlogId) {
       var blog = await blogService.GetBlogById(BlogId);
       
       var isAuthorized = await authorizationService.AuthorizeAsync(User, BlogId, Policy.CanEditBlog);
       ViewBag.IsUserAuthorized = isAuthorized.Succeeded;
       
       var vm = new BlogViewModel {
           Blog = blog
       };

       return View(vm);
    }
    
    [Authorize]
    [Route("campaign/{CampaignId:long}/blog/new")]
    public IActionResult CreateBlog(long CampaignId) {
        var form = new BlogFormModel {
            CampaignId = CampaignId
        };
        
        return View(form);
    }
    
    
    [Authorize(Policy = Policy.CanEditBlog)]
    [Route("campaign/{CampaignId:long}/blog/{BlogId:long}/edit")]
    public async Task<IActionResult> EditBlog(long CampaignId, long BlogId) {
        
        var blog = await blogService.GetBlogById(BlogId);
        
        var form = new BlogFormModel {
            Title = blog.title,
            Content = blog.blog_content,
            SessionDate = blog.session_date,
            CampaignId = blog.campaign_id,
            DocumentIdentifier = blog.document_identifier,
            DocumentName = blog.document_name,
            BlogId = blog.id
        };
        
        return View(form);
    }
    
    [Authorize]
    [HttpPost]
    [Route("campaign/{CampaignId:long}/blog/new")]
    public async Task<IActionResult> CreateBlog(long CampaignId, BlogFormModel form) {
        
        if (!ModelState.IsValid) return View(form);
        
        // Upload file
        if (!documentStorage.IsValidImage(form.File!))
        {
            ModelState.AddModelError("File", "The file must be an image.");
            return View(form);
        }
        
        var document = await documentStorage.SaveDocument(form.File!);
        
        var id = await blogService.CreateBlog(form, document.FileIdentifier);
        
        if (id < 0) {
            SetErrorMessage("Failed to create campaign. Please try again.");
            return View(nameof(CreateBlog), form);
        }
        
        _logger.LogInformation("User {User} created blog {Id}", id, usernameRetriever.Username);
        SetSuccessMessage("Blog created successfully.");
        return RedirectToAction(nameof(ViewBlog), new { CampaignId, BlogId = id});
    }
    
    [Authorize(Policy = Policy.CanEditBlog)]
    [HttpPost]
    [Route("campaign/{CampaignId:long}/blog/{BlogId:long}/edit")]
    public async Task<IActionResult> EditBlog(long CampaignId, long BlogId, BlogFormModel form) {
        
        if (!ModelState.IsValid) return View(nameof(EditBlog), form);

        var document = new SavedDocumentResponse();
        if (form.File is not null)
        {
            if (!documentStorage.IsValidImage(form.File!))
            {
                ModelState.AddModelError("File", "The file must be an image.");
                return View(form);
            }
            document = await documentStorage.SaveDocument(form.File!);
        }
        
        var rv = await blogService.UpdateBlog(form, document.FileIdentifier);
        
        if(rv == false) {
            SetErrorMessage("Failed to update blog. Please try again.");
            return View(nameof(EditBlog), form);
        }
        
        _logger.LogInformation("User {User} updated blog {Id}", BlogId, usernameRetriever.Username);
        SetSuccessMessage("Blog updated successfully.");
        return RedirectToAction(nameof(ViewBlog), new { CampaignId, BlogId });
    }
}
