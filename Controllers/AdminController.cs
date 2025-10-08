using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobExchangeMvc.Services.Interfaces;
using JobExchangeMvc.DTOs;

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
        try
        {
            // ✅ FIX #11: Comprehensive Dashboard Statistics
            var users = await _userService.GetAllUsersAsync();
            var pagedJobs = await _jobService.GetAllJobsForAdminAsync(page: 1, pageSize: 10000);
            var jobs = pagedJobs.Items.ToList();
            var applications = (await _applicationService.GetAllApplicationsForAdminAsync()).ToList();

            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            var stats = new AdminDashboardStatsDto
            {
                // User Statistics
                TotalUsers = users.Count(),
                TotalAdmins = users.Count(u => u.Role == "Admin"),
                TotalEmployers = users.Count(u => u.Role == "Employer"),
                TotalApplicants = users.Count(u => u.Role == "Applicant"),
                ActiveUsers = users.Count(u => u.IsActive),
                InactiveUsers = users.Count(u => !u.IsActive),
                NewUsersThisMonth = users.Count(u => u.CreatedAt >= firstDayOfMonth),

                // Job Statistics - Breakdown by Status
                TotalJobs = jobs.Count,
                PendingJobs = jobs.Count(j => j.Status == "Pending"),
                ApprovedJobs = jobs.Count(j => j.Status == "Approved"),
                RejectedJobs = jobs.Count(j => j.Status == "Rejected"),
                ClosedJobs = jobs.Count(j => j.Status == "Closed"),
                ExpiredJobs = jobs.Count(j => j.Status == "Expired"),
                NewJobsThisMonth = jobs.Count(j => j.CreatedAt >= firstDayOfMonth),
                TotalViews = jobs.Sum(j => j.ViewCount),

                // Application Statistics - Breakdown by Status
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == "Pending"),
                ApprovedApplications = applications.Count(a => a.Status == "Approved"),
                InterviewingApplications = applications.Count(a => a.Status == "Interviewing"),
                AcceptedApplications = applications.Count(a => a.Status == "Accepted"),
                RejectedApplications = applications.Count(a => a.Status == "Rejected"),
                CancelledApplications = applications.Count(a => a.Status == "Cancelled"),
                NewApplicationsThisMonth = applications.Count(a => a.AppliedAt >= firstDayOfMonth),

                // Top Categories (Top 5)
                TopCategories = jobs
                    .Where(j => !string.IsNullOrEmpty(j.CategoryName))
                    .GroupBy(j => j.CategoryName!)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),

                // Top Companies (Top 5 by job count)
                TopCompanies = jobs
                    .Where(j => !string.IsNullOrEmpty(j.CompanyName))
                    .GroupBy(j => j.CompanyName!)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return View(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dashboard";
            return View(new AdminDashboardStatsDto());
        }
    }

    // GET: Admin/Users
    [HttpGet]
    public async Task<IActionResult> Users(string? role, string? status, int page = 1)
    {
        // ✅ FIX: Apply filters in service layer BEFORE pagination
        var pagedUsers = await _userService.GetAllUsersAsProfileDtoAsync(page: page, pageSize: 10, role: role, status: status);

        ViewBag.SelectedRole = role;
        ViewBag.SelectedStatus = status;
        ViewBag.PagedResult = pagedUsers;

        return View(pagedUsers.Items);
    }

    // POST: Admin/UpdateUserStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserStatus(int userId, bool isActive)
    {
        var result = await _userService.UpdateUserStatusAsync(userId, isActive);

        if (!result)
        {
            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Không thể cập nhật trạng thái người dùng" });
            }

            TempData["ErrorMessage"] = "Không thể cập nhật trạng thái người dùng";
        }
        else
        {
            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = $"Đã {(isActive ? "kích hoạt" : "khóa")} tài khoản thành công!" });
            }

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
    public async Task<IActionResult> Jobs(string? status, int page = 1)
    {
        // ✅ FIX: Apply status filter in service layer BEFORE pagination
        var pagedJobs = await _jobService.GetAllJobsForAdminAsync(page: page, pageSize: 10, status: status);

        ViewBag.SelectedStatus = status;
        ViewBag.PagedResult = pagedJobs;

        return View(pagedJobs.Items);
    }

    // POST: Admin/ApproveJob
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveJob(int jobId)
    {
        try
        {
            var result = await _jobService.UpdateJobStatusAsync(jobId, "Approved");

            if (!result)
            {
                // Check if it's AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không thể duyệt tin tuyển dụng. Tin không tồn tại." });
                }

                TempData["ErrorMessage"] = "Không thể duyệt tin tuyển dụng. Tin không tồn tại.";
                return RedirectToAction("Jobs");
            }

            _logger.LogInformation("Job {JobId} approved successfully", jobId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Duyệt tin tuyển dụng thành công!" });
            }

            TempData["SuccessMessage"] = "Duyệt tin tuyển dụng thành công!";
            return RedirectToAction("Jobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving job {JobId}", jobId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi duyệt tin!" });
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi duyệt tin!";
            return RedirectToAction("Jobs");
        }
    }

    // POST: Admin/RejectJob
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectJob(int jobId)
    {
        try
        {
            var result = await _jobService.UpdateJobStatusAsync(jobId, "Rejected");

            if (!result)
            {
                // Check if it's AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không thể từ chối tin tuyển dụng. Tin không tồn tại." });
                }

                TempData["ErrorMessage"] = "Không thể từ chối tin tuyển dụng. Tin không tồn tại.";
                return RedirectToAction("Jobs");
            }

            _logger.LogInformation("Job {JobId} rejected successfully", jobId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Từ chối tin tuyển dụng thành công!" });
            }

            TempData["SuccessMessage"] = "Từ chối tin tuyển dụng thành công!";
            return RedirectToAction("Jobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting job {JobId}", jobId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi từ chối tin!" });
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi từ chối tin!";
            return RedirectToAction("Jobs");
        }
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
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Tên danh mục không được để trống" });
                }

                TempData["ErrorMessage"] = "Tên danh mục không được để trống";
                return RedirectToAction("Categories");
            }

            await _jobService.CreateCategoryAsync(name, description);

            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Thêm danh mục thành công!" });
            }

            TempData["SuccessMessage"] = "Thêm danh mục thành công!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");

            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm danh mục" });
            }

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
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Tên danh mục không được để trống" });
                }

                TempData["ErrorMessage"] = "Tên danh mục không được để trống";
                return RedirectToAction("Categories");
            }

            var result = await _jobService.UpdateCategoryAsync(id, name, description, isActive);

            if (result)
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Cập nhật danh mục thành công!" });
                }

                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            }
            else
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không tìm thấy danh mục" });
                }

                TempData["ErrorMessage"] = "Không tìm thấy danh mục";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);

            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật danh mục" });
            }

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
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = $"Đã {(isActive ? "kích hoạt" : "tạm dừng")} danh mục thành công!" });
                }

                TempData["SuccessMessage"] = $"Đã {(isActive ? "kích hoạt" : "tạm dừng")} danh mục thành công!";
            }
            else
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không tìm thấy danh mục" });
                }

                TempData["ErrorMessage"] = "Không tìm thấy danh mục";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling category status {CategoryId}", id);

            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi thay đổi trạng thái danh mục" });
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thay đổi trạng thái danh mục";
        }

        return RedirectToAction("Categories");
    }

    // GET: Admin/Statistics
    [HttpGet]
    public async Task<IActionResult> Statistics()
    {
        var users = await _userService.GetAllUsersAsync();
        var pagedJobs = await _jobService.GetAllJobsForAdminAsync(page: 1, pageSize: 10000); // Large pageSize để lấy tất cả cho stats
        var jobs = pagedJobs.Items;

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

    // ✅ FIX #2: POST: Admin/ExpireJobs - Manually trigger job expiration
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExpireJobs()
    {
        try
        {
            var expiredCount = await _jobService.ExpireJobsPastDeadlineAsync();

            if (expiredCount > 0)
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = $"Đã tự động đóng {expiredCount} tin tuyển dụng quá hạn!", count = expiredCount });
                }

                TempData["SuccessMessage"] = $"Đã tự động đóng {expiredCount} tin tuyển dụng quá hạn!";
            }
            else
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Không có tin tuyển dụng nào quá hạn.", count = 0 });
                }

                TempData["InfoMessage"] = "Không có tin tuyển dụng nào quá hạn.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error expiring jobs");

            // Check if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi đóng tin tuyển dụng quá hạn" });
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi đóng tin tuyển dụng quá hạn";
        }

        return RedirectToAction("Jobs");
    }

    // ✅ NEW: POST: Admin/HardDeleteJob - XÓA HẰN job khỏi database
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDeleteJob(int jobId)
    {
        try
        {
            var result = await _jobService.HardDeleteJobAsync(jobId);

            if (result)
            {
                _logger.LogInformation("Admin hard deleted job {JobId}", jobId);

                // Check if it's AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Đã xóa vĩnh viễn tin tuyển dụng khỏi hệ thống!" });
                }

                TempData["SuccessMessage"] = "Đã xóa vĩnh viễn tin tuyển dụng khỏi hệ thống!";
            }
            else
            {
                // Check if it's AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không tìm thấy tin tuyển dụng để xóa" });
                }

                TempData["ErrorMessage"] = "Không tìm thấy tin tuyển dụng để xóa";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hard deleting job {JobId}", jobId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa tin tuyển dụng!" });
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa tin tuyển dụng!";
        }

        return RedirectToAction("Jobs");
    }
}
