/*
 * 文件名称：MemoryCacheOption.cs
 * 功能描述：内存缓存配置选项类
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// 内存缓存配置选项
/// <para>用于配置内存缓存相关参数</para>
/// </summary>
/// <remarks>
/// <para>配置项说明：</para>
/// <list type="bullet">
///   <item>DefaultExpirationMinutes：默认过期时间（可选，默认 5 分钟）</item>
///   <item>SlidingExpirationMinutes：滑动过期时间（可选，null 表示不启用）</item>
///   <item>SizeLimit：缓存大小限制（可选，null 表示不限制）</item>
///   <item>CompactionPercentage：压缩百分比（可选，默认 0.05）</item>
///   <item>ExpirationScanFrequencyMinutes：过期扫描频率（可选，默认 1 分钟）</item>
/// </list>
/// <para>配置来源：appsettings.json 的 MemoryCache 节点</para>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "MemoryCache": {
///     "DefaultExpirationMinutes": 5,
///     "SlidingExpirationMinutes": 10,
///     "SizeLimit": 1073741824,
///     "CompactionPercentage": 0.05,
///     "ExpirationScanFrequencyMinutes": 1
///   }
/// }
/// </code>
/// </example>
public class MemoryCacheOption : OptionBase {
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
    /// 滑动过期时间
    /// <para>每次访问缓存项时重置过期时间（分钟）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null，表示不启用滑动过期</para>
    /// <para>如果配置则必须大于 0</para>
    /// <para>滑动过期与绝对过期同时存在时，以先到者为准</para>
    /// </remarks>
    /// <value>滑动过期时间（分钟），null 表示不启用</value>
    public int? SlidingExpirationMinutes {
        get; set;
    }

    /// <summary>
    /// 缓存大小限制
    /// <para>内存缓存的最大大小（字节）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null，表示不限制大小</para>
    /// <para>如果配置则必须大于 0</para>
    /// <para>达到限制时会根据 CompactionPercentage 进行压缩</para>
    /// <para>建议值：1073741824（1GB）或根据服务器内存配置</para>
    /// </remarks>
    /// <value>缓存大小限制（字节），null 表示不限制</value>
    public long? SizeLimit {
        get; set;
    }

    /// <summary>
    /// 压缩百分比
    /// <para>当缓存达到大小限制时，压缩的缓存项百分比</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0.05（5%）</para>
    /// <para>取值范围：0-1 之间</para>
    /// <para>值越大，压缩时移除的缓存项越多</para>
    /// </remarks>
    /// <value>压缩百分比（0-1）</value>
    public double CompactionPercentage { get; set; } = 0.05;

    /// <summary>
    /// 过期扫描频率
    /// <para>扫描并移除过期缓存项的时间间隔（分钟）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 1 分钟</para>
    /// <para>必须大于 0</para>
    /// <para>扫描频率过低可能导致过期缓存项占用内存</para>
    /// <para>扫描频率过高可能影响性能</para>
    /// </remarks>
    /// <value>过期扫描频率（分钟）</value>
    public int ExpirationScanFrequencyMinutes { get; set; } = 1;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有配置项的有效性和约束条件</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>DefaultExpirationMinutes 必须大于 0</item>
    ///   <item>SlidingExpirationMinutes 如果配置则必须大于 0</item>
    ///   <item>SizeLimit 如果配置则必须大于 0</item>
    ///   <item>CompactionPercentage 必须在 0-1 之间</item>
    ///   <item>ExpirationScanFrequencyMinutes 必须大于 0</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateMin(DefaultExpirationMinutes, 1, nameof(DefaultExpirationMinutes),
            "内存缓存默认过期时间必须大于 0");

        if (SlidingExpirationMinutes.HasValue) {
            ValidateMin(SlidingExpirationMinutes.Value, 1, nameof(SlidingExpirationMinutes),
                "内存缓存滑动过期时间必须大于 0");
        }

        if (SizeLimit.HasValue) {
            ValidateMin((int)SizeLimit.Value, 1, nameof(SizeLimit),
                "内存缓存大小限制必须大于 0");
        }

        ValidateRange(CompactionPercentage, 0.0, 1.0, nameof(CompactionPercentage),
            "内存缓存压缩百分比必须在 0-1 之间");

        ValidateMin(ExpirationScanFrequencyMinutes, 1, nameof(ExpirationScanFrequencyMinutes),
            "内存缓存过期扫描频率必须大于 0");
    }
}
