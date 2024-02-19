using LeviathanRipBlog.Data.Repositories;
using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Identity;
namespace LeviathanRipBlog.Web.Services.ManageUsers;

public interface IManageUsersService {
    Task<List<ApplicationUser>> GetUsers();
    
    // User Invites
    Task<List<user_invitation>> GetInvites();
    Task<user_invitation?> GetInvite(string inviteId);

    Task<ApplicationUser> PromoteAdmin(string userId);
    Task<ApplicationUser> RevokeAdmin(string userId);
}

public class ManageUsersService : IManageUsersService {
    
    private readonly IUserRepository userRepository;
    private readonly UserManager<ApplicationUser> userManager;
    
    public ManageUsersService(IUserRepository userRepository, UserManager<ApplicationUser> userManager) {
        this.userRepository = userRepository;
        this.userManager = userManager;
    }
    
    public async Task<List<ApplicationUser>> GetUsers() {
        return await userRepository.GetUsers();
    }
    
    public async Task<List<user_invitation>> GetInvites() {
        return await userRepository.GetInvites();
    }
    
    public async Task<user_invitation?> GetInvite(string inviteId) {
        return await userRepository.GetInvite(inviteId);
    }
    
    public async Task<ApplicationUser> PromoteAdmin(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null) throw new NullReferenceException("User not found");
        
        // Add to role
        if (await userManager.IsInRoleAsync(user, Roles.ADMIN)) return user;
        
        await userManager.AddToRoleAsync(user, Roles.ADMIN);
        user.IsAdmin = true;

        return user;
    }

    public async Task<ApplicationUser> RevokeAdmin(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null) throw new NullReferenceException("User not found");
        
        // Remove from role and make sure they're in the user role
        await userManager.RemoveFromRoleAsync(user, Roles.ADMIN);
        
        if(!await userManager.IsInRoleAsync(user, Roles.USER))
        {
            await userManager.AddToRoleAsync(user, Roles.USER);
        }
        
        user.IsAdmin = false;
        return user;
    }

    private async Task RemoveUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        if (user is null) throw new NullReferenceException("User not found"); 
        
        await userManager.DeleteAsync(user);
    }
}
