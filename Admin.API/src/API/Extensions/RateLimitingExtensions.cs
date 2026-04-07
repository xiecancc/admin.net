/*
 * 文件名称：RateLimitingExtensions.cs
 * 功能描述：请求限流扩展方法，配置 ASP.NET Core 10.0 内置限流中间件
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-06
 */

using System.Threading.RateLimiting;
using Infrastructure.Shared.Options;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace API.Extensions;

/// <summary>
/// 请求限流扩展方法
/// <para>配置 ASP.NET Core 10.0 内置限流中间件</para>
/// </summary>
/// <remarks>
/// <para>功能说明：</para>
/// <list type="bullet">
///   <item>支持多种限流算法：固定窗口、滑动窗口、令牌桶、并发限流</item>
///   <item>支持多种分区策略：用户、IP、全局、请求头</item>
///   <item>支持端点级别限流策略</item>
///   <item>支持自定义拒绝响应</item>
///   <item>支持配置启用/禁用</item>
/// </list>
/// </remarks>
public static class RateLimitingExtensions {
    /// <summary>
    /// 添加限流服务
    /// <para>从已注册的 RateLimitOption 配置读取限流策略</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>配置来源：appsettings.json 的 RateLimit 节点</para>
    /// <para>配置注册：在 Infrastructure 层的 DependencyExtensions 中统一注册</para>
    /// </remarks>
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services) {
        _ = services.ConfigureOptions<ConfigureRateLimiterOptions>();
        return services;
    }
}

/// <summary>
/// 配置限流选项
/// <para>从 DI 容器注入 RateLimitOption 配置</para>
/// </summary>
public sealed class ConfigureRateLimiterOptions(IOptions<RateLimitOption> rateLimitOption) : IConfigureOptions<RateLimiterOptions> {
    private readonly RateLimitOption _rateLimitOption = rateLimitOption.Value;

    /// <summary>
    /// 配置限流选项
    /// </summary>
    public void Configure(RateLimiterOptions options) {
        if (!_rateLimitOption.Enabled) {
            return;
        }

        ConfigureRateLimitOptions(options);

        options.OnRejected = async (context, cancellationToken) => {
            await OnRejectedAsync(context, cancellationToken);
        };
    }

