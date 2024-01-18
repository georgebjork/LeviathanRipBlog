using Microsoft.AspNetCore.Components.Forms;
using static System.String;

namespace LeviathanRipBlog.Services;



public class FileDocumentStorage(IConfiguration configuration, ILogger<FileDocumentStorage> logger) : IDocumentStorage
{
    private string RootPath => configuration.GetValue<string?>("file_root_path") ?? "c:\\storage\\blog";
    private long MaxFileSize => 50L * 1024 * 1024; // 50MB

    public async Task<SavedDocumentResponse> SaveDocument(IBrowserFile file)
    {
        try
        {
            // Random file name
            var fileName = Guid.NewGuid();
            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);

            var filePath = Path.Combine(RootPath, fileName.ToString());

            await using FileStream fs = new(filePath, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fs);
            
            return new SavedDocumentResponse { OriginalFileName = file.Name, FileIdentifier = fileName.ToString() };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "File {file} was unable to be uploaded", file.Name);
            throw;
        }
    }

    public async Task<(byte[], string)> RetrieveDocument(string filename) {
        try {
            var filePath = Path.Combine(RootPath, filename);
            if (File.Exists(filePath)) {
                var bytes = await File.ReadAllBytesAsync(filePath);
                var mimeType = GetMimeType(filePath);
                return (bytes, mimeType);
            }
        }
        catch {
            // ignored
        }

        return (Array.Empty<byte>(), string.Empty);
    }

    public Task<bool> RemoveFile(string documentIdentifier)
    {
        var filePath = Path.Combine(RootPath, documentIdentifier);

        if (!File.Exists(filePath))
        {
            logger.LogWarning("File not found: {id}", documentIdentifier);
            return Task.FromResult(true); // Even tho this is a technical failure... there is no need to lock in the user and prevent them from deleting. Its already gone.
        }

        try
        {
            File.Delete(filePath);
            logger.LogInformation("File deleted: {id}", documentIdentifier);
            return Task.FromResult(true);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "IO exception for file: {id}", documentIdentifier);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception for file: {id}", documentIdentifier);
            return Task.FromResult(false);
        }
    }
    
    private string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            // Add other extensions and MIME types as needed
            _ => "application/octet-stream" // Default or unknown file types
        };
    }
}





public record SavedDocumentResponse
{
    public string OriginalFileName = "";
    public string FileIdentifier = "";
}