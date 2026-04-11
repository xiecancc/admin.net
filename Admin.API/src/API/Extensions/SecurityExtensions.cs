/*
 * 文件名称: SecurityExtensions.cs
 * 功能描述: 安全性扩展方法，使用 ASP.NET Core 10 安全增强特性
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace API.Extensions;

/// <summary>
/// 安全性扩展方法
/// <para>使用 ASP.NET Core 10 安全增强特性，添加安全头部和 HSTS</para>
/// </summary>
public static class SecurityExtensions {
    /// <summary>
    /// 添加安全头部中间件
    /// </summary>
    /// <param name="app">Web 应用程序</param>
    /// <returns>Web 应用程序</returns>
    public static WebApplication UseSecurityHeaders(this WebApplication app) {
        app.Use(async (context, next) => {
            context.Response.Headers.XFrameOptions = "DENY";
            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers.XXSSProtection = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy")) {
                context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' data:; " +
                    "connect-src 'self'; " +
                    "frame-ancestors 'none';";
            }

            await next();
        });

        return app;
    }
}
