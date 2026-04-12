/*
 * 文件名称: HybridCacheProvider.cs
 * 功能描述: 基于 HybridCache 的缓存提供者实现
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using System.Collections.Concurrent;
using System.Diagnostics;
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
/// <remarks>
/// <para>增强功能：</para>
/// <list type="bullet">
///   <item>缓存命中率统计</item>
///   <item>缓存雪崩保护（随机过期偏移）</item>
///   <item>增强的缓存击穿保护（信号量锁）</item>
///   <item>可配置的缓存选项</item>
/// </list>
/// </remarks>
public sealed class HybridCacheProvider : ICacheProvider {
    private readonly HybridCache _hybridCache;
    private readonly IRedisConnectionManager? _redisConnection;
    private readonly RedisOption _redisOptions;
    private readonly HybridCacheOption _options;
    private readonly ILogger<HybridCacheProvider> _logger;

    private readonly CacheStatistics _statistics = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();
    private readonly SemaphoreSlim _concurrencyLimiter = null!;

    /// <summary>
    /// 初始化 HybridCacheProvider
    /// </summary>
    /// <param name="hybridCache">HybridCache 实例</param>
    /// <param name="redisConnection">Redis 连接管理器</param>
    /// <param name="redisOptions">Redis 配置选项</param>
    /// <param name="hybridOptions">混合缓存配置选项</param>
    /// <param name="logger">日志记录器</param>
    public HybridCacheProvider(
        HybridCache hybridCache,
        IRedisConnectionManager redisConnection,
        IOptions<RedisOption> redisOptions,
        IOptions<HybridCacheOption> hybridOptions,
        ILogger<HybridCacheProvider> logger) {
        _hybridCache = hybridCache ?? throw new ArgumentNullException(nameof(hybridCache));
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _redisOptions = redisOptions?.Value ?? throw new ArgumentNullException(nameof(redisOptions));
        _options = hybridOptions?.Value ?? throw new ArgumentNullException(nameof(hybridOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _concurrencyLimiter = new SemaphoreSlim(_options.MaxConcurrentFactoryCalls, _options.MaxConcurrentFactoryCalls);
    }

    /// <summary>
    /// 初始化 HybridCacheProvider（兼容旧版构造函数）
    /// </summary>
    /// <param name="hybridCache">HybridCache 实例</param>
    /// <param name="redisConnection">Redis 连接管理器</param>
    /// <param name="redisOptions">Redis 配置选项</param>
    /// <param name="memoryOptions">内存缓存配置选项（用于获取默认过期时间）</param>
    /// <param name="logger">日志记录器</param>
    public HybridCacheProvider(
        HybridCache hybridCache,
        IRedisConnectionManager redisConnection,
        IOptions<RedisOption> redisOptions,
        IOptions<MemoryCacheOption> memoryOptions,
        ILogger<HybridCacheProvider> logger) {
        _hybridCache = hybridCache ?? throw new ArgumentNullException(nameof(hybridCache));
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _redisOptions = redisOptions?.Value ?? throw new ArgumentNullException(nameof(redisOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var memOptions = memoryOptions?.Value ?? throw new ArgumentNullException(nameof(memoryOptions));
        _options = new HybridCacheOption {
            DefaultExpirationMinutes = memOptions.DefaultExpirationMinutes
        };
        _concurrencyLimiter = new SemaphoreSlim(_options.MaxConcurrentFactoryCalls, _options.MaxConcurrentFactoryCalls);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
#pragma warning disable CA1031
    public async Task<T?> GetAsync<T>(string key) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var stopwatch = Stopwatch.StartNew();

        try {
            var value = await _hybridCache.GetOrCreateAsync<T?>(
                key,
                _ => ValueTask.FromResult<T?>(default),
                new HybridCacheEntryOptions { Expiration = GetExpirationWithAvalancheProtection() });

            stopwatch.Stop();

            if (_options.EnableStatistics) {
                if (value is not null) {
                    _statistics.RecordHit(stopwatch.Elapsed);
                    _logger.LogDebug("缓存命中: {Key}, 耗时: {Elapsed}ms", key, stopwatch.ElapsedMilliseconds);
                }
                else {
                    _statistics.RecordMiss(stopwatch.Elapsed);
                    _logger.LogDebug("缓存未命中: {Key}, 耗时: {Elapsed}ms", key, stopwatch.ElapsedMilliseconds);
                }
            }

            return value;
        }
        catch (Exception ex) {
            stopwatch.Stop();
            _logger.LogError(ex, "获取缓存失败 | Key: {Key}", key);
            return default;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 设置缓存
    /// </summary>
#pragma warning disable CA1031
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        try {
            var actualExpiration = expiration ?? GetExpirationWithAvalancheProtection();
            var options = new HybridCacheEntryOptions {
                Expiration = actualExpiration
            };

            await _hybridCache.SetAsync(key, value, options);
            _logger.LogDebug("缓存设置成功: {Key}, 过期时间: {Expiration}", key, actualExpiration);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "设置缓存失败 | Key: {Key}", key);
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 删除缓存
    /// </summary>
#pragma warning disable CA1031
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
#pragma warning disable CA1031
    public async Task<int> RemoveByPatternAsync(string pattern) {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        if (!_redisOptions.Enabled || _redisConnection == null) {
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
                await db.KeyDeleteAsync(keys);
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
#pragma warning disable CA1031
    public async Task<bool> ExistsAsync(string key) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_redisOptions.Enabled || _redisConnection == null) {
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
#pragma warning disable CA1031
    public async Task<long> DistributedLimiterAsync(string key, TimeSpan? expiration = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_redisOptions.Enabled || _redisConnection == null) {
            _logger.LogWarning("Redis 未启用，分布式限流不可用");
            return 0;
        }

        try {
            var db = _redisConnection.GetDatabase();
            var value = await db.StringIncrementAsync(GetPrefixedKey(key));

            if (value == 1 && expiration.HasValue) {
                await db.KeyExpireAsync(GetPrefixedKey(key), expiration.Value);
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
    /// <para>增强的击穿保护机制：</para>
    /// <list type="bullet">
    ///   <item>HybridCache 内置击穿保护</item>
    ///   <item>额外的信号量锁防止并发工厂调用</item>
    ///   <item>并发限制防止数据库过载</item>
    /// </list>
    /// </summary>
#pragma warning disable CA1031
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        var stopwatch = Stopwatch.StartNew();

        try {
            var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            var timeout = TimeSpan.FromMilliseconds(_options.StampedeProtectionTimeoutMs);
            var acquired = await keyLock.WaitAsync(timeout);

            if (!acquired) {
                _logger.LogWarning("缓存击穿保护超时，直接调用工厂方法: {Key}", key);
                return await ExecuteFactoryWithConcurrencyLimitAsync(factory);
            }

            try {
                var actualExpiration = expiration ?? GetExpirationWithAvalancheProtection();
                var options = new HybridCacheEntryOptions {
                    Expiration = actualExpiration
                };

                var value = await _hybridCache.GetOrCreateAsync(
                    key,
                    async _ => await ExecuteFactoryWithConcurrencyLimitAsync(factory),
                    options);

                stopwatch.Stop();

                if (_options.EnableStatistics) {
                    _statistics.RecordHit(stopwatch.Elapsed);
                    _logger.LogDebug("缓存获取或设置成功: {Key}, 耗时: {Elapsed}ms", key, stopwatch.ElapsedMilliseconds);
                }

                return value;
            }
            finally {
                keyLock.Release();
            }
        }
        catch (Exception ex) {
            stopwatch.Stop();
            _logger.LogError(ex, "获取或设置缓存失败 | Key: {Key}", key);
            return default;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 获取缓存统计数据
    /// </summary>
    /// <returns>缓存统计数据快照</returns>
    public CacheStatisticsSnapshot GetStatistics() {
        return _statistics.GetSnapshot();
    }

    /// <summary>
    /// 重置缓存统计数据
    /// </summary>
    public void ResetStatistics() {
        _statistics.Reset();
        _logger.LogInformation("缓存统计数据已重置");
    }

    /// <summary>
    /// 获取带雪崩保护的过期时间
    /// <para>在基础过期时间上添加随机偏移，防止大量缓存同时失效</para>
    /// </summary>
    /// <returns>带随机偏移的过期时间</returns>
    private TimeSpan GetExpirationWithAvalancheProtection() {
        var baseExpiration = TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);

        if (!_options.EnableAvalancheProtection || _options.AvalancheProtectionMaxOffsetSeconds <= 0) {
            return baseExpiration;
        }

        var maxOffsetSeconds = _options.AvalancheProtectionMaxOffsetSeconds;
        var randomOffset = Random.Shared.Next(0, maxOffsetSeconds + 1);

        return baseExpiration.Add(TimeSpan.FromSeconds(randomOffset));
    }

    /// <summary>
    /// 在并发限制下执行工厂方法
    /// <para>限制同时执行数据加载的并发数，防止数据库过载</para>
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="factory">工厂方法</param>
    /// <returns>工厂方法返回的数据</returns>
    private async Task<T?> ExecuteFactoryWithConcurrencyLimitAsync<T>(Func<Task<T?>> factory) {
        await _concurrencyLimiter.WaitAsync();
        try {
            return await factory();
        }
        finally {
            _concurrencyLimiter.Release();
        }
    }

    /// <summary>
    /// 获取带前缀的缓存键
    /// </summary>
    private string GetPrefixedKey(string key) {
        return _redisOptions.InstanceName.IsNullOrWhiteSpace()
            ? key
            : $"{_redisOptions.InstanceName}{key}";
    }
}
