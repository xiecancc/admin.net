/*
 * 文件名称: TelemetryExtensions.cs
 * 功能描述: OpenTelemetry 分布式追踪扩展方法
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Infrastructure.Shared.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace API.Extensions;

/// <summary>
/// OpenTelemetry 遥测扩展方法
/// <para>提供分布式追踪和指标收集功能</para>
/// </summary>
public static class TelemetryExtensions {
    /// <summary>
    /// 添加 OpenTelemetry 遥测服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置信息</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>注意：OpenTelemetry 配置方式特殊，不支持 IOptions 延迟配置模式</para>
    /// <para>配置来源：appsettings.json 的 Telemetry 节点</para>
    /// </remarks>
    public static IServiceCollection AddTelemetryServices(this IServiceCollection services, IConfiguration configuration) {
        var telemetryOption = configuration.GetSection("Telemetry").Get<TelemetryOption>() ?? new TelemetryOption();

        if (!telemetryOption.Enabled) {
            return services;
        }

        services.AddOpenTelemetry()
            .WithTracing(tracing => {
                tracing
                    .AddAspNetCoreInstrumentation(options => {
                        options.RecordException = true;
                        options.Filter = httpContext => {
                            var path = httpContext.Request.Path.Value ?? string.Empty;
                            return !path.Contains("/health", StringComparison.OrdinalIgnoreCase) &&
                                   !path.Contains("/swagger", StringComparison.OrdinalIgnoreCase);
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation();

                if (!string.IsNullOrEmpty(telemetryOption.OtlpEndpoint)) {
                    tracing.AddOtlpExporter(exporterOptions => {
                        exporterOptions.Endpoint = new Uri(telemetryOption.OtlpEndpoint);
                    });
                }
            })
            .WithMetrics(metrics => {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (!string.IsNullOrEmpty(telemetryOption.OtlpEndpoint)) {
                    metrics.AddOtlpExporter(exporterOptions => {
                        exporterOptions.Endpoint = new Uri(telemetryOption.OtlpEndpoint);
                    });
                }
            });

        services.AddSingleton<Instrumentation>();

        return services;
    }
}

/// <summary>
/// 自定义指标和追踪工具
/// </summary>
public sealed class Instrumentation {
    /// <summary>
    /// 指标名称
    /// </summary>
    public const string METER_NAME = "Admin.NET";

    /// <summary>
    /// 活动源名称
    /// </summary>
    public const string ACTIVITY_SOURCE_NAME = "Admin.NET";

    /// <summary>
    /// 活动源
    /// </summary>
    public ActivitySource ActivitySource {
        get;
    }

    /// <summary>
    /// 计量器
    /// </summary>
    public Meter Meter {
        get;
    }

    /// <summary>
    /// 请求计数器
    /// </summary>
    public Counter<long> RequestCounter {
        get;
    }

    /// <summary>
    /// 请求持续时间直方图
    /// </summary>
    public Histogram<double> RequestDuration {
        get;
    }

    /// <summary>
    /// 初始化遥测工具
    /// </summary>
    public Instrumentation() {
        ActivitySource = new ActivitySource(ACTIVITY_SOURCE_NAME);
        Meter = new Meter(METER_NAME);
        RequestCounter = Meter.CreateCounter<long>("requests_total", "次", "请求总数");
        RequestDuration = Meter.CreateHistogram<double>("request_duration_ms", "毫秒", "请求持续时间");
    }
}
