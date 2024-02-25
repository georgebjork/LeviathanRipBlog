using LeviathanRipBlog.Web.Data;
using LeviathanRipBlog.Web.Data.Models;
namespace LeviathanRipBlog.Web.Models.ManageUsers;

public class ManageUserViewModel {
    
    public List<ApplicationUser> Users { get; set; } = new();
    public List<user_invitation> Invites { get; set; } = new();
 }
