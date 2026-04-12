/*
 * 文件名称: CacheStatistics.cs
 * 功能描述: 缓存统计数据模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Domain.Shared.Caches;

/// <summary>
/// 缓存统计数据
/// <para>记录缓存操作的统计信息，用于监控缓存性能</para>
/// </summary>
/// <remarks>
/// <para>统计指标：</para>
/// <list type="bullet">
///   <item>命中次数：缓存命中的总次数</item>
///   <item>未命中次数：缓存未命中的总次数</item>
///   <item>命中率：命中次数 / (命中次数 + 未命中次数)</item>
///   <item>总请求数：命中次数 + 未命中次数</item>
///   <item>平均响应时间：缓存操作的平均耗时</item>
/// </list>
/// </remarks>
public sealed class CacheStatistics {
    private long _hitCount;
    private long _missCount;
    private long _totalResponseTimeTicks;
    private long _operationCount;

    /// <summary>
    /// 缓存命中次数
    /// <para>从缓存中成功获取数据的次数</para>
    /// </summary>
    /// <value>命中次数</value>
    public long HitCount => _hitCount;

    /// <summary>
    /// 缓存未命中次数
    /// <para>缓存中不存在数据，需要从数据源加载的次数</para>
    /// </summary>
    /// <value>未命中次数</value>
    public long MissCount => _missCount;

    /// <summary>
    /// 缓存命中率
    /// <para>命中次数占总请求次数的比例</para>
    /// </summary>
    /// <value>命中率（0-1 之间）</value>
    /// <remarks>
    /// <para>计算公式：命中次数 / (命中次数 + 未命中次数)</para>
    /// <para>当总请求次数为 0 时，返回 0</para>
    /// </remarks>
    public double HitRate {
        get {
            var total = _hitCount + _missCount;
            return total == 0 ? 0 : (double)_hitCount / total * 100;
        }
    }

    /// <summary>
    /// 总请求次数
    /// <para>命中次数与未命中次数的总和</para>
    /// </summary>
    /// <value>总请求次数</value>
    public long TotalRequests => _hitCount + _missCount;

    /// <summary>
    /// 平均响应时间
    /// <para>缓存操作的平均耗时（毫秒）</para>
    /// </summary>
    /// <value>平均响应时间（毫秒）</value>
    /// <remarks>
    /// <para>计算公式：总响应时间 / 操作次数</para>
    /// <para>当操作次数为 0 时，返回 0</para>
    /// </remarks>
    public double AverageResponseTimeMs {
        get {
            if (_operationCount == 0) {
                return 0;
            }

            return TimeSpan.FromTicks(_totalResponseTimeTicks).TotalMilliseconds / _operationCount;
        }
    }

    /// <summary>
    /// 统计开始时间
    /// <para>统计数据的起始时间</para>
    /// </summary>
    /// <value>统计开始时间</value>
    public DateTimeOffset StartTime { get; }

    /// <summary>
    /// 最后更新时间
    /// <para>统计数据最后更新的时间</para>
    /// </summary>
    /// <value>最后更新时间</value>
    public DateTimeOffset LastUpdateTime { get; private set; }

    /// <summary>
    /// 初始化缓存统计数据
    /// </summary>
    public CacheStatistics() {
        StartTime = DateTimeOffset.UtcNow;
        LastUpdateTime = StartTime;
    }

    /// <summary>
    /// 记录缓存命中
    /// <para>线程安全的命中计数递增</para>
    /// </summary>
    /// <param name="responseTime">本次操作的响应时间</param>
    public void RecordHit(TimeSpan responseTime) {
        Interlocked.Increment(ref _hitCount);
        RecordResponseTime(responseTime);
    }

    /// <summary>
    /// 记录缓存未命中
    /// <para>线程安全的未命中计数递增</para>
    /// </summary>
    /// <param name="responseTime">本次操作的响应时间</param>
    public void RecordMiss(TimeSpan responseTime) {
        Interlocked.Increment(ref _missCount);
        RecordResponseTime(responseTime);
    }

    /// <summary>
    /// 记录响应时间
    /// <para>线程安全的响应时间累加</para>
    /// </summary>
    /// <param name="responseTime">响应时间</param>
    private void RecordResponseTime(TimeSpan responseTime) {
        Interlocked.Add(ref _totalResponseTimeTicks, responseTime.Ticks);
        Interlocked.Increment(ref _operationCount);
        LastUpdateTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// 重置统计数据
    /// <para>将所有统计指标重置为初始值</para>
    /// </summary>
    public void Reset() {
        _hitCount = 0;
        _missCount = 0;
        _totalResponseTimeTicks = 0;
        _operationCount = 0;
        LastUpdateTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// 获取统计快照
    /// <para>返回当前统计数据的不可变快照</para>
    /// </summary>
    /// <returns>包含当前统计数据的快照对象</returns>
    public CacheStatisticsSnapshot GetSnapshot() {
        return new CacheStatisticsSnapshot {
            HitCount = _hitCount,
            MissCount = _missCount,
            HitRate = HitRate,
            TotalRequests = TotalRequests,
            AverageResponseTimeMs = AverageResponseTimeMs,
            StartTime = StartTime,
            LastUpdateTime = LastUpdateTime
        };
    }
}

/// <summary>
/// 缓存统计快照
/// <para>缓存统计数据的不可变快照，用于报告和监控</para>
/// </summary>
public sealed class CacheStatisticsSnapshot {
    /// <summary>
    /// 缓存命中次数
    /// </summary>
    /// <value>命中次数</value>
    public required long HitCount { get; init; }

    /// <summary>
    /// 缓存未命中次数
    /// </summary>
    /// <value>未命中次数</value>
    public required long MissCount { get; init; }

    /// <summary>
    /// 缓存命中率
    /// </summary>
    /// <value>命中率（0-1 之间）</value>
    public required double HitRate { get; init; }

    /// <summary>
    /// 总请求次数
    /// </summary>
    /// <value>总请求次数</value>
    public required long TotalRequests { get; init; }

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    /// <value>平均响应时间</value>
    public required double AverageResponseTimeMs { get; init; }

    /// <summary>
    /// 统计开始时间
    /// </summary>
    /// <value>统计开始时间</value>
    public required DateTimeOffset StartTime { get; init; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    /// <value>最后更新时间</value>
    public required DateTimeOffset LastUpdateTime { get; init; }
}
