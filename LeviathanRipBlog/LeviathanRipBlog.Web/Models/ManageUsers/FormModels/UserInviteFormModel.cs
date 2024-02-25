using System.ComponentModel.DataAnnotations;
namespace LeviathanRipBlog.Web.Models.ManageUsers.FormModels;

public class UserInviteFormModel
{
    [Required(ErrorMessage="An Email is Required")]
    [EmailAddress]
    public string Email { get; set; } = "";
}