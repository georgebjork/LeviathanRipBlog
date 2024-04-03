using LeviathanRipBlog.Web.Controllers.BaseControllers;
using LeviathanRipBlog.Web.Data.Repositories;
using LeviathanRipBlog.Web.Models.RequestLog;
using Microsoft.AspNetCore.Mvc;

namespace LeviathanRipBlog.Web.Controllers;

public class RequestLogController(IRequestLogRepository repository) : BaseAuthorizedController
{
    private readonly IRequestLogRepository _repository = repository;

    [Route("/request-log")]
    public async Task<IActionResult> Index()
    {
        var logs = await _repository.GetLogs();
        var vm = new RequestLogViewModel
        {
            Logs = logs.OrderByDescending(x => x.id).ToList()
        };

        return View(vm);
    }
}