using System.Net;
using StackExchange.Redis;

namespace Domain.Shared.Caches;

/// <summary>
/// Redis 连接管理服务
/// </summary>
public interface IRedisConnectionManager : IDisposable {
    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected {
        get;
    }

    /// <summary>
    /// 获取 Redis 数据库
    /// </summary>
    /// <param name="database">数据库索引</param>
    /// <returns>Redis 数据库</returns>
    IDatabase GetDatabase(int database = 0);

    /// <summary>
    /// 获取所有终结点
    /// </summary>
    /// <returns>终结点集合</returns>
    EndPoint[] GetEndPoints();

    /// <summary>
    /// 获取服务器实例
    /// </summary>
    /// <param name="endpoint">终结点</param>
    /// <returns>服务器实例</returns>
    IServer GetServer(EndPoint endpoint);
}
