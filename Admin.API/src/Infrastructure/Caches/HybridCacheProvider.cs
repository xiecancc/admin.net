/*
 * 文件名称: HybridCacheProvider.cs
 * 功能描述: 基于 HybridCache 的缓存提供者实现
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Options;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caches;

/// <summary>
/// 基于 HybridCache 的缓存提供者
/// <para>使用 .NET 10 HybridCache 实现 L1/L2 缓存协调</para>
/// <para>HybridCache 原生能力：</para>
/// <list type="bullet">
///   <item>L1 内存缓存 + L2 分布式缓存自动协调</item>
///   <item>内置缓存击穿保护（Stampede Protection）</item>
///   <item>自动序列化/反序列化</item>
///   <item>可配置的过期策略</item>
/// </list>
/// </summary>
public sealed class HybridCacheProvider(
    HybridCache hybridCache,
    IRedisConnectionManager redisConnection,
    IOptions<RedisOption> redisOptions,
    IOptions<MemoryCacheOption> memoryOptions,
    ILogger<HybridCacheProvider> logger) : ICacheProvider {
    private readonly HybridCache _hybridCache = hybridCache;
    private readonly IRedisConnectionManager _redisConnection = redisConnection;
    private readonly RedisOption _redisOptions = redisOptions.Value;
    private readonly MemoryCacheOption _memoryOptions = memoryOptions.Value;
    private readonly ILogger<HybridCacheProvider> _logger = logger;

    /// <summary>
    /// 获取缓存
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task<T?> GetAsync<T>(string key) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try {
            var value = await _hybridCache.GetOrCreateAsync<T?>(
                key,
                _ => ValueTask.FromResult<T?>(default),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(_memoryOptions.DefaultExpirationMinutes) });

            if (value is not null) {
                _logger.LogDebug("缓存命中: {Key}", key);
            }

            return value;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取缓存失败 | Key: {Key}", key);
            return default;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 设置缓存
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        try {
            var options = new HybridCacheEntryOptions {
                Expiration = expiration ?? TimeSpan.FromMinutes(_memoryOptions.DefaultExpirationMinutes)
            };

            await _hybridCache.SetAsync(key, value, options);
            _logger.LogDebug("缓存设置成功: {Key}", key);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "设置缓存失败 | Key: {Key}", key);
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 删除缓存
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task RemoveAsync(string key) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try {
            await _hybridCache.RemoveAsync(key);
            _logger.LogDebug("缓存删除成功: {Key}", key);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "删除缓存失败 | Key: {Key}", key);
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 按模式删除缓存
    /// <para>注意：HybridCache 不支持模式删除，此方法直接操作 Redis L2 缓存</para>
    /// <para>多实例部署时，L1 内存缓存可能存在短暂不一致</para>
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task<int> RemoveByPatternAsync(string pattern) {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        if (!_redisOptions.Enabled) {
            _logger.LogWarning("Redis 未启用，模式删除不可用");
            return 0;
        }

        try {
            var db = _redisConnection.GetDatabase();
            var endPoints = _redisConnection.GetEndPoints();
            if (endPoints.Length == 0) {
                return 0;
            }

            var server = _redisConnection.GetServer(endPoints[0]);
            var prefixedPattern = GetPrefixedKey(pattern);
            var keys = server.Keys(pattern: prefixedPattern).ToArray();

            if (keys.Length > 0) {
                _ = await db.KeyDeleteAsync(keys);
                _logger.LogDebug("按模式删除缓存: {Pattern}, 数量: {Count}", pattern, keys.Length);
                return keys.Length;
            }

            return 0;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "按模式删除缓存失败 | Pattern: {Pattern}", pattern);
            return 0;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 检查缓存是否存在
    /// <para>注意：HybridCache 不提供 Exists 检查，此方法检查 Redis L2 缓存</para>
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task<bool> ExistsAsync(string key) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_redisOptions.Enabled) {
            _logger.LogWarning("Redis 未启用，缓存存在检查不可用");
            return false;
        }

        try {
            var db = _redisConnection.GetDatabase();
            return await db.KeyExistsAsync(GetPrefixedKey(key));
        }
        catch (Exception ex) {
            _logger.LogError(ex, "检查缓存存在失败 | Key: {Key}", key);
            return false;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 分布式限流原子递增
    /// <para>使用 Redis 原子操作保证多实例一致性</para>
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task<long> DistributedLimiterAsync(string key, TimeSpan? expiration = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_redisOptions.Enabled) {
            _logger.LogWarning("Redis 未启用，分布式限流不可用");
            return 0;
        }

        try {
            var db = _redisConnection.GetDatabase();
            var value = await db.StringIncrementAsync(GetPrefixedKey(key));

            if (value == 1 && expiration.HasValue) {
                _ = await db.KeyExpireAsync(GetPrefixedKey(key), expiration.Value);
            }

            _logger.LogDebug("分布式限流: {Key}, 值: {Value}", key, value);
            return value;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "分布式限流失败 | Key: {Key}", key);
            return 0;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 获取或设置缓存（带击穿保护）
    /// <para>HybridCache 内置击穿保护，同一时刻只有一个请求能加载数据</para>
    /// </summary>
#pragma warning disable CA1031 // 缓存操作需要捕获所有异常以确保不影响主业务流程
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try {
            var options = new HybridCacheEntryOptions {
                Expiration = expiration ?? TimeSpan.FromMinutes(_memoryOptions.DefaultExpirationMinutes)
            };

            var value = await _hybridCache.GetOrCreateAsync(
                key,
                async _ => await factory(),
                options);

            return value;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取或设置缓存失败 | Key: {Key}", key);
            return default;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 获取带前缀的缓存键
    /// </summary>
    private string GetPrefixedKey(string key) {
        return _redisOptions.InstanceName.IsNullOrWhiteSpace()
            ? key
            : $"{_redisOptions.InstanceName}{key}";
    }
}
