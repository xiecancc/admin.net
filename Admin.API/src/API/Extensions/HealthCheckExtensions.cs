/*
 * 文件名称: HealthCheckExtensions.cs
 * 功能描述: 健康检查扩展方法，使用 ASP.NET Core 10 增强的健康检查框架
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using System.Collections.Frozen;
using System.Text;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Options;
using Infrastructure.Shared.Utils;
using Infrastructure.Sugars;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace API.Extensions;

/// <summary>
/// 健康检查扩展方法
/// <para>使用 ASP.NET Core 10 增强的健康检查框架，支持多层级健康状态和依赖项检查</para>
/// </summary>
public static class HealthCheckExtensions {
    private static readonly FrozenDictionary<HealthStatus, int> HealthStatusCodes = new Dictionary<HealthStatus, int> {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }.ToFrozenDictionary();
    /// <summary>
    /// 添加健康检查服务
    /// <para>从已注册的 RedisOption 配置读取 Redis 是否启用</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>配置来源：appsettings.json 的 Redis 节点</para>
    /// <para>配置注册：在 Infrastructure 层的 DependencyExtensions 中统一注册</para>
    /// </remarks>
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services) {
        services.AddScoped<DatabaseHealthCheck>();
        services.AddScoped<MemoryHealthCheck>();
        services.AddScoped<RedisHealthCheck>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: ["database", "infrastructure"])
            .AddCheck<MemoryHealthCheck>("memory", tags: ["system", "memory"]);

        services.ConfigureOptions<ConfigureHealthCheckOptions>();
        return services;
    }

    /// <summary>
    /// 配置健康检查端点
    /// </summary>
    /// <param name="app">Web 应用程序</param>
    /// <returns>Web 应用程序</returns>
    public static WebApplication UseHealthCheckEndpoints(this WebApplication app) {
        app.MapHealthChecks("/health", new HealthCheckOptions {
            ResponseWriter = WriteHealthCheckResponse,
            AllowCachingResponses = false,
            ResultStatusCodes = HealthStatusCodes
        });

        return app;
    }

    /// <summary>
    /// 自定义健康检查响应写入器
    /// </summary>
    private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report) {
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.TotalMilliseconds,
                tags = entry.Value.Tags,
                data = entry.Value.Data
            }),
            timestamp = DateTime.UtcNow
        };

        var json = JsonUtil.Serialize(response);
        return context.Response.WriteAsync(json, Encoding.UTF8);
    }
}

/// <summary>
/// 配置健康检查选项
/// <para>从 DI 容器注入 RedisOption 配置，动态添加 Redis 健康检查</para>
/// </summary>
public sealed class ConfigureHealthCheckOptions(IOptions<RedisOption> redisOption) : IConfigureOptions<HealthCheckServiceOptions> {
    private readonly RedisOption _redisOption = redisOption.Value;

    /// <summary>
    /// 配置健康检查选项
    /// </summary>
    public void Configure(HealthCheckServiceOptions options) {
        if (_redisOption.Enabled) {
            options.Registrations.Add(new HealthCheckRegistration(
                "redis",
                sp => sp.GetRequiredService<RedisHealthCheck>(),
                null,
                ["redis", "cache"]));
        }
    }
}

/// <summary>
/// 数据库健康检查
/// </summary>
public class DatabaseHealthCheck(SugarDb sugarDb) : IHealthCheck {
    /// <summary>
    /// 检查健康状态
    /// </summary>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) {
        try {
            var db = sugarDb.GetClient();
            var canConnect = db.Ado.GetDataTable("SELECT 1") != null;

            return canConnect ? Task.FromResult(HealthCheckResult.Healthy("数据库连接正常")) : Task.FromResult(HealthCheckResult.Unhealthy("数据库连接测试失败"));
        }
#pragma warning disable CA1031 // 健康检查需要捕获所有异常以返回正确的健康状态
        catch (Exception ex) {
            return Task.FromResult(HealthCheckResult.Unhealthy("数据库健康检查失败", ex));
        }
#pragma warning restore CA1031
    }
}

/// <summary>
/// Redis 健康检查
/// </summary>
public class RedisHealthCheck(IRedisConnectionManager redisConnection) : IHealthCheck {
    /// <summary>
    /// 检查健康状态
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) {
        try {
            if (!redisConnection.IsConnected) {
                return HealthCheckResult.Degraded("Redis 连接不稳定");
            }

            var database = redisConnection.GetDatabase();
            await database.PingAsync();

            return HealthCheckResult.Healthy("Redis 连接正常");
        }
#pragma warning disable CA1031 // 健康检查需要捕获所有异常以返回正确的健康状态
        catch (Exception ex) {
            return HealthCheckResult.Degraded("Redis 连接失败，已降级运行", ex);
        }
#pragma warning restore CA1031
    }
}

/// <summary>
/// 内存健康检查
/// </summary>
public class MemoryHealthCheck : IHealthCheck {
    private const long WARNING_THRESHOLD_BYTES = 1024 * 1024 * 512;
    private const long CRITICAL_THRESHOLD_BYTES = 1024 * 1024 * 1024;

    /// <summary>
    /// 检查健康状态
    /// </summary>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) {
        try {
            var allocated = GC.GetTotalMemory(false);
            var data = new Dictionary<string, object> {
                ["AllocatedBytes"] = allocated,
                ["AllocatedMB"] = allocated / (1024 * 1024),
                ["Gen0Collections"] = GC.CollectionCount(0),
                ["Gen1Collections"] = GC.CollectionCount(1),
                ["Gen2Collections"] = GC.CollectionCount(2),
                ["WarningThresholdMB"] = WARNING_THRESHOLD_BYTES / (1024 * 1024),
                ["CriticalThresholdMB"] = CRITICAL_THRESHOLD_BYTES / (1024 * 1024)
            };

            if (allocated >= CRITICAL_THRESHOLD_BYTES) {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"内存使用过高: {allocated / (1024 * 1024)}MB 超过临界阈值", null, data));
            }

            return allocated >= WARNING_THRESHOLD_BYTES
                ? Task.FromResult(HealthCheckResult.Degraded(
                    $"内存使用较高: {allocated / (1024 * 1024)}MB 超过警告阈值", null, data))
                : Task.FromResult(HealthCheckResult.Healthy(
                $"内存使用正常: {allocated / (1024 * 1024)}MB", data));
        }
#pragma warning disable CA1031 // 健康检查需要捕获所有异常以返回正确的健康状态
        catch (Exception ex) {
            return Task.FromResult(HealthCheckResult.Unhealthy("内存健康检查失败", ex));
        }
#pragma warning restore CA1031
    }
}
