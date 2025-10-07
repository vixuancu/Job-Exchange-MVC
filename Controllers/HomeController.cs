using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using JobExchangeMvc.Models;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Controllers;

/// <summary>
/// Controller trang chủ
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJobService _jobService;

    public HomeController(
        ILogger<HomeController> logger,
        IJobService jobService)
    {
        _logger = logger;
        _jobService = jobService;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy 6 việc làm mới nhất (page 1, pageSize 6)
        var pagedJobs = await _jobService.GetAllJobsAsync(page: 1, pageSize: 6);

        // Lấy danh mục
        var categories = await _jobService.GetAllCategoriesAsync();

        ViewBag.Categories = categories;
        return View(pagedJobs.Items); // Chỉ truyền Items (IEnumerable<JobDto>)
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
