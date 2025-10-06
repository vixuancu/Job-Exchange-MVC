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
    public async Task<IActionResult> Users(string? role, string? status)
    {
        var users = await _userService.GetAllUsersAsProfileDtoAsync();

        // Filter by role
        if (!string.IsNullOrEmpty(role))
        {
            users = users.Where(u => u.Role == role);
        }

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            if (status == "active")
            {
                users = users.Where(u => u.IsActive);
            }
            else if (status == "inactive")
            {
                users = users.Where(u => !u.IsActive);
            }
        }

        ViewBag.SelectedRole = role;
        ViewBag.SelectedStatus = status;

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
        // Get all categories including inactive ones for admin
        var categories = await _jobService.GetAllCategoriesForAdminAsync();
        return View(categories);
    }

    // POST: Admin/CreateCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(string name, string? description)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Tên danh mục không được để trống";
                return RedirectToAction("Categories");
            }

            await _jobService.CreateCategoryAsync(name, description);
            TempData["SuccessMessage"] = "Thêm danh mục thành công!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm danh mục";
        }

        return RedirectToAction("Categories");
    }

    // POST: Admin/UpdateCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, string name, string? description, bool isActive)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Tên danh mục không được để trống";
                return RedirectToAction("Categories");
            }

            var result = await _jobService.UpdateCategoryAsync(id, name, description, isActive);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật danh mục";
        }

        return RedirectToAction("Categories");
    }

    // POST: Admin/ToggleCategoryStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCategoryStatus(int id, bool isActive)
    {
        try
        {
            var result = await _jobService.ToggleCategoryStatusAsync(id, isActive);

            if (result)
            {
                TempData["SuccessMessage"] = $"Đã {(isActive ? "kích hoạt" : "tạm dừng")} danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling category status {CategoryId}", id);
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thay đổi trạng thái danh mục";
        }

        return RedirectToAction("Categories");
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
