using LeviathanRipBlog.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
namespace LeviathanRipBlog.Services;

public interface IUsernameRetriever
{
    public Task<string> Username { get; }
    public Task<string?> Identity { get; }
}

public class UsernameRetriever : IUsernameRetriever {
    private readonly AuthenticationStateProvider _authentication_state_provider;
    public UsernameRetriever(AuthenticationStateProvider authenticationStateProvider) {
        _authentication_state_provider = authenticationStateProvider;
    }

    public Task<string> Username => _authentication_state_provider.CurrentUserName();
    public Task<string?> Identity => _authentication_state_provider.CurrentIdentity();
}
