using System.Security.Claims;

namespace JobExchangeMvc.Middlewares;

/// <summary>
/// Middleware kiểm tra quyền truy cập dựa trên role và path
/// Mục đích: Bảo vệ các route Admin, Employer
/// </summary>
public class RoleBasedAccessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RoleBasedAccessMiddleware> _logger;

    // Định nghĩa các route yêu cầu role cụ thể
    private static readonly Dictionary<string, string[]> ProtectedRoutes = new()
    {
        { "/Admin", new[] { "Admin" } },
        { "/Jobs/Create", new[] { "Employer", "Admin" } },
        { "/Jobs/Edit", new[] { "Employer", "Admin" } },
        { "/Jobs/Delete", new[] { "Employer", "Admin" } },
        { "/Jobs/MyJobs", new[] { "Employer", "Admin" } },
        { "/Jobs/Applicants", new[] { "Employer", "Admin" } },
        { "/Applications/MyApplications", new[] { "Applicant", "Admin" } },
        { "/Applications/Apply", new[] { "Applicant" } }
    };

    public RoleBasedAccessMiddleware(RequestDelegate next, ILogger<RoleBasedAccessMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // Kiểm tra nếu path cần bảo vệ
        var protectedRoute = ProtectedRoutes.FirstOrDefault(r =>
            path.StartsWith(r.Key, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(protectedRoute.Key))
        {
            var requiredRoles = protectedRoute.Value;

            // Kiểm tra user đã đăng nhập chưa
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning(
                    "[ACCESS DENIED] Unauthenticated user tried to access {Path}",
                    path);

                context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(path)}");
                return;
            }

            // Lấy role của user
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value;

            // Kiểm tra role có hợp lệ không
            if (string.IsNullOrEmpty(userRole) || !requiredRoles.Contains(userRole))
            {
                _logger.LogWarning(
                    "[ACCESS DENIED] User {UserId} ({UserName}) with role '{UserRole}' tried to access {Path} | Required roles: {RequiredRoles}",
                    userId, userName, userRole ?? "None", path, string.Join(", ", requiredRoles));

                // Redirect đến trang Access Denied
                context.Response.Redirect("/Account/AccessDenied");
                return;
            }

            _logger.LogInformation(
                "[ACCESS GRANTED ✅] User {UserId} ({UserName}) with role '{UserRole}' accessed {Path}",
                userId, userName, userRole, path);
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method để đăng ký middleware
/// </summary>
public static class RoleBasedAccessMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleBasedAccess(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleBasedAccessMiddleware>();
    }
}
