using Microsoft.AspNetCore.Components.Forms;

namespace LeviathanRipBlog.Services;

public interface IDocumentStorage
{
    Task<SavedDocumentResponse> SaveDocument(IBrowserFile file);
    Task<(byte[], string)> RetrieveDocument(string filename);
    Task<bool> RemoveFile(string documentIdentifier);
}