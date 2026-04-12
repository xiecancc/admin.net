/*
 * 文件名称: HybridCacheOption.cs
 * 功能描述: 混合缓存配置选项类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// 混合缓存配置选项
/// <para>用于配置 HybridCache 的行为参数</para>
/// </summary>
/// <remarks>
/// <para>配置项说明：</para>
/// <list type="bullet">
///   <item>DefaultExpirationMinutes：默认过期时间（分钟）</item>
///   <item>EnableStatistics：是否启用统计功能</item>
///   <item>EnableAvalancheProtection：是否启用雪崩保护</item>
///   <item>AvalancheProtectionMaxOffsetSeconds：雪崩保护最大偏移时间（秒）</item>
///   <item>StampedeProtectionTimeoutMs：击穿保护超时时间（毫秒）</item>
///   <item>MaxConcurrentFactoryCalls：最大并发工厂调用数</item>
/// </list>
/// <para>配置来源：appsettings.json 的 HybridCache 节点</para>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "HybridCache": {
///     "DefaultExpirationMinutes": 5,
///     "EnableStatistics": true,
///     "EnableAvalancheProtection": true,
///     "AvalancheProtectionMaxOffsetSeconds": 30,
///     "StampedeProtectionTimeoutMs": 5000,
///     "MaxConcurrentFactoryCalls": 100
///   }
/// }
/// </code>
/// </example>
public class HybridCacheOption : OptionBase {
    /// <summary>
    /// 默认过期时间
    /// <para>缓存项的默认绝对过期时间（分钟）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 5 分钟</para>
    /// <para>必须大于 0</para>
    /// </remarks>
    /// <value>默认过期时间（分钟）</value>
    public int DefaultExpirationMinutes { get; set; } = 5;

    /// <summary>
    /// 是否启用统计功能
    /// <para>启用后会记录缓存命中率等统计信息</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 true</para>
    /// <para>启用统计会有轻微性能开销，建议生产环境启用</para>
    /// </remarks>
    /// <value>true 表示启用统计；false 表示禁用</value>
    public bool EnableStatistics { get; set; } = true;

    /// <summary>
    /// 是否启用雪崩保护
    /// <para>启用后会在过期时间上添加随机偏移，防止大量缓存同时失效</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 true</para>
    /// <para>雪崩保护通过在过期时间上添加随机偏移实现</para>
    /// <para>偏移范围为 0 到 AvalancheProtectionMaxOffsetSeconds 秒</para>
    /// </remarks>
    /// <value>true 表示启用雪崩保护；false 表示禁用</value>
    public bool EnableAvalancheProtection { get; set; } = true;

    /// <summary>
    /// 雪崩保护最大偏移时间
    /// <para>过期时间的随机偏移最大值（秒）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 30 秒</para>
    /// <para>实际偏移时间为 0 到此值之间的随机数</para>
    /// <para>建议值为默认过期时间的 10%-20%</para>
    /// </remarks>
    /// <value>最大偏移时间（秒）</value>
    public int AvalancheProtectionMaxOffsetSeconds { get; set; } = 30;

    /// <summary>
    /// 击穿保护超时时间
    /// <para>等待其他线程完成数据加载的最大时间（毫秒）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 5000 毫秒（5 秒）</para>
    /// <para>当缓存未命中时，只有一个线程负责加载数据</para>
    /// <para>其他线程等待此超时时间，超时后直接调用工厂方法</para>
    /// <para>值过短可能导致重复加载，过长可能影响响应时间</para>
    /// </remarks>
    /// <value>击穿保护超时时间（毫秒）</value>
    public int StampedeProtectionTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 最大并发工厂调用数
    /// <para>允许同时执行数据加载的最大并发数</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 100</para>
    /// <para>用于限制并发加载的数量，防止数据库过载</para>
    /// <para>值过小可能导致请求排队等待，过大可能导致资源竞争</para>
    /// </remarks>
    /// <value>最大并发工厂调用数</value>
    public int MaxConcurrentFactoryCalls { get; set; } = 100;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有配置项的有效性和约束条件</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>DefaultExpirationMinutes 必须大于 0</item>
    ///   <item>AvalancheProtectionMaxOffsetSeconds 必须大于等于 0</item>
    ///   <item>StampedeProtectionTimeoutMs 必须大于 0</item>
    ///   <item>MaxConcurrentFactoryCalls 必须大于 0</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateMin(DefaultExpirationMinutes, 1, nameof(DefaultExpirationMinutes),
            "混合缓存默认过期时间必须大于 0");

        ValidateMin(AvalancheProtectionMaxOffsetSeconds, 0, nameof(AvalancheProtectionMaxOffsetSeconds),
            "雪崩保护最大偏移时间必须大于等于 0");

        ValidateMin(StampedeProtectionTimeoutMs, 1, nameof(StampedeProtectionTimeoutMs),
            "击穿保护超时时间必须大于 0");

        ValidateMin(MaxConcurrentFactoryCalls, 1, nameof(MaxConcurrentFactoryCalls),
            "最大并发工厂调用数必须大于 0");
    }
}
