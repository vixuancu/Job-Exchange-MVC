using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using JobExchangeMvc.DTOs;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Controllers;

/// <summary>
/// Controller xử lý các thao tác liên quan đến việc làm
/// </summary>
public class JobsController : Controller
{
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        IJobService jobService,
        IApplicationService applicationService,
        ILogger<JobsController> logger)
    {
        _jobService = jobService;
        _applicationService = applicationService;
        _logger = logger;
    }

    // GET: Jobs
    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? location, int page = 1)
    {
        var pagedJobs = await _jobService.GetAllJobsAsync(page: page, pageSize: 10, searchTerm: searchTerm, categoryId: categoryId, location: location);
        var categories = await _jobService.GetAllCategoriesAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CategoryId = categoryId;
        ViewBag.Location = location;
        ViewBag.PagedResult = pagedJobs; // ✅ Pass pagination info to view

        return View(pagedJobs.Items); // ✅ Pass only Items to view
    }

    // GET: Jobs/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);

        if (job == null)
        {
            return NotFound();
        }

        // ✅ FIX #14: Increment view count with unique tracking
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        await _jobService.IncrementViewCountAsync(id, userId, ipAddress, userAgent);

        // Check if user has already applied
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Applicant"))
        {
            ViewBag.HasApplied = await _applicationService.HasAppliedAsync(userId!.Value, id);
        }

        return View(job);
    }

    // GET: Jobs/Create
    [HttpGet]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Create()
    {
        var categories = await _jobService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    // POST: Jobs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Create(JobDto model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _jobService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var job = await _jobService.CreateJobAsync(userId, model);

            TempData["SuccessMessage"] = "Tạo tin tuyển dụng thành công! Tin của bạn đang chờ duyệt.";
            return RedirectToAction("MyJobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job");
            ModelState.AddModelError(string.Empty, ex.Message);

            var categories = await _jobService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }
    }

    // GET: Jobs/Edit/5
    [HttpGet]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var jobs = await _jobService.GetJobsByEmployerAsync(userId);
        var job = jobs.FirstOrDefault(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        var categories = await _jobService.GetAllCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", job.CategoryId);

        return View(job);
    }

    // POST: Jobs/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Edit(int id, JobDto model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _jobService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _jobService.UpdateJobAsync(id, userId, model);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Cập nhật tin tuyển dụng thành công!";
            return RedirectToAction("MyJobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job {JobId}", id);
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật tin tuyển dụng");

            var categories = await _jobService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }
    }

    // POST: Jobs/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        try
        {
            var result = await _jobService.DeleteJobAsync(id, userId);

            if (!result)
            {
                // Check if it's AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không thể xóa tin tuyển dụng. Tin không tồn tại hoặc bạn không có quyền xóa." });
                }

                TempData["ErrorMessage"] = "Không thể xóa tin tuyển dụng";
                return RedirectToAction("MyJobs");
            }

            _logger.LogInformation("Job {JobId} deleted successfully by user {UserId}", id, userId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Xóa tin tuyển dụng thành công!" });
            }

            TempData["SuccessMessage"] = "Xóa tin tuyển dụng thành công!";
            return RedirectToAction("MyJobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job {JobId} by user {UserId}", id, userId);

            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa tin!" });
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa tin!";
            return RedirectToAction("MyJobs");
        }
    }

    // GET: Jobs/MyJobs
    [HttpGet]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> MyJobs(int page = 1)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        // ✅ FIX: Use pagination
        var pagedJobs = await _jobService.GetJobsByEmployerWithPaginationAsync(userId, page: page, pageSize: 10);

        ViewBag.PagedResult = pagedJobs;

        return View(pagedJobs.Items);
    }

    // GET: Jobs/Applicants/5
    [HttpGet]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> Applicants(int id, int page = 1)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var jobs = await _jobService.GetJobsByEmployerAsync(userId);
        var job = jobs.FirstOrDefault(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        // ✅ FIX: GetApplicationsByJobAsync now returns PagedResultDto
        var pagedApplications = await _applicationService.GetApplicationsByJobAsync(id, page: page, pageSize: 20);

        ViewBag.Job = job;
        ViewBag.PagedResult = pagedApplications;

        return View(pagedApplications.Items);
    }

    // AJAX: Search jobs
    [HttpGet]
    public async Task<IActionResult> SearchPartial(string? searchTerm, int? categoryId, string? location, int page = 1)
    {
        var pagedJobs = await _jobService.GetAllJobsAsync(page: page, pageSize: 10, searchTerm: searchTerm, categoryId: categoryId, location: location);
        return PartialView("_JobListPartial", pagedJobs.Items);
    }
}
