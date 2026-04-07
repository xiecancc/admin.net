/*
 * 文件名称: SerilogExtensions.cs
 * 功能描述: Serilog 日志扩展方法，配置 Serilog 日志服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Serilog;

namespace API.Extensions;

/// <summary>
/// Serilog 日志扩展方法
/// <para>配置 Serilog 日志服务</para>
/// </summary>
public static class SerilogExtensions {
    /// <summary>
    /// 为应用注册 Serilog 日志
    /// </summary>
    /// <param name="builder">Web 应用构建器</param>
    /// <returns>Web 应用构建器</returns>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder) {
        _ = builder.Host.UseSerilog((ctx, services, cfg) => {
            _ = cfg
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });
        return builder;
    }

    /// <summary>
    /// 配置请求日志中间件
    /// </summary>
    /// <param name="app">Web 应用</param>
    /// <returns>Web 应用</returns>
    public static WebApplication UseSerilogLogging(this WebApplication app) {
        _ = app.UseMiddleware<Middlewares.RequestLoggingMiddleware>();
        return app;
    }
}
