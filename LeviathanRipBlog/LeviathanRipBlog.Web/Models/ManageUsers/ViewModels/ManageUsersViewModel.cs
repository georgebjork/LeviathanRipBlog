using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Models.ManageUsers.FormModels;

namespace LeviathanRipBlog.Web.Models.ManageUsers.ViewModels;

public class ManageUserViewModel {
    
    public List<ApplicationUser> Users { get; set; } = new();
    public List<user_invitation> Invites { get; set; } = new();

    public UserInviteFormModel InviteFormModel { get; set; } = new();
}