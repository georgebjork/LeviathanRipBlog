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
    
    Task<IdentityResult> RemoveUser(string userId);
}

public class ManageUsersService : IManageUsersService {
    
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public ManageUsersService(IUserRepository userRepository, UserManager<ApplicationUser> userManager) {
        _userRepository = userRepository;
        _userManager = userManager;
    }
    
    public async Task<List<ApplicationUser>> GetUsers() {
        return await _userRepository.GetUsers();
    }
    
    public async Task<List<user_invitation>> GetInvites() {
        return await _userRepository.GetInvites();
    }
    
    public async Task<user_invitation?> GetInvite(string inviteId) {
        return await _userRepository.GetInvite(inviteId);
    }
    
    public async Task<ApplicationUser> PromoteAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) throw new NullReferenceException("User not found");
        
        // Add to role
        if (await _userManager.IsInRoleAsync(user, Roles.ADMIN)) return user;
        
        await _userManager.AddToRoleAsync(user, Roles.ADMIN);
        user.IsAdmin = true;

        return user;
    }

    public async Task<ApplicationUser> RevokeAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) throw new NullReferenceException("User not found");
        
        // Remove from role and make sure they're in the user role
        await _userManager.RemoveFromRoleAsync(user, Roles.ADMIN);
        
        if(!await _userManager.IsInRoleAsync(user, Roles.USER))
        {
            await _userManager.AddToRoleAsync(user, Roles.USER);
        }
        
        user.IsAdmin = false;
        return user;
    }

    public async Task<IdentityResult> RemoveUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null) throw new NullReferenceException("User not found"); 
        
        return await _userManager.DeleteAsync(user);
    }
}
