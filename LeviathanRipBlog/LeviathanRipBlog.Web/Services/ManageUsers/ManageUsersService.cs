using LeviathanRipBlog.Data.Repositories;
using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Data.Models;
namespace LeviathanRipBlog.Web.Services.ManageUsers;

public interface IManageUsersService {
    Task<List<ApplicationUser>> GetUsers();
    
    // User Invites
    Task<List<user_invitation>> GetInvites();
    Task<user_invitation?> GetInvite(string inviteId);
}

public class ManageUsersService : IManageUsersService {
    
    private readonly IUserRepository userRepository;
    
    public ManageUsersService(IUserRepository userRepository) {
        this.userRepository = userRepository;
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
}
