/*
 * 文件名称：DatabaseOption.cs
 * 功能描述：数据库配置选项类，提供 SqlSugar 数据库连接的完整配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using Infrastructure.Shared.Utils;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Infrastructure.Shared.Options;

/// <summary>
/// 数据库配置选项
/// <para>用于配置 SqlSugar 数据库连接和相关行为</para>
/// </summary>
/// <remarks>
/// <para>职责：提供数据库连接、日志、性能监控等配置</para>
/// <para>配置来源：appsettings.json 的 Database 节点</para>
/// <para>配置说明：
/// <list type="bullet">
///   <item>必填配置：ConnectionString、DbType</item>
///   <item>可选配置：其他配置项，不配置时使用类型默认值</item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "Database": {
///     "ConnectionString": "Server=localhost;Database=Admin;Uid=root;Pwd=123456;",
///     "DbType": "MySql",
///     "InitKeyType": "Attribute",
///     "LogLevel": "Information",
///     "LanguageType": "Chinese"
///   }
/// }
/// </code>
/// </example>
public class DatabaseOption : OptionBase {
    /// <summary>
    /// 数据库连接字符串
    /// <para>用于连接数据库的完整连接信息</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>格式示例：Server=localhost;Database=Admin;Uid=root;Pwd=123456;Port=3306;SslMode=None;</para>
    /// </remarks>
    /// <value>数据库连接字符串</value>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// 数据库类型（字符串，用于配置绑定）
    /// <para>指定使用的数据库类型</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>可选值：MySql、SqlServer、Sqlite、PostgreSQL、Oracle、Dm、Kdbndp、Oscar、MySqlConnector、Access、OpenGauss、QuestDB、HG、ClickHouse、GBase、Odbc、Custom</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>数据库类型字符串</value>
    public string DbType { get; init; } = null!;

    /// <summary>
    /// 是否自动关闭连接
    /// <para>控制数据库操作后是否自动关闭连接</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 false</para>
    /// <para>推荐配置为 true，自动关闭连接可以防止连接泄漏</para>
    /// </remarks>
    /// <value>true 表示自动关闭连接；false 表示手动管理连接</value>
    public bool IsAutoCloseConnection {
        get; set;
    }

    /// <summary>
    /// 主键类型（字符串，用于配置绑定）
    /// <para>指定主键和索引的识别方式</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认使用 Attribute（特性方式）</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>主键类型字符串</value>
    public string InitKeyType { get; init; } = null!;

    /// <summary>
    /// 命令超时时间
    /// <para>数据库命令执行的超时时间（秒）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（无超时）</para>
    /// <para>建议根据业务需求设置合理的超时时间</para>
    /// </remarks>
    /// <value>超时时间（秒）</value>
    public int CommandTimeout {
        get; set;
    }

    /// <summary>
    /// 是否启用 SQL 日志
    /// <para>控制是否记录 SQL 执行日志</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 false</para>
    /// <para>生产环境建议关闭，开发环境可开启用于调试</para>
    /// </remarks>
    /// <value>true 表示启用 SQL 日志；false 表示禁用</value>
    public bool EnableLog {
        get; set;
    }

    /// <summary>
    /// SQL 日志级别（字符串，用于配置绑定）
    /// <para>控制 SQL 日志的输出级别</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认使用 Information</para>
    /// <para>可选值：Debug、Information、Warning、Error、Trace</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>日志级别字符串</value>
    public string LogLevel { get; init; } = null!;

    /// <summary>
    /// 是否启用慢查询检测
    /// <para>控制是否检测并记录执行缓慢的 SQL 查询</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 false</para>
    /// <para>生产环境建议开启，用于识别性能瓶颈</para>
    /// </remarks>
    /// <value>true 表示启用慢查询检测；false 表示禁用</value>
    public bool EnablePerformanceAnalyze {
        get; set;
    }

    /// <summary>
    /// 慢查询阈值
    /// <para>判定为慢查询的执行时间阈值（毫秒）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0</para>
    /// <para>建议根据业务性能要求设置，推荐 500-1000 毫秒</para>
    /// </remarks>
    /// <value>慢查询阈值（毫秒）</value>
    public int PerformanceAnalyzeThreshold {
        get; set;
    }

    /// <summary>
    /// 是否启用读写分离
    /// <para>控制是否使用主从数据库架构</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 false</para>
    /// <para>启用后必须配置 SlaveConnectionStrings 从库连接字符串</para>
    /// </remarks>
    /// <value>true 表示启用读写分离；false 表示禁用</value>
    public bool EnableReadWriteSplit {
        get; set;
    }

    /// <summary>
    /// 从库连接字符串配置列表
    /// <para>用于读写分离架构的从数据库连接配置</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null</para>
    /// <para>当 EnableReadWriteSplit 为 true 时，此列表不能为空</para>
    /// </remarks>
    /// <value>从库连接字符串配置列表</value>
    public List<SlaveConnectionStringConfig>? SlaveConnectionStrings {
        get; set;
    }

    /// <summary>
    /// SqlSugar 错误提示语言（字符串，用于配置绑定）
    /// <para>控制 SqlSugar 异常和错误信息的显示语言</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认使用 Default</para>
    /// <para>可选值：Chinese（中文）、English（英文）、Default（默认）</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>语言类型字符串</value>
    public string? LanguageType {
        get; init;
    }

    /// <summary>
    /// 数据库类型枚举（只读，类型安全访问）
    /// <para>将字符串 DbType 转换为 SqlSugar.DbType 枚举</para>
    /// </summary>
    /// <remarks>
    /// 此属性为只读属性，提供类型安全的枚举访问
    /// <para>当 DbType 字符串为无效值时抛出 InvalidOperationException</para>
    /// </remarks>
    /// <value>SqlSugar.DbType 枚举值</value>
    /// <exception cref="InvalidOperationException">当 DbType 为无效值时抛出</exception>
    public SqlSugar.DbType DbTypeEnum => EnumUtil.ToEnum<SqlSugar.DbType>(DbType);

    /// <summary>
    /// 主键类型枚举（只读，类型安全访问）
    /// <para>将字符串 InitKeyType 转换为 SqlSugar.InitKeyType 枚举</para>
    /// </summary>
    /// <remarks>
    /// 此属性为只读属性，提供类型安全的枚举访问
    /// <para>当 InitKeyType 字符串为无效值时抛出 InvalidOperationException</para>
    /// </remarks>
    /// <value>SqlSugar.InitKeyType 枚举值</value>
    /// <exception cref="InvalidOperationException">当 InitKeyType 为无效值时抛出</exception>
    public SqlSugar.InitKeyType InitKeyTypeEnum => EnumUtil.ToEnum<SqlSugar.InitKeyType>(InitKeyType);

    /// <summary>
    /// 日志级别枚举（只读，类型安全访问）
    /// <para>将字符串 LogLevel 转换为 MsLogLevel 枚举</para>
    /// </summary>
    /// <remarks>
    /// 此属性为只读属性，提供类型安全的枚举访问
    /// <para>当 LogLevel 字符串为无效值或 null 时返回默认值 Information</para>
    /// </remarks>
    /// <value>MsLogLevel 枚举值</value>
    public MsLogLevel LogLevelEnum => EnumUtil.ToEnumNullable<MsLogLevel>(LogLevel, MsLogLevel.Information) ?? MsLogLevel.Information;

    /// <summary>
    /// 语言类型枚举（只读，类型安全访问）
    /// <para>将字符串 LanguageType 转换为 SqlSugar.LanguageType 枚举</para>
    /// </summary>
    /// <remarks>
    /// 此属性为只读属性，提供类型安全的枚举访问
    /// <para>当 LanguageType 为 null 或无效值时返回 Default</para>
    /// </remarks>
    /// <value>SqlSugar.LanguageType 枚举值</value>
    public SqlSugar.LanguageType LanguageTypeEnum => EnumUtil.ToEnumNullable<SqlSugar.LanguageType>(LanguageType, SqlSugar.LanguageType.Default) ?? SqlSugar.LanguageType.Default;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>ConnectionString 不能为空（必填）</item>
    ///   <item>DbType 必须是有效的数据库类型（必填）</item>
    ///   <item>InitKeyType 必须是有效的主键类型（可选，默认 Attribute）</item>
    ///   <item>CommandTimeout 如果配置则必须大于 0</item>
    ///   <item>PerformanceAnalyzeThreshold 如果配置则必须大于 0</item>
    ///   <item>启用读写分离时必须配置从库连接字符串</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateNotEmpty(ConnectionString, nameof(ConnectionString), "数据库连接字符串必须配置");
        ValidateNotEmpty(DbType, nameof(DbType), "数据库类型必须配置");

        _ = DbTypeEnum;

        if (!string.IsNullOrEmpty(InitKeyType)) {
            _ = InitKeyTypeEnum;
        }

        if (CommandTimeout != 0) {
            ValidateMin(CommandTimeout, 1, nameof(CommandTimeout), "命令超时时间必须大于 0");
        }

        if (EnablePerformanceAnalyze && PerformanceAnalyzeThreshold != 0) {
            ValidateMin(PerformanceAnalyzeThreshold, 1, nameof(PerformanceAnalyzeThreshold), "慢查询阈值必须大于 0");
        }

        if (EnableReadWriteSplit) {
            ValidateCondition(SlaveConnectionStrings != null && SlaveConnectionStrings.Count > 0,
                "启用读写分离时必须配置从库连接字符串");
        }
    }
}

/// <summary>
/// 从库连接字符串配置
/// <para>用于读写分离架构的从数据库配置</para>
/// </summary>
/// <remarks>
/// 职责：配置从库连接字符串和命中率权重
/// </remarks>
/// <example>
/// <code>
/// var slaveConfig = new SlaveConnectionStringConfig {
///     ConnectionString = "Server=slave;Database=Admin;Uid=root;Pwd=123456;",
///     HitRate = 2
/// };
/// </code>
/// </example>
public class SlaveConnectionStringConfig {
    /// <summary>
    /// 从库连接字符串
    /// <para>用于连接从数据库的完整连接信息</para>
    /// </summary>
    /// <remarks>
    /// <para>格式与主库连接字符串相同</para>
    /// </remarks>
    /// <value>从库连接字符串</value>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// 命中率权重
    /// <para>控制从库被选中的概率权重</para>
    /// </summary>
    /// <remarks>
    /// <para>权重越高，被选中的概率越大</para>
    /// </remarks>
    /// <value>命中率权重值</value>
    public int HitRate {
        get; set;
    }
}