    private void ConfigureRateLimitOptions(RateLimiterOptions options) {
        options.RejectionStatusCode = _rateLimitOption.OnRejected?.StatusCode ?? 429;

        if (_rateLimitOption.EndpointPolicies != null) {
            foreach (var endpointPolicy in _rateLimitOption.EndpointPolicies) {
                var policyName = string.IsNullOrWhiteSpace(endpointPolicy.PolicyName)
                    ? $"{endpointPolicy.Method}_{endpointPolicy.PathPattern}"
                    : endpointPolicy.PolicyName;

                AddRateLimitPolicy(options, policyName, endpointPolicy);
            }
        }

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => {
            var partitionKey = GetPartitionKey(httpContext);
            var policy = _rateLimitOption.DefaultPolicy;

            return CreateRateLimitPartition(partitionKey, policy);
        });
    }

    private void AddRateLimitPolicy(RateLimiterOptions options, string policyName, EndpointRateLimitOption endpointPolicy) {
        _ = options.AddPolicy(policyName, httpContext => {
            var partitionKey = GetPartitionKey(httpContext);
            return CreateRateLimitPartition(partitionKey, endpointPolicy);
        });
    }

    private RateLimitPartition<string> CreateRateLimitPartition(string partitionKey, RateLimitPolicyOption policy) {
        return policy.AlgorithmEnum switch {
            RateLimitAlgorithmType.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions {
                    PermitLimit = policy.PermitLimit,
                    Window = TimeSpan.FromSeconds(policy.WindowSeconds),
                    QueueProcessingOrder = GetQueueProcessingOrder(policy.QueueProcessingOrderEnum),
                    QueueLimit = policy.QueueLimit
                }),

            RateLimitAlgorithmType.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey,
                _ => new SlidingWindowRateLimiterOptions {
                    PermitLimit = policy.PermitLimit,
                    Window = TimeSpan.FromSeconds(policy.WindowSeconds),
                    SegmentsPerWindow = policy.SegmentsPerWindow > 0 ? policy.SegmentsPerWindow : 1,
                    QueueProcessingOrder = GetQueueProcessingOrder(policy.QueueProcessingOrderEnum),
                    QueueLimit = policy.QueueLimit
                }),

            RateLimitAlgorithmType.TokenBucket => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey,
                _ => new TokenBucketRateLimiterOptions {
                    TokenLimit = policy.PermitLimit,
                    QueueProcessingOrder = GetQueueProcessingOrder(policy.QueueProcessingOrderEnum),
                    QueueLimit = policy.QueueLimit,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(policy.WindowSeconds),
                    TokensPerPeriod = policy.TokensPerPeriod > 0 ? policy.TokensPerPeriod : policy.PermitLimit
                }),

            RateLimitAlgorithmType.Concurrency => RateLimitPartition.GetConcurrencyLimiter(
                partitionKey,
                _ => new ConcurrencyLimiterOptions {
                    PermitLimit = policy.PermitLimit,
                    QueueProcessingOrder = GetQueueProcessingOrder(policy.QueueProcessingOrderEnum),
                    QueueLimit = policy.QueueLimit
                }),

            _ => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions {
                    PermitLimit = policy.PermitLimit,
                    Window = TimeSpan.FromSeconds(policy.WindowSeconds),
                    QueueProcessingOrder = GetQueueProcessingOrder(policy.QueueProcessingOrderEnum),
                    QueueLimit = policy.QueueLimit
                })
        };
    }

    private RateLimitPartition<string> CreateRateLimitPartition(string partitionKey, EndpointRateLimitOption endpointPolicy) {
        return endpointPolicy.AlgorithmEnum switch {
            RateLimitAlgorithmType.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions {
                    PermitLimit = endpointPolicy.PermitLimit,
                    Window = TimeSpan.FromSeconds(endpointPolicy.WindowSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = endpointPolicy.QueueLimit
                }),

            RateLimitAlgorithmType.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey,
                _ => new SlidingWindowRateLimiterOptions {
                    PermitLimit = endpointPolicy.PermitLimit,
                    Window = TimeSpan.FromSeconds(endpointPolicy.WindowSeconds),
                    SegmentsPerWindow = endpointPolicy.SegmentsPerWindow > 0 ? endpointPolicy.SegmentsPerWindow : 1,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = endpointPolicy.QueueLimit
                }),

            RateLimitAlgorithmType.TokenBucket => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey,
                _ => new TokenBucketRateLimiterOptions {
                    TokenLimit = endpointPolicy.PermitLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = endpointPolicy.QueueLimit,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(endpointPolicy.WindowSeconds),
                    TokensPerPeriod = endpointPolicy.TokensPerPeriod > 0 ? endpointPolicy.TokensPerPeriod : endpointPolicy.PermitLimit
                }),

            RateLimitAlgorithmType.Concurrency => RateLimitPartition.GetConcurrencyLimiter(
                partitionKey,
                _ => new ConcurrencyLimiterOptions {
                    PermitLimit = endpointPolicy.PermitLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = endpointPolicy.QueueLimit
                }),

            _ => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions {
                    PermitLimit = endpointPolicy.PermitLimit,
                    Window = TimeSpan.FromSeconds(endpointPolicy.WindowSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = endpointPolicy.QueueLimit
                })
        };
    }

    private string GetPartitionKey(HttpContext httpContext) {
        return _rateLimitOption.PartitionTypeEnum switch {
            RateLimitPartitionType.User => GetUserPartitionKey(httpContext),
            RateLimitPartitionType.Ip => GetIpPartitionKey(httpContext),
            RateLimitPartitionType.Global => "global",
            RateLimitPartitionType.Header => GetHeaderPartitionKey(httpContext, _rateLimitOption.HeaderName!),
            _ => GetIpPartitionKey(httpContext)
        };
    }

    private static string GetUserPartitionKey(HttpContext httpContext) {
        var userId = httpContext.User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userId)) {
            userId = httpContext.User.FindFirst("sub")?.Value;
        }

        return userId ?? string.Empty;
    }

    private static string GetIpPartitionKey(HttpContext httpContext) {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(forwardedFor)) {
            var ip = forwardedFor.Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrWhiteSpace(ip)) {
                return ip;
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string GetHeaderPartitionKey(HttpContext httpContext, string headerName) {
        return httpContext.Request.Headers[headerName].FirstOrDefault() ?? string.Empty;
    }

    private static QueueProcessingOrder GetQueueProcessingOrder(RateLimitQueueProcessingOrder queueProcessingOrder) {
        return queueProcessingOrder switch {
            RateLimitQueueProcessingOrder.OldestFirst => QueueProcessingOrder.OldestFirst,
            RateLimitQueueProcessingOrder.NewestFirst => QueueProcessingOrder.NewestFirst,
            _ => QueueProcessingOrder.OldestFirst
        };
    }

    private async Task OnRejectedAsync(OnRejectedContext context, CancellationToken cancellationToken) {
        var statusCode = _rateLimitOption.OnRejected?.StatusCode ?? 429;
        var message = _rateLimitOption.OnRejected?.Message ?? "请求过于频繁，请稍后再试";

        context.HttpContext.Response.StatusCode = statusCode;

        var retryAfter = GetRetryAfter(context.Lease);
        if (retryAfter.HasValue) {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.Value.ToString();
        }

        await context.HttpContext.Response.WriteAsJsonAsync(new {
            Success = false,
            Message = message
        }, cancellationToken);
    }

    private static int? GetRetryAfter(RateLimitLease? lease) {
        if (lease == null) {
            return null;
        }

        return lease.TryGetMetadata("RETRY_AFTER", out var retryAfter) && retryAfter is TimeSpan retryAfterSpan
            ? (int)retryAfterSpan.TotalSeconds
            : null;
    }
}
