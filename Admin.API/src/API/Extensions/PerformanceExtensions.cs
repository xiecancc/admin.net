/*
 * 文件名称: PerformanceExtensions.cs
 * 功能描述: 性能优化扩展方法，包括响应压缩、输出缓存、静态资源优化
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace API.Extensions;

/// <summary>
/// 性能优化扩展方法
/// <para>提供响应压缩、输出缓存、静态资源优化等功能</para>
/// </summary>
public static class PerformanceExtensions {
    /// <summary>
    /// 添加性能优化服务
    /// <para>包括响应压缩、输出缓存、响应缓存</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddPerformanceServices(this IServiceCollection services) {
        services.AddResponseCompression(options => {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                "application/json",
                "text/json",
                "application/javascript",
                "text/javascript",
                "text/css",
                "text/plain",
                "text/xml",
                "application/xml",
                "image/svg+xml"
            ]);
        });

        services.Configure<BrotliCompressionProviderOptions>(options => {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options => {
            options.Level = CompressionLevel.Optimal;
        });

        services.AddOutputCache(options => {
            options.AddBasePolicy(policy =>
                policy.Expire(TimeSpan.FromSeconds(60)));

            options.AddPolicy("NoCache", policy =>
                policy.NoCache());

            options.AddPolicy("ShortCache", policy =>
                policy.Expire(TimeSpan.FromSeconds(30)));

            options.AddPolicy("MediumCache", policy =>
                policy.Expire(TimeSpan.FromMinutes(5)));

            options.AddPolicy("LongCache", policy =>
                policy.Expire(TimeSpan.FromHours(1)));
        });

        services.AddResponseCaching();

        return services;
    }

    /// <summary>
    /// 使用性能优化中间件
    /// <para>包括响应压缩、输出缓存、响应缓存、静态资源优化</para>
    /// </summary>
    /// <param name="app">Web 应用程序</param>
    /// <returns>Web 应用程序</returns>
    public static WebApplication UsePerformanceMiddlewares(this WebApplication app) {
        app.UseResponseCompression();
        app.UseResponseCaching();
        app.UseOutputCache();

        return app;
    }

    /// <summary>
    /// 映射优化后的静态资源
    /// <para>使用 .NET 9+ MapStaticAssets 特性，提供构建时压缩和更好的缓存</para>
    /// </summary>
    /// <param name="app">Web 应用程序</param>
    /// <returns>Web 应用程序</returns>
    public static WebApplication MapOptimizedStaticAssets(this WebApplication app) {
        app.MapStaticAssets();
        return app;
    }
}
