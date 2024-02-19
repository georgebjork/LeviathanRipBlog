using LeviathanRipBlog.Data.Repositories;
using LeviathanRipBlog.Web.Data.Repositories;
using LeviathanRipBlog.Web.Services;
using LeviathanRipBlog.Web.Services.Authorization;
using LeviathanRipBlog.Web.Services.Blog;
using LeviathanRipBlog.Web.Services.Campaign;
using LeviathanRipBlog.Web.Services.ManageUsers;
namespace LeviathanRipBlog.Web;

public static class RegisterServices {
    
    public static void ConfigureServices(this IServiceCollection services) {
        
        // Repositories
        services.AddTransient<ICampaignRepository, CampaignRepository>();
        services.AddTransient<IBlogRepository, BlogRepository>();
        services.AddTransient<IDocumentRepository, DocumentRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IRecordOwnerRepository, RecordOwnerRepository>();
        
        // Services
        services.AddTransient<IUsernameRetriever, UsernameRetriever>();
        services.AddSingleton<IRecordOwnershipService, RecordOwnershipService>();
        
        services.AddTransient<IBlogService, BlogService>();
        services.AddTransient<ICampaignService, CampaignService>();
        services.AddTransient<IManageUsersService, ManageUsersService>();
    }
}
