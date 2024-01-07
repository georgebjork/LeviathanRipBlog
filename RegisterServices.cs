using LeviathanRipBlog.Data.Repositories;
using LeviathanRipBlog.Services;
namespace LeviathanRipBlog;

public static class RegisterServices {
    
    public static void Configure(this IServiceCollection services) {
        
        // Repositories
        services.AddTransient<ICampaignRepository, CampaignRepository>();
        services.AddTransient<IBlogRepository, BlogRepository>();
        services.AddTransient<IDocumentRepository, DocumentRepository>();
        
        // Services
        services.AddTransient<IUsernameRetriever, UsernameRetriever>();
        services.AddSingleton<StatusMessageService>();
        services.AddTransient<IDocumentStorage, FileDocumentStorage>();

    }
}
