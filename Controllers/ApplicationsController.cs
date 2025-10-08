using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobExchangeMvc.DTOs;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Controllers;

/// <summary>
/// Controller xử lý các thao tác liên quan đến đơn ứng tuyển
/// </summary>
[Authorize]
public class ApplicationsController : Controller
{
    private readonly IApplicationService _applicationService;
    private readonly IJobService _jobService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(
        IApplicationService applicationService,
        IJobService jobService,
        ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _jobService = jobService;
        _logger = logger;
    }

    // GET: Applications/MyApplications
    [HttpGet]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> MyApplications(int page = 1)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var pagedApplications = await _applicationService.GetApplicationsByApplicantAsync(userId, page: page, pageSize: 10);

        ViewBag.PagedResult = pagedApplications; // ✅ Pass pagination info to view
        return View(pagedApplications.Items); // ✅ Pass only Items to view
    }

    // GET: Applications/Apply/5
    [HttpGet]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Apply(int jobId)
    {
        var job = await _jobService.GetJobByIdAsync(jobId);
        if (job == null)
        {
            return NotFound();
        }

        // Check if already applied
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var hasApplied = await _applicationService.HasAppliedAsync(userId, jobId);

        if (hasApplied)
        {
            TempData["ErrorMessage"] = "Bạn đã ứng tuyển vào công việc này rồi!";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }

        ViewBag.Job = job;
        return View(new ApplicationDto { JobId = jobId });
    }

    // POST: Applications/Apply
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Apply(ApplicationDto model, IFormFile? cvFile)
    {
        if (!ModelState.IsValid)
        {
            var job = await _jobService.GetJobByIdAsync(model.JobId);
            ViewBag.Job = job;
            return View(model);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Handle CV upload
            if (cvFile != null && cvFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(cvFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }

                model.CvUrl = $"/uploads/cvs/{uniqueFileName}";
            }

            var application = await _applicationService.CreateApplicationAsync(userId, model);

            TempData["SuccessMessage"] = "Nộp đơn ứng tuyển thành công!";
            return RedirectToAction("MyApplications");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying for job {JobId}", model.JobId);
            ModelState.AddModelError(string.Empty, ex.Message);

            var job = await _jobService.GetJobByIdAsync(model.JobId);
            ViewBag.Job = job;
            return View(model);
        }
    }

    // GET: Applications/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);

        if (application == null)
        {
            return NotFound();
        }

        // Authorization check
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        // Applicant can only view their own applications
        if (userRole == "Applicant")
        {
            var myApplicationsPage = await _applicationService.GetApplicationsByApplicantAsync(userId, page: 1, pageSize: 1000);
            if (!myApplicationsPage.Items.Any(a => a.Id == id))
            {
                return Forbid();
            }
        }
        // Employer can only view applications for their jobs
        else if (userRole == "Employer")
        {
            var myApplicationsPage = await _applicationService.GetApplicationsByEmployerAsync(userId, page: 1, pageSize: 1000);
            if (!myApplicationsPage.Items.Any(a => a.Id == id))
            {
                return Forbid();
            }
        }

        return View(application);
    }

    // POST: Applications/Cancel/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _applicationService.CancelApplicationAsync(id, userId);

        if (!result)
        {
            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Không thể hủy đơn ứng tuyển" });
            }

            TempData["ErrorMessage"] = "Không thể hủy đơn ứng tuyển";
        }
        else
        {
            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Hủy đơn ứng tuyển thành công!" });
            }

            TempData["SuccessMessage"] = "Hủy đơn ứng tuyển thành công!";
        }

        return RedirectToAction("MyApplications");
    }

    // POST: Applications/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? note)
    {
        var result = await _applicationService.UpdateApplicationStatusAsync(id, status, note);

        if (!result)
        {
            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Không thể cập nhật trạng thái đơn ứng tuyển" });
            }

            TempData["ErrorMessage"] = "Không thể cập nhật trạng thái đơn ứng tuyển";
        }
        else
        {
            // Check if it's AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
            }

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
        }

        return RedirectToAction("Details", new { id });
    }

    // GET: Applications/ByEmployer
    [HttpGet]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> ByEmployer()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var applications = await _applicationService.GetApplicationsByEmployerAsync(userId);
        return View(applications);
    }
}
