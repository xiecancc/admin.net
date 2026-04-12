namespace Domain.Shared.Caches;

/// <summary>
/// 缓存提供者接口
/// <para>统一的缓存操作接口，支持通配符删除和统计功能</para>
/// </summary>
public interface ICacheProvider {
    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T">缓存类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <returns>缓存值</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <typeparam name="T">缓存类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="value">缓存值</param>
    /// <param name="expiration">过期时间</param>
    /// <returns>任务</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>任务</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// 按模式删除缓存（支持通配符）
    /// <para>
    /// 通配符说明：
    /// <list type="bullet">
    ///   <item><description>* - 匹配任意数量的任意字符</description></item>
    ///   <item><description>? - 匹配单个任意字符</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="pattern">模式（支持 * 和 ? 通配符）</param>
    /// <returns>删除的键数量</returns>
    Task<int> RemoveByPatternAsync(string pattern);

    /// <summary>
    /// 检查缓存是否存在
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 分布式限流原子递增
    /// <para>用于分布式限流计数，保证多实例一致性</para>
    /// </summary>
    /// <param name="key">限流键</param>
    /// <param name="expiration">过期时间（仅首次设置时生效）</param>
    /// <returns>递增后的值</returns>
    /// <remarks>
    /// <para>如果键不存在，则初始化为 1</para>
    /// <para>操作为原子性，保证并发安全</para>
    /// <para>MultiCacheProvider 实现仅使用 Redis，保证多实例一致性</para>
    /// </remarks>
    Task<long> DistributedLimiterAsync(string key, TimeSpan? expiration = null);

    /// <summary>
    /// 获取或设置缓存（带击穿保护）
    /// <para>如果缓存不存在，使用工厂方法获取数据并缓存</para>
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="factory">数据工厂方法</param>
    /// <param name="expiration">过期时间</param>
    /// <returns>缓存数据</returns>
    /// <remarks>
    /// <para>使用锁机制防止缓存击穿</para>
    /// <para>同一时刻只有一个请求能加载数据，其他请求等待</para>
    /// </remarks>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null);

    /// <summary>
    /// 获取缓存统计数据
    /// <para>返回包含命中率、响应时间等统计信息的快照</para>
    /// </summary>
    /// <returns>缓存统计数据快照</returns>
    /// <remarks>
    /// <para>统计信息包括：命中次数、未命中次数、命中率、平均响应时间等</para>
    /// <para>如果统计功能未启用，返回空的统计数据</para>
    /// </remarks>
    CacheStatisticsSnapshot GetStatistics();

    /// <summary>
    /// 重置缓存统计数据
    /// <para>将所有统计指标重置为初始值</para>
    /// </summary>
    /// <remarks>
    /// <para>用于定期重置统计数据，便于监控特定时间段的缓存性能</para>
    /// <para>如果统计功能未启用，此方法不执行任何操作</para>
    /// </remarks>
    void ResetStatistics();
}
