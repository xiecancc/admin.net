using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Options;

namespace Infrastructure.Caches;

/// <summary>
/// Redis 连接管理实现
/// </summary>
public class RedisConnectionManager : IRedisConnectionManager {
    private readonly ConnectionMultiplexer? _connection;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RedisConnectionManager(
        IOptions<RedisOption> redisOptions,
        ILogger<RedisConnectionManager> logger) {
        ArgumentNullException.ThrowIfNull(redisOptions);
        ArgumentNullException.ThrowIfNull(logger);

        if (redisOptions.Value.Enabled) {
            _connection = CreateConnection(redisOptions.Value, logger);
            IsConnected = _connection?.IsConnected == true;
        }
        else {
            IsConnected = false;
        }
    }

    /// <summary>
    /// 创建 Redis 连接
    /// </summary>
    private static ConnectionMultiplexer? CreateConnection(RedisOption options, ILogger logger) {
        try {
            var config = new ConfigurationOptions {
                EndPoints = { options.ConnectionString },
                ConnectTimeout = options.ConnectTimeout,
                SyncTimeout = options.SyncTimeout,
                AbortOnConnectFail = false
            };

            var connection = ConnectionMultiplexer.Connect(config);
            logger.LogInformation("Redis 连接成功：{ConnectionString}", options.ConnectionString);
            return connection;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Redis 连接失败，将禁用 Redis 缓存");
            return null;
        }
    }

    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected => field && _connection?.IsConnected == true;

    /// <summary>
    /// 获取 Redis 数据库
    /// </summary>
    public IDatabase GetDatabase(int database = 0) {
        return _connection == null ? throw new InvalidOperationException("Redis 未连接") : _connection.GetDatabase(database);
    }

    /// <summary>
    /// 获取所有终结点
    /// </summary>
    public EndPoint[] GetEndPoints() {
        return _connection?.GetEndPoints() ?? Array.Empty<EndPoint>();
    }

    /// <summary>
    /// 获取服务器实例
    /// </summary>
    public IServer GetServer(EndPoint endpoint) {
        return _connection == null ? throw new InvalidOperationException("Redis 未连接") : _connection.GetServer(endpoint);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose() {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
