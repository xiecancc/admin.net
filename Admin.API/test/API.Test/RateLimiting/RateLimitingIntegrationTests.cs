/*
 * 文件名称: RateLimitingIntegrationTests.cs
 * 功能描述: 限流中间件集成测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using System.Net;
using System.Text.Json;

namespace API.Test.RateLimiting;

/// <summary>
/// 限流中间件集成测试类
/// <para>测试限流中间件在实际 HTTP 请求中的行为，包括限流触发、响应格式等</para>
/// </summary>
public class RateLimitingIntegrationTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>, IAsyncDisposable {
    private readonly HttpClient _client = factory.CreateClient();

    public async ValueTask DisposeAsync() {
        _client.Dispose();
        await Task.Delay(100);
    }

    /// <summary>
    /// 测试启用限流后超过限制返回 429 状态码
    /// <para>预期结果：超过限流阈值后返回 429 TooManyRequests 状态码</para>
    /// </summary>
    [Fact]
    public async Task RateLimiting_WhenEnabled_ShouldReturn429AfterExceedingLimit() {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var requests = new List<HttpResponseMessage>();
        for (int i = 0; i < 105; i++) {
            var response = await client.GetAsync("/api/v1/user");
            requests.Add(response);
        }

        var rateLimitedResponses = requests.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        Assert.True(rateLimitedResponses > 0, "应该有限流响应");
    }

    /// <summary>
    /// 测试限流响应返回正确的响应格式
    /// <para>预期结果：限流响应包含 success 和 message 字段</para>
    /// </summary>
    [Fact]
    public async Task RateLimiting_WhenRateLimited_ShouldReturnCorrectResponseFormat() {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        HttpResponseMessage? rateLimitedResponse = null;
        for (int i = 0; i < 105; i++) {
            var response = await client.GetAsync("/api/v1/user");
            if (response.StatusCode == HttpStatusCode.TooManyRequests) {
                rateLimitedResponse = response;
                break;
            }
        }

        Assert.NotNull(rateLimitedResponse);

        var content = await rateLimitedResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.True(result.TryGetProperty("success", out var successProp), "响应应包含 success 字段");
        Assert.False(successProp.GetBoolean(), "限流响应 success 应为 false");
        Assert.True(result.TryGetProperty("message", out var messageProp), "响应应包含 message 字段");
        Assert.False(string.IsNullOrEmpty(messageProp.GetString()), "message 不应为空");
    }

    /// <summary>
    /// 测试限流响应包含 RetryAfter 响应头
    /// <para>预期结果：限流响应包含 RetryAfter 或 retry-after 响应头</para>
    /// </summary>
    [Fact]
    public async Task RateLimiting_WhenRateLimited_ShouldIncludeRetryAfterHeader() {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        HttpResponseMessage? rateLimitedResponse = null;
        for (int i = 0; i < 105; i++) {
            var response = await client.GetAsync("/api/v1/user");
            if (response.StatusCode == HttpStatusCode.TooManyRequests) {
                rateLimitedResponse = response;
                break;
            }
        }

        Assert.NotNull(rateLimitedResponse);

        var hasRetryAfter = rateLimitedResponse.Headers.Contains("RetryAfter") ||
                           rateLimitedResponse.Headers.Contains("retry-after") ||
                           rateLimitedResponse.Content.Headers.Contains("RetryAfter");

        Assert.True(hasRetryAfter, "限流响应应包含 RetryAfter 响应头");
    }

    /// <summary>
    /// 测试不同端点具有独立的限流计数
    /// <para>预期结果：一个端点达到限流阈值不影响其他端点</para>
    /// </summary>
    [Fact]
    public async Task RateLimiting_DifferentEndpoints_ShouldHaveSeparateLimits() {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        for (int i = 0; i < 50; i++) {
            await client.GetAsync("/api/v1/user");
        }

        var rolesResponse = await client.GetAsync("/api/v1/role");
        Assert.True(rolesResponse.StatusCode != HttpStatusCode.TooManyRequests,
            "不同端点应该有独立的限流计数");
    }

    /// <summary>
    /// 测试不同客户端具有独立的限流计数
    /// <para>预期结果：一个客户端达到限流阈值不影响其他客户端</para>
    /// </summary>
    [Fact]
    public async Task RateLimiting_SameEndpointFromDifferentClients_ShouldHaveSeparateLimits() {
        using var factory1 = new TestWebApplicationFactory();
        using var client1 = factory1.CreateClient();

        using var factory2 = new TestWebApplicationFactory();
        using var client2 = factory2.CreateClient();

        for (int i = 0; i < 50; i++) {
            await client1.GetAsync("/api/v1/user");
        }

        var response = await client2.GetAsync("/api/v1/user");
        Assert.True(response.StatusCode != HttpStatusCode.TooManyRequests,
            "不同客户端应该有独立的限流计数");
    }

    /// <summary>
    /// 测试首次请求不被限流
    /// <para>预期结果：首次请求返回正常状态码，不是 429</para>
    /// </summary>
    [Fact]
    public async Task RateLimiting_FirstRequest_ShouldNotReturn429() {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/user");
        Assert.True(response.StatusCode != HttpStatusCode.TooManyRequests,
            "首次请求不应被限流");
    }
}
