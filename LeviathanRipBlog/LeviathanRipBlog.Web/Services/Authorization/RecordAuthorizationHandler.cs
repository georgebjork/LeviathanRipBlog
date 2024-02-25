using System.Security.Claims;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
namespace LeviathanRipBlog.Web.Services.Authorization;

public class RecordAuthorizationHandler(IRecordOwnershipService recordOwnershipService) : IAuthorizationHandler {

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.PendingRequirements.ToList())
        {
            await ProcessRequirement(context, requirement);
        }
    }
    
    private async Task ProcessRequirement(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        switch (requirement)
        {
            case  CampaignOwnerRequirement campaignOwnerRequirement:
                await HandleCampaignOwnerRequirement(context, campaignOwnerRequirement);
                break;
            case BlogOwnerRequirement blogOwnerRequirement:
                await HandleBlogOwnerRequirement(context, blogOwnerRequirement);
                break;
        }
    }
    
    private async Task HandleCampaignOwnerRequirement(AuthorizationHandlerContext context, CampaignOwnerRequirement requirement)
    {
        var userIdClaim = GetUserId(context);
        if (userIdClaim == null || !GetRouteValue(context, "CampaignId").HasValue)
        {
            context.Fail();
            return;
        }

        if (IsAdmin(context) || await recordOwnershipService.IsCampaignOwner(userIdClaim.Value, GetRouteValue(context, "CampaignId")!.Value))
        {
            context.Succeed(requirement);
            return;
        }
        context.Fail();
    }

    
    private async Task HandleBlogOwnerRequirement(AuthorizationHandlerContext context, BlogOwnerRequirement requirement)
    {
        var userIdClaim = GetUserId(context);
        if (userIdClaim == null || !GetRouteValue(context, "BlogId").HasValue)
        {
            context.Fail();
            return;
        }

        if (IsAdmin(context) || await recordOwnershipService.IsBlogOwner(userIdClaim.Value, GetRouteValue(context, "BlogId")!.Value))
        {
            context.Succeed(requirement);
            return;
        }
        context.Fail();
    }

    private long? GetRouteValue(AuthorizationHandlerContext context, string key)
    {
        switch (context.Resource)
        {
            // First, check if context.Resource is a long and return it directly if so
            case long resourceId:
                return resourceId;

            // If context.Resource is not a long, proceed to check if it's an HttpContext
            case HttpContext httpContext:
            {
                var val = httpContext.GetRouteValue(key)?.ToString();
                if (long.TryParse(val, out var value))
                {
                    return value;
                }
                break;
            }
        }

        return null; // Returning null if the value can't be determined
    }

    
    private Claim? GetUserId(AuthorizationHandlerContext context)
    {
        return context.User.FindFirst(ClaimTypes.NameIdentifier);
    }

    private bool IsAdmin(AuthorizationHandlerContext context)
    {
        return context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == Roles.ADMIN);
    }

}

public record CampaignOwnerRequirement : IAuthorizationRequirement;
public record BlogOwnerRequirement : IAuthorizationRequirement;