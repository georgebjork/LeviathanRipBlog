using LeviathanRipBlog.Web.Helpers;
namespace LeviathanRipBlog.Web.Services;

public interface IUsernameRetriever
{
    public string Username { get; }
    public string UserId { get; }
}

public class UsernameRetriever(IHttpContextAccessor httpContextAccessor) : IUsernameRetriever {

    public string Username => httpContextAccessor.CurrentUserName();
    public string UserId => httpContextAccessor.CurrentUserId();
}
