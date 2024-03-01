using System.Diagnostics;
using LeviathanRipBlog.Services;
using LeviathanRipBlog.Web.Helpers;
using LeviathanRipBlog.Web.Settings;
using Microsoft.Extensions.Options;

namespace LeviathanRipBlog.Web.Services.Documents;

public class SupabaseDocumentStorage(ILogger<SupabaseDocumentStorage> logger, IOptions<SupabaseSettings> settings) : IDocumentStorage
{
    private readonly SupabaseSettings _settings = settings.Value;
    
    
    private Supabase.Client GetClient()
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(_settings.SupabaseUrl, _settings.PublicKey, options);
        return supabase;
    }
    
    public async Task<SavedDocumentResponse> SaveDocument(IFormFile file)
    {
        var supabase = GetClient();
        await supabase.InitializeAsync();
        
        try
        {
            var fileName = Guid.NewGuid().ToString();
            
            await using var fileStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(fileStream);
            fileStream.Position = 0; // Reset the position of the stream to the beginning

            var options = new Supabase.Storage.FileOptions { ContentType = file.ContentType };
            
            await supabase.Storage.From(_settings.BucketName).Upload(fileStream.ToArray(), $"{fileName}", options);
            
            return new SavedDocumentResponse { OriginalFileName = file.Name, FileIdentifier = fileName };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "There was an exception in SupabaseDocumentStorage");
            throw;
        }       
    }

    public async Task<(byte[], string)> RetrieveDocument(string filename)
    {
        var supabase = GetClient();
        await supabase.InitializeAsync();
        
        var bytes = await supabase.Storage.From("leviathan-rip-bucket").DownloadPublicFile(filename);
        var mimeType = FileTypeHelper.TryGetExtension(bytes) ?? "application/octet-stream";
        
        return (bytes, mimeType);
    }

    public async Task<bool> RemoveFile(string documentIdentifier)
    {
        try
        {
            var supabase = GetClient();
            await supabase.InitializeAsync();
            
            await supabase.Storage.From("leviathan-rip-bucket").Remove([documentIdentifier]);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception for file: {id}", documentIdentifier);
            return false;
        }
    }
    
    
    private static string GetMimeTypeFromUrl(string url)
    {
        var uri = new Uri(url);
        var path = uri.AbsolutePath;
        var extension = System.IO.Path.GetExtension(path).ToLower();
        
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}