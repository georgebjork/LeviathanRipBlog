using LeviathanRipBlog.Data;
using LeviathanRipBlog.Data.Models;
using LeviathanRipBlog.Settings;
using Microsoft.AspNetCore.Identity;

namespace LeviathanRipBlog.Services;

public interface IIdentityService
{
    Task<bool> CanEditBlog(string userId, blog blog);
}

public class IdentityService: IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly UserManager<ApplicationUser> userManager;

    public IdentityService(ILogger<IdentityService> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        this.userManager = userManager;
    }

    public async Task<bool> CanEditBlog(string userId, blog blog)
    {
        // Check if owner
        if (userId == blog.owner_id) return true;
        
        // Get user 
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return false;

        // Check if admin 
        return await userManager.IsInRoleAsync(user, Roles.ADMIN);
    }
}