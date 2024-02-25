using LeviathanRipBlog.Data.Repositories;
using LeviathanRipBlog.Web.Data.Models;
using LeviathanRipBlog.Web.Helpers;
using LeviathanRipBlog.Web.Models.Blog.FormModels;
using LeviathanRipBlog.Web.Models.QueryModels.Blog;
using LeviathanRipBlog.Web.Services.Documents;
namespace LeviathanRipBlog.Web.Services.Blog;

public interface IBlogService {
    public Task<BlogQueryModel> GetBlogById(long id);    
    public Task<List<BlogQueryModel>> GetCampaignBlogs(long campaignId);
    public Task<List<BlogQueryModel>> GetRecentBlogs(int numBlogs); 
    
    public Task<long> CreateBlog(BlogFormModel form, string documentIdentifier);
    public Task<bool> UpdateBlog(BlogFormModel form, string documentIdentifier = "");
}


public class BlogService : IBlogService {
    
    private readonly ILogger<BlogService> logger;
    private readonly IBlogRepository blogRepository;
    private readonly IUsernameRetriever usernameRetriever;
    private readonly IDocumentRepository documentRepository;
    
    public BlogService(IBlogRepository blog_repository, ILogger<BlogService> logger, IUsernameRetriever username_retriever, IDocumentRepository document_repository) {
        blogRepository = blog_repository;
        this.logger = logger;
        usernameRetriever = username_retriever;
        documentRepository = document_repository;
    }

    public async Task<BlogQueryModel> GetBlogById(long id) {
        var blog = await blogRepository.GetBlogById(id);
        
        if (blog is null) {
            throw new Exception($"Blog with id {id} not found");
        }
        
        logger.LogInformation("User {UsernameRetrieverUsername} retrieved blog with id {Id}", usernameRetriever.Username, id);
        return blog!;
    }
    
    public async Task<List<BlogQueryModel>> GetCampaignBlogs(long campaignId) {
        var blogs = await blogRepository.GetCampaignBlogs(campaignId);
        return blogs;
    }
    
    public async Task<List<BlogQueryModel>> GetRecentBlogs(int numBlogs) {
        var blogs = await blogRepository.GetRecentBlogs(numBlogs);
        return blogs;
    }
    
    
    public async Task<long> CreateBlog(BlogFormModel form, string documentIdentifier) {
        
        var userId = usernameRetriever.UserId;
        var username = usernameRetriever.Username;
        var date = DateTime.UtcNow;
        
        var record = new blog {
            campaign_id = form.CampaignId,
            title = form.Title,
            blog_content = form.Content,
            publish_date = date,
            session_date = form.SessionDate,
            is_draft = false,
            owner_id = userId,
            is_deleted = false,
            created_by = username,
            created_on = date,
            updated_by = username,
            updated_on = date
        };
        
        var rv = await blogRepository.Insert(record);
        
        var document_record = new blog_documents {
            blog_id = rv,
            document_name = form.File!.FileName,
            document_identifier = documentIdentifier,
            is_deleted = false,
            created_by = username,
            created_on = date,
            updated_by = username,
            updated_on = date
        };
        
        await blogRepository.Insert(document_record);
        return rv;
    }
    public async Task<bool> UpdateBlog(BlogFormModel form, string documentIdentifier = "") {
        
        var blog = await blogRepository.GetBlogById(form.BlogId);
        
        if(blog is null) {
            logger.LogWarning("User {UsernameRetrieverUsername} attempted to update blog with id {Id} but it was not found", usernameRetriever.Username, form.BlogId);
            return false;
        }
        
        blog.title = form.Title;
        blog.is_deleted = form.IsDeleted;
        blog.blog_content = form.Content;
        blog.session_date = form.SessionDate;
        blog.updated_by = usernameRetriever.Username;
        blog.updated_on = DateTime.UtcNow;
        
        var rv = await blogRepository.Update(blog.ToBlog());
        
        // If not null, then we have a new file to upload and will update our record accordingly. 
        // But, if deleted is true, then we need to update the record to reflect that.
        if (form.File is null && !form.IsDeleted) return rv;
        
        var document = await documentRepository.GetDocumentByBlogId(form.BlogId);

        if (document is null) return rv;

        document.is_deleted = form.IsDeleted;
        document.updated_by = usernameRetriever.Username;
        document.updated_on = DateTime.UtcNow;
        
        if (!form.IsDeleted && form.File is not null)
        {
            document.document_name = form.File!.FileName;
            document.document_identifier = documentIdentifier;
        }
        await documentRepository.Update(document);

        return rv;
    }
}
