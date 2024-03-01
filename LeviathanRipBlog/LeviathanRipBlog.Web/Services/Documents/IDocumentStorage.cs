using LeviathanRipBlog.Web.Services;
using LeviathanRipBlog.Web.Services.Documents;
using Microsoft.AspNetCore.Components.Forms;

namespace LeviathanRipBlog.Services;

public interface IDocumentStorage
{
    Task<SavedDocumentResponse> SaveDocument(IFormFile file);
    Task<(byte[], string)> RetrieveDocument(string filename);
    Task<bool> RemoveFile(string documentIdentifier);
    
    
    
    public bool IsValidImage(IFormFile file) {
        var validImageMimeTypes = new List<string>
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp"
        };
        return validImageMimeTypes.Contains(file.ContentType.ToLowerInvariant());
    }
    
   
}