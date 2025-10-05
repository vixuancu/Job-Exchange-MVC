using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Controllers;

/// <summary>
/// Controller dành cho Admin quản lý hệ thống
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUserService userService,
        IJobService jobService,
        IApplicationService applicationService,
        ILogger<AdminController> logger)
    {
        _userService = userService;
        _jobService = jobService;
        _applicationService = applicationService;
        _logger = logger;
    }

    // GET: Admin/Dashboard
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var users = await _userService.GetAllUsersAsync();
        var jobs = await _jobService.GetAllJobsAsync();

        ViewBag.TotalUsers = users.Count();
        ViewBag.TotalEmployers = users.Count(u => u.Role == "Employer");
        ViewBag.TotalApplicants = users.Count(u => u.Role == "Applicant");
        ViewBag.TotalJobs = jobs.Count();
        ViewBag.PendingJobs = jobs.Count(j => j.Status == "Pending");
        ViewBag.ApprovedJobs = jobs.Count(j => j.Status == "Approved");

        return View();
    }

    // GET: Admin/Users
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    // POST: Admin/UpdateUserStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserStatus(int userId, bool isActive)
    {
        var result = await _userService.UpdateUserStatusAsync(userId, isActive);

        if (!result)
        {
            TempData["ErrorMessage"] = "Không thể cập nhật trạng thái người dùng";
        }
        else
        {
            TempData["SuccessMessage"] = $"Đã {(isActive ? "kích hoạt" : "khóa")} tài khoản thành công!";
        }

        return RedirectToAction("Users");
    }

    // POST: Admin/UpdateUserRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserRole(int userId, string role)
    {
        var result = await _userService.UpdateUserRoleAsync(userId, role);

        if (!result)
        {
            TempData["ErrorMessage"] = "Không thể cập nhật vai trò người dùng";
        }
        else
        {
            TempData["SuccessMessage"] = "Cập nhật vai trò thành công!";
        }

        return RedirectToAction("Users");
    }

    // GET: Admin/Jobs
    [HttpGet]
    public async Task<IActionResult> Jobs(string? status)
    {
        var allJobs = await _jobService.GetAllJobsAsync();

        // Filter by status if provided
        var jobs = string.IsNullOrEmpty(status)
            ? allJobs
            : allJobs.Where(j => j.Status == status);

        ViewBag.SelectedStatus = status;
        return View(jobs);
    }

    // POST: Admin/ApproveJob
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveJob(int jobId)
    {
        var result = await _jobService.UpdateJobStatusAsync(jobId, "Approved");

        if (!result)
        {
            TempData["ErrorMessage"] = "Không thể duyệt tin tuyển dụng";
        }
        else
        {
            TempData["SuccessMessage"] = "Duyệt tin tuyển dụng thành công!";
        }

        return RedirectToAction("Jobs");
    }

    // POST: Admin/RejectJob
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectJob(int jobId)
    {
        var result = await _jobService.UpdateJobStatusAsync(jobId, "Rejected");

        if (!result)
        {
            TempData["ErrorMessage"] = "Không thể từ chối tin tuyển dụng";
        }
        else
        {
            TempData["SuccessMessage"] = "Từ chối tin tuyển dụng thành công!";
        }

        return RedirectToAction("Jobs");
    }

    // GET: Admin/Categories
    [HttpGet]
    public async Task<IActionResult> Categories()
    {
        var categories = await _jobService.GetAllCategoriesAsync();
        return View(categories);
    }

    // GET: Admin/Statistics
    [HttpGet]
    public async Task<IActionResult> Statistics()
    {
        var users = await _userService.GetAllUsersAsync();
        var jobs = await _jobService.GetAllJobsAsync();

        var stats = new
        {
            UsersByRole = users.GroupBy(u => u.Role).Select(g => new { Role = g.Key, Count = g.Count() }),
            JobsByCategory = jobs.GroupBy(j => j.CategoryName).Select(g => new { Category = g.Key, Count = g.Count() }),
            JobsByStatus = jobs.GroupBy(j => j.Status).Select(g => new { Status = g.Key, Count = g.Count() }),
            RecentUsers = users.OrderByDescending(u => u.CreatedAt).Take(10),
            RecentJobs = jobs.OrderByDescending(j => j.CreatedAt).Take(10)
        };

        return View(stats);
    }
}
