using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
namespace LeviathanRipBlog.Web.Controllers.BaseControllers;

[Authorize(Roles = Roles.ADMIN)]
public class BaseAuthorizedController : BaseController {
    
}
