using LeviathanRipBlog.Web.Controllers.BaseControllers;
using LeviathanRipBlog.Web.Models.Campaign.FormModels;
using LeviathanRipBlog.Web.Models.Campaign.ViewModels;
using LeviathanRipBlog.Web.Services;
using LeviathanRipBlog.Web.Services.Blog;
using LeviathanRipBlog.Web.Services.Campaign;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LeviathanRipBlog.Web.Controllers;

public class CampaignController : BaseController {

    private readonly ICampaignService campaignService;
    private readonly IBlogService blogService;
    private readonly IAuthorizationService authorizationService;
    private readonly IUsernameRetriever usernameRetriever;
    private readonly ILogger<CampaignController> _logger;
    
    public CampaignController(ICampaignService campaign_service, ILogger<CampaignController> logger, IBlogService blog_service, IAuthorizationService authorization_service, IUsernameRetriever username_retriever) {
        campaignService = campaign_service;
        _logger = logger;
        blogService = blog_service;
        authorizationService = authorization_service;
        usernameRetriever = username_retriever;
    }
    
    [Route("/campaigns")]
    public async Task<IActionResult> Campaigns() {
        var campaigns = await campaignService.GetCampaigns();
        
        var vm = new CampaignListViewModel() {
            Campaigns = campaigns
        };
        
        return View(vm);
    }
    
    [Route("/campaigns/{CampaignId:long}")]
    public async Task<IActionResult> ViewCampaign(long CampaignId) {
        
        var campaign = await campaignService.GetCampaignById(CampaignId);

        if (campaign is null)
        {
            SetErrorMessage("Campaign not found.");
            return RedirectToAction(nameof(Campaigns));
        }

        var blogs = await blogService.GetCampaignBlogs(CampaignId);
        
        var isAuthorized = await authorizationService.AuthorizeAsync(User, CampaignId, Policy.CanEditCampaign);
        ViewBag.IsUserAuthorized = isAuthorized.Succeeded;
        
        var vm = new CampaignViewModel {
            Campaign = campaign,
            Blogs = blogs
        };

        return View(vm);
    }
    
    [Authorize]
    [Route("/campaigns/my-campaigns")]
    public async Task<IActionResult> MyCampaigns() {
        
        var campaigns = await campaignService.GetCampaignsByUserId(usernameRetriever.UserId);
        
        var vm = new CampaignListViewModel {
            Campaigns = campaigns,
        };

        return View(vm);
    }
    
    [Authorize]
    [Route("/campaigns/create")]
    public IActionResult CreateCampaign() {
        var form = new CampaignFormModel();
        return View(form);
    }
    
    [Authorize]
    [Route("/campaigns/{CampaignId:long}/edit")]
    public async Task<IActionResult> EditCampaign([FromRoute] long CampaignId) {
        
        var campaign = await campaignService.GetCampaignById(CampaignId);
        
        if (campaign is null) {
            SetErrorMessage("Campaign not found.");
            return RedirectToAction(nameof(Campaigns));
        }
        
        var form = new CampaignFormModel {
            Name = campaign.name,
            Description = campaign.description,
            CampaignId = campaign.id
        };
        
        _logger.LogInformation("User {User} is editing campaign {Id}", CampaignId, usernameRetriever.Username);
        return View(form);
    }
    
    [HttpPost]
    [Route("/campaigns/add")]
    [Authorize]
    public async Task<IActionResult> AddCampaign([FromForm] CampaignFormModel form) {
        
        if (!ModelState.IsValid) {
            return View(nameof(CreateCampaign), form);
        }
        
        var id = await campaignService.CreateCampaign(form);

        if (id < 0) {
            SetErrorMessage("Failed to create campaign. Please try again.");
            return View(nameof(CreateCampaign), form);
        }
        
        _logger.LogInformation("User {User} created campaign {Id}", id, usernameRetriever.Username);
        SetSuccessMessage("Campaign created successfully.");
        return RedirectToAction(nameof(ViewCampaign), new { CampaignId = id });
    }
    
    [HttpPost]
    [Route("/campaigns/update")]
    [Authorize]
    public async Task<IActionResult> UpdateCampaign([FromForm] CampaignFormModel form) {
        
        if (!ModelState.IsValid) {
            return View(nameof(EditCampaign), form);
        }
        
        var rv = await campaignService.UpdateCampaign(form);

        if (!rv) {
            SetErrorMessage("Failed to update campaign. Please try again.");
            return View(nameof(EditCampaign), form);
        }
        
        _logger.LogInformation("User {User} updated campaign {Id}", form.CampaignId, usernameRetriever.Username);
        SetSuccessMessage("Campaign updated successfully.");
        return RedirectToAction(nameof(ViewCampaign), new { form.CampaignId });
    }
    
    
    [HttpPost]
    [Route("/campaigns/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteCampaign([FromForm] CampaignFormModel form) {
        
        var rv = await campaignService.UpdateCampaign(form);

        if (!rv) {
            SetErrorMessage("Failed to delete campaign. Please try again.");
            return View(nameof(EditCampaign), form);
        }
        
        _logger.LogInformation("User {User} deleted campaign {Id}", form.CampaignId, usernameRetriever.Username);
        SetErrorMessage("Campaign deleted successfully.");
        return RedirectToAction(nameof(MyCampaigns));
    }
}
