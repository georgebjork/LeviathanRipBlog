using Dapper;
using LeviathanRipBlog.Data.Models;

namespace LeviathanRipBlog.Data.Repositories;

public interface IDocumentRepository : IBaseRepository
{
    Task<string?> GetDocumentName(string documentIdentifier);
    Task<string?> GetDocumentIdentifierByBlogId(long blogId);

    Task<blog_documents?> GetDocumentByBlogId(long blogId);
}

public class DocumentRepository(IConfiguration configuration, ILogger<DocumentRepository> logger)
    : BasePgRepository(configuration, logger), IDocumentRepository
{
    public ILogger<DocumentRepository> Logger { get; } = logger;


    public async Task<string?> GetDocumentName(string document_identifier)
    {
        var sql = "SELECT document_name FROM blog_documents WHERE document_identifier = @id AND is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<string>(sql, new { id = document_identifier});
        return rv;
    }

    public async Task<string?> GetDocumentIdentifierByBlogId(long blogId)
    {
        var sql = "SELECT document_identifier FROM blog_documents WHERE blog_id = @id AND is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<string>(sql, new { id = blogId});
        return rv;
    }

    public async Task<blog_documents?> GetDocumentByBlogId(long blogId)
    {
        var sql = "SELECT * FROM blog_documents WHERE blog_id = @id AND is_deleted = false";
        await using var conn = await GetNewOpenConnection();
        var rv = await conn.QueryFirstOrDefaultAsync<blog_documents>(sql, new { id = blogId});
        return rv;
    }
}