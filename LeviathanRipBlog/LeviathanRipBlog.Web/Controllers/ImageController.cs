using LeviathanRipBlog.Services;
using LeviathanRipBlog.Web.Controllers.BaseControllers;
using LeviathanRipBlog.Web.Models.Blog.FormModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
namespace LeviathanRipBlog.Web.Controllers;

public class ImageController : BaseController {
    
    private readonly IDocumentStorage documentStorage;
    private readonly ILogger<ImageController> _logger;
    //private readonly HttpContext httpContext;
    
    
    public ImageController(ILogger<ImageController> logger, IDocumentStorage document_storage) {
        _logger = logger;
        documentStorage = document_storage;
    }
    
    /*[ResponseCache(Duration = 43200)]*/
    [Route("/api/img/{imgId}")]
    public async Task<IResult> GetImage(string imgId) {
        
        var (imageData, mimeType) = await documentStorage.RetrieveDocument(imgId);
        if (imageData.Length == 0 || string.IsNullOrWhiteSpace(mimeType))
        {
            return Results.NotFound();
        }

        // Set the cache-control header to save on client
       // httpContext.Response.Headers.CacheControl = "public,max-age=43200"; // 12 hours

        return Results.File(imageData, mimeType);
    }
    
    [HttpPost]
    [Route("/api/img/{imgId}/delete")]
    public async Task<IActionResult> DeleteImage(string imgId, [FromForm] BlogFormModel model) {
        var success = await documentStorage.RemoveFile(imgId);
        if (success)
        {
            model.DocumentIdentifier = "";
            model.DocumentName = "";
            return PartialView("~/Views/Blog/Shared/_BlogImageUpload.cshtml", model);
        }
        return PartialView("~/Views/Blog/Shared/_BlogImageUpload.cshtml", model);
    }
}
