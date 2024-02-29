using LeviathanRipBlog.Services;
using LeviathanRipBlog.Web.Settings;
using Microsoft.Extensions.Options;
using Supabase;

namespace LeviathanRipBlog.Web.Services.Documents;

public class SupabaseDocumentStorage(ILogger<SupabaseDocumentStorage> logger, Client client) : IDocumentStorage
{
    public Task<SavedDocumentResponse> SaveDocument(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<(byte[], string)> RetrieveDocument(string filename)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveFile(string documentIdentifier)
    {
        throw new NotImplementedException();
    }
}