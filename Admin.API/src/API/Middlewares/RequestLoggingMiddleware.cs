/*
 * 文件名称: RequestLoggingMiddleware.cs
 * 功能描述: 请求日志中间件，记录请求开始和结束信息
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Infrastructure.Shared.Contexts;
using Infrastructure.Shared.Utils;

namespace API.Middlewares;

/// <summary>
/// 请求日志中间件
/// <para>记录请求开始和结束信息</para>
/// </summary>
/// <param name="next">下一个中间件委托</param>
/// <param name="logger">日志记录器</param>
public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware>? logger) {
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="ctx">HTTP 上下文</param>
    /// <returns>任务</returns>
    public async Task InvokeAsync(HttpContext ctx) {
        var httpContextProvider = ctx.RequestServices.GetRequiredService<IHttpContextProvider>();
        var userContextProvider = ctx.RequestServices.GetRequiredService<IUserContextProvider>();
        var traceId = httpContextProvider.TraceId ?? ctx.TraceIdentifier;
        var requestId = httpContextProvider.RequestId ?? ctx.TraceIdentifier;
        var userId = userContextProvider.UserId?.ToString();
        var userAgent = httpContextProvider.UserAgent;
        var clientIp = httpContextProvider.ClientIp;
        var requestPath = httpContextProvider.RequestPath;
        var requestMethod = httpContextProvider.RequestMethod;

        if (requestPath is null || !requestPath.StartsWith("/api/")) {
            await _next(ctx);
            return;
        }

        ctx.Response.Headers["X-Trace-Id"] = traceId;
        ctx.Response.Headers["X-Request-Id"] = requestId;

        var startTime = DateTime.UtcNow.ToTimestampMs();

        if (logger != null) {
            using var scope = logger.BeginScope(new Dictionary<string, object?> {
                ["TraceId"] = traceId,
                ["RequestId"] = requestId,
                ["UserId"] = userId,
                ["ClientIp"] = clientIp,
            ["UserAgent"] = userAgent,
            ["RequestPath"] = requestPath,
            ["RequestMethod"] = requestMethod
        });

        logger?.LogInformation("请求开始: {RequestMethod} {RequestPath} | 客户端IP: {ClientIp} | 浏览器: {UserAgent}",
            requestMethod, requestPath, clientIp, userAgent);

        await _next(ctx);

        var responseTime = DateTime.UtcNow.ToTimestampMs() - startTime;

        logger?.LogInformation("请求结束: {RequestMethod} {RequestPath} | 状态码: {StatusCode} | 响应耗时: {ResponseTime:F2}ms",
            requestMethod, requestPath, ctx.Response.StatusCode, responseTime);
        }
    }
}
