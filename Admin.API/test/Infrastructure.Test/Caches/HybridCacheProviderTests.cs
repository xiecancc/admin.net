/*
 * 文件名称: HybridCacheProviderTests.cs
 * 功能描述: 混合缓存提供者测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using System.Net;
using Infrastructure.Caches;
using Domain.Shared.Caches;
using Infrastructure.Shared.Options;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;

namespace Infrastructure.Test.Caches;

/// <summary>
/// 混合缓存提供者测试类
/// <para>测试缓存策略和缓存击穿保护功能</para>
/// </summary>
public class HybridCacheProviderTests {
    private readonly Mock<IRedisConnectionManager> _mockRedisConnection;
    private readonly Mock<ILogger<HybridCacheProvider>> _mockLogger;
    private readonly HybridCacheOption _hybridCacheOptions;
    private readonly RedisOption _redisOptions;

    public HybridCacheProviderTests() {
        _mockRedisConnection = new Mock<IRedisConnectionManager>();
        _mockLogger = new Mock<ILogger<HybridCacheProvider>>();

        _hybridCacheOptions = new HybridCacheOption {
            DefaultExpirationMinutes = 5,
            EnableStatistics = true,
            EnableAvalancheProtection = true,
            AvalancheProtectionMaxOffsetSeconds = 30,
            StampedeProtectionTimeoutMs = 5000,
            MaxConcurrentFactoryCalls = 100
        };

        _redisOptions = new RedisOption {
            Enabled = true,
            ConnectionString = "localhost:6379",
            InstanceName = "Test_"
        };
    }

    #region GetAsync Tests

    /// <summary>
    /// 测试获取缓存 - 缓存命中
    /// <para>场景：缓存中存在数据</para>
    /// <para>预期：返回缓存数据</para>
    /// </summary>
    [Fact]
    public async Task GetAsync_WithCacheHit_ShouldReturnCachedValue() {
        var cacheKey = "test:key";
        var expectedValue = "test-value";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        mockHybridCache.Setup(x => x.GetOrCreateAsync(
            cacheKey,
            It.IsAny<Func<CancellationToken, ValueTask<string?>>>(),
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        var result = await provider.GetAsync<string>(cacheKey);

        Assert.Equal(expectedValue, result);
    }

    /// <summary>
    /// 测试获取缓存 - 缓存未命中
    /// <para>场景：缓存中不存在数据</para>
    /// <para>预期：返回默认值</para>
    /// </summary>
    [Fact]
    public async Task GetAsync_WithCacheMiss_ShouldReturnDefault() {
        var cacheKey = "test:missing-key";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        mockHybridCache.Setup(x => x.GetOrCreateAsync(
            cacheKey,
            It.IsAny<Func<CancellationToken, ValueTask<string?>>>(),
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var result = await provider.GetAsync<string>(cacheKey);

        Assert.Null(result);
    }

    /// <summary>
    /// 测试获取缓存 - 空键
    /// <para>场景：传入空的缓存键</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetAsync_WithEmptyKey_ShouldThrowArgumentException(string key) {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => provider.GetAsync<string>(key));
    }

    /// <summary>
    /// 测试获取缓存 - null 键
    /// <para>场景：传入 null 的缓存键</para>
    /// <para>预期：抛出 ArgumentNullException</para>
    /// </summary>
    [Fact]
    public async Task GetAsync_WithNullKey_ShouldThrowArgumentNullException() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => provider.GetAsync<string>(null!));
    }

    #endregion

    #region SetAsync Tests

    /// <summary>
    /// 测试设置缓存 - 正常情况
    /// <para>场景：设置缓存数据</para>
    /// <para>预期：成功设置缓存</para>
    /// </summary>
    [Fact]
    public async Task SetAsync_WithValidParameters_ShouldSetCache() {
        var cacheKey = "test:set-key";
        var value = "test-value";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        mockHybridCache.Setup(x => x.SetAsync(
            cacheKey,
            value,
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        await provider.SetAsync(cacheKey, value);

        mockHybridCache.Verify(x => x.SetAsync(
            cacheKey,
            value,
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// 测试设置缓存 - 自定义过期时间
    /// <para>场景：设置缓存并指定过期时间</para>
    /// <para>预期：使用指定的过期时间</para>
    /// </summary>
    [Fact]
    public async Task SetAsync_WithCustomExpiration_ShouldUseCustomExpiration() {
        var cacheKey = "test:expiring-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(10);

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await provider.SetAsync(cacheKey, value, expiration);

        mockHybridCache.Verify(x => x.SetAsync(
            cacheKey,
            value,
            It.Is<HybridCacheEntryOptions>(o => o.Expiration == expiration),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// 测试设置缓存 - 空值
    /// <para>场景：尝试设置 null 值</para>
    /// <para>预期：抛出 ArgumentNullException</para>
    /// </summary>
    [Fact]
    public async Task SetAsync_WithNullValue_ShouldThrowArgumentNullException() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => provider.SetAsync<string>("test:key", null!));
    }

    #endregion

    #region RemoveAsync Tests

    /// <summary>
    /// 测试删除缓存 - 正常情况
    /// <para>场景：删除存在的缓存</para>
    /// <para>预期：成功删除缓存</para>
    /// </summary>
    [Fact]
    public async Task RemoveAsync_WithValidKey_ShouldRemoveCache() {
        var cacheKey = "test:remove-key";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        mockHybridCache.Setup(x => x.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        await provider.RemoveAsync(cacheKey);

        mockHybridCache.Verify(x => x.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// 测试删除缓存 - 空键
    /// <para>场景：传入空的缓存键</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Fact]
    public async Task RemoveAsync_WithEmptyKey_ShouldThrowArgumentException() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => provider.RemoveAsync(""));
    }

    #endregion

    #region RemoveByPatternAsync Tests

    /// <summary>
    /// 测试按模式删除缓存 - Redis 启用
    /// <para>场景：Redis 启用时按模式删除</para>
    /// <para>预期：调用 Redis 删除</para>
    /// </summary>
    [Fact]
    public async Task RemoveByPatternAsync_WithRedisEnabled_ShouldCallRedis() {
        var pattern = "test:*";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProviderWithRedis(mockHybridCache.Object);

        var mockDatabase = new Mock<IDatabase>();
        var mockServer = new Mock<IServer>();

        _mockRedisConnection.Setup(x => x.GetDatabase()).Returns(mockDatabase.Object);
        _mockRedisConnection.Setup(x => x.GetEndPoints()).Returns([new IPEndPoint(IPAddress.Loopback, 6379)]);
        _mockRedisConnection.Setup(x => x.GetServer(It.IsAny<EndPoint>())).Returns(mockServer.Object);

        var keys = new List<RedisKey> { "test:key1", "test:key2" };
        mockServer.Setup(x => x.KeysAsync(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(keys.ToAsyncEnumerable());

        mockDatabase.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(2);

        var result = await provider.RemoveByPatternAsync(pattern);

        Assert.Equal(2, result);
    }

    /// <summary>
    /// 测试按模式删除缓存 - Redis 未启用
    /// <para>场景：Redis 未启用</para>
    /// <para>预期：返回 0</para>
    /// </summary>
    [Fact]
    public async Task RemoveByPatternAsync_WithRedisDisabled_ShouldReturnZero() {
        var pattern = "test:*";

        _redisOptions.Enabled = false;
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        var result = await provider.RemoveByPatternAsync(pattern);

        Assert.Equal(0, result);
    }

    /// <summary>
    /// 测试按模式删除缓存 - 空模式
    /// <para>场景：传入空的模式</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Fact]
    public async Task RemoveByPatternAsync_WithEmptyPattern_ShouldThrowArgumentException() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => provider.RemoveByPatternAsync(""));
    }

    #endregion

    #region ExistsAsync Tests

    /// <summary>
    /// 测试检查缓存存在 - Redis 启用
    /// <para>场景：检查缓存是否存在</para>
    /// <para>预期：返回正确的结果</para>
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsAsync_WithRedisEnabled_ShouldReturnCorrectResult(bool exists) {
        var cacheKey = "test:exists-key";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProviderWithRedis(mockHybridCache.Object);

        var mockDatabase = new Mock<IDatabase>();
        mockDatabase.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(exists);

        _mockRedisConnection.Setup(x => x.GetDatabase()).Returns(mockDatabase.Object);

        var result = await provider.ExistsAsync(cacheKey);

        Assert.Equal(exists, result);
    }

    /// <summary>
    /// 测试检查缓存存在 - Redis 未启用
    /// <para>场景：Redis 未启用</para>
    /// <para>预期：返回 false</para>
    /// </summary>
    [Fact]
    public async Task ExistsAsync_WithRedisDisabled_ShouldReturnFalse() {
        var cacheKey = "test:exists-key";

        _redisOptions.Enabled = false;
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        var result = await provider.ExistsAsync(cacheKey);

        Assert.False(result);
    }

    #endregion

    #region DistributedLimiterAsync Tests

    /// <summary>
    /// 测试分布式限流 - 正常情况
    /// <para>场景：使用 Redis 进行分布式限流</para>
    /// <para>预期：返回递增后的值</para>
    /// </summary>
    [Fact]
    public async Task DistributedLimiterAsync_WithRedisEnabled_ShouldIncrement() {
        var key = "rate-limit:test";
        var expectedValue = 5L;

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProviderWithRedis(mockHybridCache.Object);

        var mockDatabase = new Mock<IDatabase>();
        mockDatabase.Setup(x => x.StringIncrementAsync(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(expectedValue);

        _mockRedisConnection.Setup(x => x.GetDatabase()).Returns(mockDatabase.Object);

        var result = await provider.DistributedLimiterAsync(key);

        Assert.Equal(expectedValue, result);
    }

    /// <summary>
    /// 测试分布式限流 - Redis 未启用
    /// <para>场景：Redis 未启用</para>
    /// <para>预期：返回 0</para>
    /// </summary>
    [Fact]
    public async Task DistributedLimiterAsync_WithRedisDisabled_ShouldReturnZero() {
        var key = "rate-limit:test";

        _redisOptions.Enabled = false;
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        var result = await provider.DistributedLimiterAsync(key);

        Assert.Equal(0, result);
    }

    #endregion

    #region GetOrSetAsync Tests

    /// <summary>
    /// 测试获取或设置缓存 - 缓存未命中
    /// <para>场景：缓存不存在，调用工厂方法</para>
    /// <para>预期：调用工厂方法并缓存结果</para>
    /// </summary>
    [Fact]
    public async Task GetOrSetAsync_WithCacheMiss_ShouldCallFactory() {
        var cacheKey = "test:factory-key";
        var expectedValue = "factory-result";

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        mockHybridCache.Setup(x => x.GetOrCreateAsync(
            cacheKey,
            It.IsAny<Func<CancellationToken, ValueTask<string?>>>(),
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        var result = await provider.GetOrSetAsync(cacheKey, () => Task.FromResult<string?>(expectedValue));

        Assert.Equal(expectedValue, result);
    }

    /// <summary>
    /// 测试获取或设置缓存 - 空键
    /// <para>场景：传入空的缓存键</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Fact]
    public async Task GetOrSetAsync_WithEmptyKey_ShouldThrowArgumentException() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => provider.GetOrSetAsync("", () => Task.FromResult<string?>(null)));
    }

    /// <summary>
    /// 测试获取或设置缓存 - null 工厂
    /// <para>场景：传入 null 的工厂方法</para>
    /// <para>预期：抛出 ArgumentNullException</para>
    /// </summary>
    [Fact]
    public async Task GetOrSetAsync_WithNullFactory_ShouldThrowArgumentNullException() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => provider.GetOrSetAsync<string>("test:key", null!));
    }

    #endregion

    #region Statistics Tests

    /// <summary>
    /// 测试获取统计数据 - 初始状态
    /// <para>场景：获取初始统计数据</para>
    /// <para>预期：所有计数器为 0</para>
    /// </summary>
    [Fact]
    public void GetStatistics_Initially_ShouldReturnZeroCounts() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        var statistics = provider.GetStatistics();

        Assert.Equal(0, statistics.HitCount);
        Assert.Equal(0, statistics.MissCount);
        Assert.Equal(0, statistics.TotalRequests);
        Assert.Equal(0, statistics.HitRate);
    }

    /// <summary>
    /// 测试重置统计数据
    /// <para>场景：重置统计数据</para>
    /// <para>预期：所有计数器重置为 0</para>
    /// </summary>
    [Fact]
    public void ResetStatistics_ShouldResetAllCounters() {
        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);

        provider.ResetStatistics();

        var statistics = provider.GetStatistics();

        Assert.Equal(0, statistics.HitCount);
        Assert.Equal(0, statistics.MissCount);
    }

    #endregion

    #region Avalanche Protection Tests

    /// <summary>
    /// 测试雪崩保护 - 启用
    /// <para>场景：启用雪崩保护</para>
    /// <para>预期：过期时间包含随机偏移</para>
    /// </summary>
    [Fact]
    public async Task SetAsync_WithAvalancheProtection_ShouldAddRandomOffset() {
        _hybridCacheOptions.EnableAvalancheProtection = true;
        _hybridCacheOptions.AvalancheProtectionMaxOffsetSeconds = 30;

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);
        TimeSpan? capturedExpiration = null;

        mockHybridCache.Setup(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .Callback<string, string, HybridCacheEntryOptions, IEnumerable<string>?, CancellationToken>((_, _, options, _, _) => {
                capturedExpiration = options.Expiration;
            })
            .Returns(ValueTask.CompletedTask);

        await provider.SetAsync("test:key", "value");

        Assert.NotNull(capturedExpiration);
        var baseExpiration = TimeSpan.FromMinutes(_hybridCacheOptions.DefaultExpirationMinutes);
        var maxExpiration = baseExpiration.Add(TimeSpan.FromSeconds(_hybridCacheOptions.AvalancheProtectionMaxOffsetSeconds));

        Assert.True(capturedExpiration >= baseExpiration);
        Assert.True(capturedExpiration <= maxExpiration);
    }

    /// <summary>
    /// 测试雪崩保护 - 禁用
    /// <para>场景：禁用雪崩保护</para>
    /// <para>预期：过期时间为默认值</para>
    /// </summary>
    [Fact]
    public async Task SetAsync_WithoutAvalancheProtection_ShouldUseBaseExpiration() {
        _hybridCacheOptions.EnableAvalancheProtection = false;

        var mockHybridCache = new Mock<HybridCache>();
        var provider = CreateProvider(mockHybridCache.Object);
        TimeSpan? capturedExpiration = null;

        mockHybridCache.Setup(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<HybridCacheEntryOptions>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .Callback<string, string, HybridCacheEntryOptions, IEnumerable<string>?, CancellationToken>((_, _, options, _, _) => {
                capturedExpiration = options.Expiration;
            })
            .Returns(ValueTask.CompletedTask);

        await provider.SetAsync("test:key", "value");

        Assert.NotNull(capturedExpiration);
        Assert.Equal(TimeSpan.FromMinutes(_hybridCacheOptions.DefaultExpirationMinutes), capturedExpiration);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 创建缓存提供者实例
    /// </summary>
    private HybridCacheProvider CreateProvider(HybridCache hybridCache) {
        var redisOptions = Microsoft.Extensions.Options.Options.Create(_redisOptions);
        var hybridOptions = Microsoft.Extensions.Options.Options.Create(_hybridCacheOptions);

        return new HybridCacheProvider(
            hybridCache,
            _mockRedisConnection.Object,
            redisOptions,
            hybridOptions,
            _mockLogger.Object
        );
    }

    /// <summary>
    /// 创建启用 Redis 的缓存提供者实例
    /// </summary>
    private HybridCacheProvider CreateProviderWithRedis(HybridCache hybridCache) {
        _redisOptions.Enabled = true;
        return CreateProvider(hybridCache);
    }

    #endregion
}
