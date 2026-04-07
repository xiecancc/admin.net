/*
 * 文件名称：RedisOption.cs
 * 功能描述：Redis 配置选项类
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// Redis 配置选项
/// <para>用于配置 Redis 缓存连接和相关参数</para>
/// </summary>
/// <remarks>
/// <para>配置项说明：</para>
/// <list type="bullet">
///   <item>Enabled：是否启用 Redis（可选，默认 false）</item>
///   <item>ConnectionString：Redis 连接字符串（启用时必填）</item>
///   <item>InstanceName：实例名称（缓存键前缀，可选）</item>
///   <item>Database：数据库索引（可选，0-15）</item>
///   <item>ConnectTimeout：连接超时时间（可选，100-30000 毫秒）</item>
///   <item>SyncTimeout：同步超时时间（可选，100-30000 毫秒）</item>
///   <item>DefaultExpirationMinutes：默认过期时间（可选，默认 30 分钟）</item>
/// </list>
/// <para>配置来源：appsettings.json 的 Redis 节点</para>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "Redis": {
///     "Enabled": true,
///     "ConnectionString": "localhost:6379",
///     "InstanceName": "AdminNet_",
///     "Database": 0,
///     "ConnectTimeout": 5000,
///     "SyncTimeout": 5000
///   }
/// }
/// </code>
/// </example>
public class RedisOption : OptionBase {
    /// <summary>
    /// 是否启用 Redis
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 false</para>
    /// <para>为 false 时将使用内存缓存作为后备方案</para>
    /// </remarks>
    /// <value>true 表示启用 Redis；false 表示禁用</value>
    public bool Enabled {
        get; set;
    }

    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>当 Enabled 为 true 时必须配置</para>
    /// <para>格式示例：localhost:6379 或 192.168.1.100:6379,password=secret,ssl=true</para>
    /// </remarks>
    /// <value>Redis 连接字符串</value>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// 实例名称
    /// <para>用于区分不同应用的缓存键前缀</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为空字符串</para>
    /// <para>建议格式：应用名称 + 下划线</para>
    /// </remarks>
    /// <value>实例名称字符串</value>
    public string InstanceName { get; set; } = null!;

    /// <summary>
    /// 数据库索引
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0</para>
    /// <para>取值范围：0-15</para>
    /// <para>Redis 支持 16 个逻辑数据库（0-15），默认使用第 0 个</para>
    /// </remarks>
    /// <value>数据库索引值</value>
    public int Database {
        get; set;
    }

    /// <summary>
    /// 连接超时时间
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（使用系统默认）</para>
    /// <para>如果配置则取值范围：100-30000 毫秒</para>
    /// <para>超时时间过短可能导致连接失败，过长可能影响用户体验</para>
    /// </remarks>
    /// <value>连接超时时间（毫秒）</value>
    public int ConnectTimeout {
        get; set;
    }

    /// <summary>
    /// 同步超时时间
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（使用系统默认）</para>
    /// <para>如果配置则取值范围：100-30000 毫秒</para>
    /// <para>用于控制同步操作的超时时间</para>
    /// </remarks>
    /// <value>同步超时时间（毫秒）</value>
    public int SyncTimeout {
        get; set;
    }

    /// <summary>
    /// 默认过期时间
    /// <para>Redis 缓存的默认过期时间（分钟）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 30 分钟</para>
    /// <para>必须大于 0</para>
    /// </remarks>
    /// <value>默认过期时间（分钟）</value>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>启用时 ConnectionString 不能为空</item>
    ///   <item>Database 如果配置则必须在 0-15 之间</item>
    ///   <item>ConnectTimeout 如果配置则必须在 100-30000 毫秒之间</item>
    ///   <item>SyncTimeout 如果配置则必须在 100-30000 毫秒之间</item>
    ///   <item>DefaultExpirationMinutes 必须大于 0</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        if (Enabled) {
            ValidateNotEmpty(ConnectionString, nameof(ConnectionString), "Redis 启用时连接字符串必须配置");
        }

        // 可选配置：只有配置了才验证
        if (Database != 0) {
            ValidateRange(Database, 0, 15, nameof(Database), "Redis 数据库索引必须在 0-15 之间");
        }

        if (ConnectTimeout != 0) {
            ValidateRange(ConnectTimeout, 100, 30000, nameof(ConnectTimeout),
                "Redis 连接超时时间必须在 100-30000 毫秒之间");
        }

        if (SyncTimeout != 0) {
            ValidateRange(SyncTimeout, 100, 30000, nameof(SyncTimeout),
                "Redis 同步超时时间必须在 100-30000 毫秒之间");
        }

        ValidateMin(DefaultExpirationMinutes, 1, nameof(DefaultExpirationMinutes),
            "Redis 默认过期时间必须大于 0");
    }
}
