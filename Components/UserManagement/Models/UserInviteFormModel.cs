using System.ComponentModel.DataAnnotations;

namespace LeviathanRipBlog.Components.UserManagement.Models;

public class UserInviteFormModel
{
    [Required(ErrorMessage="An Email is Required")]
    [EmailAddress]
    public string Email { get; set; } = "";
}