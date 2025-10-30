using System.Security.Claims;
using JobExchangeMvc.Helpers;

namespace JobExchangeMvc.Middlewares;

/// <summary>
/// Middleware tự động lấy JWT từ cookie và set vào HttpContext.User
/// Mục đích: Kết nối Cookie-based authentication với JWT
/// </summary>
public class JwtCookieMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtCookieMiddleware> _logger;

    public JwtCookieMiddleware(RequestDelegate next, ILogger<JwtCookieMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, JwtTokenHelper jwtTokenHelper)
    {
        // Kiểm tra nếu user chưa authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            // Lấy access token từ cookie
            var accessToken = context.Request.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    // Validate và lấy claims từ JWT
                    var principal = jwtTokenHelper.GetPrincipalFromToken(accessToken);

                    if (principal != null)
                    {
                        // Set user vào HttpContext
                        context.User = principal;

                        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
                        var userRole = principal.FindFirst(ClaimTypes.Role)?.Value;

                        _logger.LogInformation(
                            "[JWT AUTH✅] User authenticated from cookie | UserId: {UserId} | Name: {UserName} | Role: {UserRole}",
                            userId, userName, userRole);
                    }
                    else
                    {
                        _logger.LogWarning("[JWT AUTH ✅] Invalid JWT token in cookie");
                        
                        // Xóa cookie không hợp lệ
                        context.Response.Cookies.Delete("AccessToken");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[JWT AUTH] Error validating JWT from cookie: {Message}", ex.Message);
                    
                    // Xóa cookie lỗi
                    context.Response.Cookies.Delete("AccessToken");
                }
            }
            else
            {
                _logger.LogDebug("[JWT AUTH] No AccessToken cookie found");
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method để đăng ký middleware
/// </summary>
public static class JwtCookieMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtCookie(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtCookieMiddleware>();
    }
}
