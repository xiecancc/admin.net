/*
 * 文件名称: RequestConfigExtensions.cs
 * 功能描述: 请求配置扩展方法，包括请求超时和请求体大小限制
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Infrastructure.Shared.Options;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace API.Extensions;

/// <summary>
/// 请求配置扩展方法
/// <para>提供请求超时和请求体大小限制功能</para>
/// </summary>
public static class RequestConfigExtensions {
    /// <summary>
    /// 添加请求超时服务
    /// <para>使用 ASP.NET Core 8+ 内置请求超时中间件</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRequestTimeoutServices(this IServiceCollection services, IConfiguration configuration) {
        var timeoutOption = configuration.GetSection("RequestTimeout").Get<RequestTimeoutOption>() ?? new RequestTimeoutOption();

        if (!timeoutOption.Enabled) {
            return services;
        }

        _ = services.AddRequestTimeouts(options => {
            options.DefaultPolicy = new RequestTimeoutPolicy {
                Timeout = TimeSpan.FromSeconds(timeoutOption.DefaultTimeoutSeconds)
            };

            foreach (var policy in timeoutOption.EndpointPolicies) {
                _ = options.AddPolicy(policy.Name, new RequestTimeoutPolicy {
                    Timeout = TimeSpan.FromSeconds(policy.TimeoutSeconds)
                });
            }
        });

        return services;
    }

    /// <summary>
    /// 添加请求体大小限制服务
    /// <para>使用 ASP.NET Core 内置大小限制特性</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddRequestSizeLimitServices(this IServiceCollection services, IConfiguration configuration) {
        var sizeLimitOption = configuration.GetSection("RequestSizeLimit").Get<RequestSizeLimitOption>() ?? new RequestSizeLimitOption();

        if (!sizeLimitOption.Enabled) {
            return services;
        }

        _ = services.Configure<KestrelServerOptions>(options => {
            options.Limits.MaxRequestBodySize = sizeLimitOption.MaxRequestBodySizeBytes;
        });

        _ = services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options => {
            options.MultipartBodyLengthLimit = sizeLimitOption.MultipartBodyLengthLimitBytes;
            options.ValueLengthLimit = (int)sizeLimitOption.MultipartBodyLengthLimitBytes;
        });

        return services;
    }

    /// <summary>
    /// 使用请求超时中间件
    /// </summary>
    /// <param name="app">Web 应用程序</param>
    /// <returns>Web 应用程序</returns>
    public static WebApplication UseRequestTimeoutMiddleware(this WebApplication app) {
        _ = app.UseRequestTimeouts();
        return app;
    }
}
