/*
 * 文件名称: CorsExtensions.cs
 * 功能描述: 跨域扩展方法，配置跨域资源共享（CORS）服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Infrastructure.Shared.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace API.Extensions;

/// <summary>
/// 跨域扩展方法
/// <para>配置跨域资源共享（CORS）服务</para>
/// </summary>
public static class CorsExtensions {
    /// <summary>
    /// 添加跨域服务
    /// <para>从已注册的 CorsOption 配置读取跨域策略</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>配置来源：appsettings.json 的 Cors 节点</para>
    /// <para>配置注册：在 Infrastructure 层的 DependencyExtensions 中统一注册</para>
    /// </remarks>
    public static IServiceCollection AddCorsServices(this IServiceCollection services) {
        _ = services.ConfigureOptions<ConfigureCorsOptions>();
        return services;
    }
}

/// <summary>
/// 配置跨域选项
/// <para>从 DI 容器注入 CorsOption 配置</para>
/// </summary>
public sealed class ConfigureCorsOptions(IOptions<CorsOption> corsOption) : IConfigureOptions<CorsOptions> {
    private readonly CorsOption _corsOption = corsOption.Value;

    /// <summary>
    /// 配置跨域选项
    /// </summary>
    public void Configure(CorsOptions options) {
        options.AddDefaultPolicy(builder => {
            _ = _corsOption.AllowAnyOrigin ? builder.AllowAnyOrigin() : builder.WithOrigins([.. _corsOption.AllowedOrigins]);

            _ = _corsOption.AllowedMethods.Count == 0 ? builder.AllowAnyMethod() : builder.WithMethods([.. _corsOption.AllowedMethods]);

            _ = _corsOption.AllowedHeaders.Count == 0 ? builder.AllowAnyHeader() : builder.WithHeaders([.. _corsOption.AllowedHeaders]);

            if (_corsOption.AllowCredentials) {
                _ = builder.AllowCredentials();
            }

            if (_corsOption.ExposedHeaders.Count > 0) {
                _ = builder.WithExposedHeaders([.. _corsOption.ExposedHeaders]);
            }

            if (_corsOption.PreflightMaxAge > 0) {
                _ = builder.SetPreflightMaxAge(TimeSpan.FromSeconds(_corsOption.PreflightMaxAge));
            }
        });
    }
}
