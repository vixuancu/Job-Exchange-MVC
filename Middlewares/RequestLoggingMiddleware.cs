using System.Diagnostics;

namespace JobExchangeMvc.Middlewares;

/// <summary>
/// Middleware ghi log thông tin request: URL, method, thời gian xử lý, user
/// Mục đích: Giám sát performance và debug
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Bắt đầu đếm thời gian
        var stopwatch = Stopwatch.StartNew();

        // Lấy thông tin request
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.ToString();
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userName = context.User.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "Anonymous";

        // Log request bắt đầu
        _logger.LogInformation(
            "[REQUEST START ✅] {Method} {Path}{QueryString} | User: {UserName} | IP: {IpAddress}",
            method, path, queryString, userName, ipAddress);

        try
        {
            // Gọi middleware tiếp theo trong pipeline
            await _next(context);

            // Dừng đếm thời gian
            stopwatch.Stop();

            // Lấy status code
            var statusCode = context.Response.StatusCode;

            // Log request hoàn thành
            if (statusCode >= 200 && statusCode < 400)
            {
                _logger.LogInformation(
                    "[REQUEST SUCCESS✅] {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | User: {UserName}",
                    method, path, statusCode, stopwatch.ElapsedMilliseconds, userName);
            }
            else if (statusCode >= 400 && statusCode < 500)
            {
                _logger.LogWarning(
                    "[REQUEST CLIENT ERROR ✅] {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | User: {UserName}",
                    method, path, statusCode, stopwatch.ElapsedMilliseconds, userName);
            }
            else if (statusCode >= 500)
            {
                _logger.LogError(
                    "[REQUEST SERVER ERROR] {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | User: {UserName}",
                    method, path, statusCode, stopwatch.ElapsedMilliseconds, userName);
            }

            // Cảnh báo nếu request chậm (> 3 giây)
            if (stopwatch.ElapsedMilliseconds > 3000)
            {
                _logger.LogWarning(
                    "[SLOW REQUEST ✅] {Method} {Path} took {Duration}ms | User: {UserName}",
                    method, path, stopwatch.ElapsedMilliseconds, userName);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log exception
            _logger.LogError(ex,
                "[REQUEST EXCEPTION] {Method} {Path} | Duration: {Duration}ms | User: {UserName} | Error: {ErrorMessage}",
                method, path, stopwatch.ElapsedMilliseconds, userName, ex.Message);

            // Re-throw để ExceptionHandler middleware xử lý
            throw;
        }
    }
}

/// <summary>
/// Extension method để dễ dàng đăng ký middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
