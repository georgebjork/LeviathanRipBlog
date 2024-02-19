using LeviathanRipBlog.Web.Controllers.BaseControllers;
using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Models.ManageUsers;
using LeviathanRipBlog.Web.Services;
using LeviathanRipBlog.Web.Services.ManageUsers;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace LeviathanRipBlog.Web.Controllers;

public class ManageUsersController : BaseAuthorizedController {
    
    private readonly IManageUsersService manageUsersService;
    private readonly IUsernameRetriever usernameRetriever;
    private readonly UserManager<ApplicationUser> UserManager;
    
    public ManageUsersController(IManageUsersService manageUsersService, IUsernameRetriever usernameRetriever, UserManager<ApplicationUser> userManager) {
        this.manageUsersService = manageUsersService;
        this.usernameRetriever = usernameRetriever;
        UserManager = userManager;
    }

    [Route("/manage-users")]
    public async Task<IActionResult> ManageUsers() {

        var users = await manageUsersService.GetUsers();
        users.RemoveAll(u => u.Id == usernameRetriever.UserId);
        
        foreach (var user in users)
        {
            if (await UserManager.IsInRoleAsync(user, Roles.ADMIN)) { user.IsAdmin = true; }
        }

        var invites = await manageUsersService.GetInvites();
        foreach (var invite in invites)
        {
            invite.InviteUrl = Url.Content($"~/register/{invite.invitation_identifier}");
        }

        var vm = new ManageUserViewModel {
            Users = users,
            Invites = invites
        };
        
        return View(vm);
    }
}
