using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using LeviathanRipBlog.Services;
using LeviathanRipBlog.Web.Settings;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace LeviathanRipBlog.Web.Services.Documents;

public class SpacesDocumentStorage(IOptions<S3Settings> settings, ILogger<SpacesDocumentStorage> logger) : IDocumentStorage
{
    private readonly S3Settings _settings = settings.Value;
    private AmazonS3Client GetClient()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = _settings.SpacesUrl
        };

        var client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, config);
        return client;
    }
    
    public async Task<SavedDocumentResponse> SaveDocument(IFormFile file)
    {
        try
        {
            var fileName = Guid.NewGuid().ToString();

            await using var fileStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(fileStream);
            fileStream.Position = 0; // Reset the position of the stream to the beginning

            var contentType = file.ContentType;

            var putRequest = new PutObjectRequest
            {
                Key = fileName,
                BucketName = _settings.BucketName,
                InputStream = fileStream,
                ContentType = contentType
            };
            var response = await GetClient().PutObjectAsync(putRequest);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                logger.LogError("Response from Spaces while uploading file {file} was not 200. Response was {code}",
                    file.Name, response.HttpStatusCode);
                throw new BadHttpRequestException("Request came back not 200");
            }
            
            // Return 
            return new SavedDocumentResponse { OriginalFileName = file.Name, FileIdentifier = fileName.ToString() };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "There was an exception in SpacesDocumentStorage");
            throw;
        }       
    }

    public async Task<(byte[], string)> RetrieveDocument(string filename)
    {
        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = filename
            };

            using var response = await GetClient().GetObjectAsync(getRequest);
        
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                logger.LogError("Response from Spaces while getting file {file} was not 200. Response was {code}",
                    filename, response.HttpStatusCode);
                throw new BadHttpRequestException("Request came back not 200");
            }
        
            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);

            // Retrieve the MIME type from the response headers
            var mimeType = response.Headers["Content-Type"];

            return (memoryStream.ToArray(), mimeType);
        }
        catch (Exception ex)
        {
            // Handle the exception
            logger.LogError(ex, "An error occurred while retrieving the document {filename}", filename);
            return (Array.Empty<byte>(), string.Empty);
        }
    }


    public async Task<bool> RemoveFile(string documentIdentifier)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = documentIdentifier
            };

            var response = await GetClient().DeleteObjectAsync(deleteRequest);

            if (response.HttpStatusCode == HttpStatusCode.NoContent || response.HttpStatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation("File deleted from S3: {id}", documentIdentifier);
                return true;
            }
            else
            {
                logger.LogWarning("Failed to delete file from S3: {id}. Response code: {code}", documentIdentifier, response.HttpStatusCode);
                return false;
            }
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogWarning("File not found in S3: {id}", documentIdentifier);
                return true; // The file is already not present in S3, treat as success.
            }

            logger.LogError(ex, "AmazonS3 exception for file: {id}", documentIdentifier);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception for file: {id}", documentIdentifier);
            return false;
        }
    }
}