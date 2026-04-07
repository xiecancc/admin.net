/*
 * 文件名称：RateLimitOption.cs
 * 功能描述：请求限流配置选项类，支持 ASP.NET Core 10.0 内置限流中间件
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using Infrastructure.Shared.Utils;

namespace Infrastructure.Shared.Options;

/// <summary>
/// 限流算法类型
/// <para>定义限流的算法策略</para>
/// </summary>
/// <remarks>
/// <para>算法类型说明：</para>
/// <list type="bullet">
///   <item>FixedWindow：固定窗口算法，在固定时间窗口内限制请求数量</item>
///   <item>SlidingWindow：滑动窗口算法，将时间窗口分成多个段，提高限流精度</item>
///   <item>TokenBucket：令牌桶算法，以固定速率生成令牌，请求需要消耗令牌</item>
///   <item>Concurrency：并发限流算法，限制同时处理的请求数量</item>
/// </list>
/// </remarks>
public enum RateLimitAlgorithmType {
    /// <summary>
    /// 固定窗口算法
    /// </summary>
    /// <remarks>
    /// <para>在固定时间窗口内限制请求数量</para>
    /// <para>优点：实现简单，内存占用小</para>
    /// <para>缺点：可能出现边界突发问题</para>
    /// </remarks>
    FixedWindow,

    /// <summary>
    /// 滑动窗口算法
    /// </summary>
    /// <remarks>
    /// <para>将时间窗口分成多个段，提高限流精度</para>
    /// <para>优点：限流更平滑，避免边界突发问题</para>
    /// <para>缺点：内存占用稍大</para>
    /// </remarks>
    SlidingWindow,

    /// <summary>
    /// 令牌桶算法
    /// </summary>
    /// <remarks>
    /// <para>以固定速率生成令牌，请求需要消耗令牌</para>
    /// <para>优点：允许一定程度的突发流量</para>
    /// <para>缺点：需要配置令牌生成速率</para>
    /// </remarks>
    TokenBucket,

    /// <summary>
    /// 并发限流算法
    /// </summary>
    /// <remarks>
    /// <para>限制同时处理的请求数量</para>
    /// <para>优点：保护系统资源，防止过载</para>
    /// <para>缺点：不适用于请求时间差异大的场景</para>
    /// </remarks>
    Concurrency
}

/// <summary>
/// 限流分区类型
/// <para>定义限流的分区策略</para>
/// </summary>
/// <remarks>
/// <para>分区类型说明：</para>
/// <list type="bullet">
///   <item>User：按用户分区，每个用户独立的限流计数</item>
///   <item>Ip：按 IP 地址分区，每个 IP 独立的限流计数</item>
///   <item>Global：全局分区，所有请求共享限流计数</item>
///   <item>Header：按请求头分区，根据指定 Header 值独立限流</item>
/// </list>
/// </remarks>
public enum RateLimitPartitionType {
    /// <summary>
    /// 按用户分区
    /// </summary>
    /// <remarks>
    /// <para>每个用户独立的限流计数</para>
    /// <para>需要用户已认证，未认证用户将使用空字符串作为分区键</para>
    /// </remarks>
    User,

    /// <summary>
    /// 按 IP 地址分区
    /// </summary>
    /// <remarks>
    /// <para>每个 IP 独立的限流计数</para>
    /// <para>适用于防止恶意攻击和滥用</para>
    /// </remarks>
    Ip,

    /// <summary>
    /// 全局分区
    /// </summary>
    /// <remarks>
    /// <para>所有请求共享限流计数</para>
    /// <para>适用于保护系统整体资源</para>
    /// </remarks>
    Global,

    /// <summary>
    /// 按请求头分区
    /// </summary>
    /// <remarks>
    /// <para>根据指定 Header 值独立限流</para>
    /// <para>需要配置 HeaderName 属性指定 Header 名称</para>
    /// </remarks>
    Header
}

/// <summary>
/// 限流队列处理顺序
/// <para>定义排队请求的处理顺序</para>
/// </summary>
/// <remarks>
/// <para>处理顺序说明：</para>
/// <list type="bullet">
///   <item>OldestFirst：先进先出，先排队的请求先处理</item>
///   <item>NewestFirst：后进先出，后排队的请求先处理</item>
/// </list>
/// </remarks>
public enum RateLimitQueueProcessingOrder {
    /// <summary>
    /// 先进先出
    /// </summary>
    /// <remarks>
    /// <para>先排队的请求先处理</para>
    /// <para>公平性更好，适用于大多数场景</para>
    /// </remarks>
    OldestFirst,

    /// <summary>
    /// 后进先出
    /// </summary>
    /// <remarks>
    /// <para>后排队的请求先处理</para>
    /// <para>可能导致旧请求超时，适用于特定场景</para>
    /// </remarks>
    NewestFirst
}

/// <summary>
/// 请求限流配置选项
/// <para>用于配置 API 请求限流策略</para>
/// </summary>
/// <remarks>
/// <para>职责：提供限流策略、分区方式、拒绝响应等配置</para>
/// <para>配置来源：appsettings.json 的 RateLimit 节点</para>
/// <para>配置说明：</para>
/// <list type="bullet">
///   <item>必填配置：DefaultPolicy</item>
///   <item>可选配置：Enabled、PartitionType、HeaderName、OnRejected、EndpointPolicies</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "RateLimit": {
///     "Enabled": true,
///     "PartitionType": "Ip",
///     "OnRejected": {
///       "StatusCode": 429,
///       "Message": "请求过于频繁，请稍后再试"
///     },
///     "DefaultPolicy": {
///       "Algorithm": "FixedWindow",
///       "PermitLimit": 100,
///       "WindowSeconds": 60
///     },
///     "EndpointPolicies": [
///       {
///         "PathPattern": "/api/auth/*",
///         "Method": "POST",
///         "Algorithm": "FixedWindow",
///         "PermitLimit": 10,
///         "WindowSeconds": 60
///       }
///     ]
///   }
/// }
/// </code>
/// </example>
public class RateLimitOption : OptionBase {
    /// <summary>
    /// 是否启用限流
    /// <para>控制是否启用请求限流功能</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 true</para>
    /// <para>为 false 时所有请求不受限流限制</para>
    /// </remarks>
    /// <value>true 表示启用限流；false 表示禁用</value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否启用分布式限流
    /// <para>控制是否使用分布式缓存进行限流计数</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 false</para>
    /// <para>为 true 时使用 ICacheProvider 进行限流计数，支持多实例部署</para>
    /// <para>为 false 时使用内存限流，仅适用于单实例部署</para>
    /// <para>依赖条件：需要 Redis 缓存配置可用</para>
    /// </remarks>
    /// <value>true 表示启用分布式限流；false 表示使用内存限流</value>
    public bool EnableDistributedLimiter {
        get; set;
    }

    /// <summary>
    /// 分区类型（字符串，用于配置绑定）
    /// <para>定义限流的分区策略</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 Ip</para>
    /// <para>可选值：User、Ip、Global、Header</para>
    /// <para>当设置为 Header 时，必须同时配置 HeaderName 属性</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>分区类型字符串</value>
    public string? PartitionType {
        get; init;
    }

    /// <summary>
    /// Header 名称
    /// <para>用于根据请求头值进行限流分区</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null</para>
    /// <para>当 PartitionType 为 Header 时必填</para>
    /// </remarks>
    /// <value>Header 名称字符串</value>
    public string? HeaderName {
        get; set;
    }

    /// <summary>
    /// 自定义拒绝响应配置
    /// <para>自定义请求被限流时的响应内容</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时使用默认响应</para>
    /// </remarks>
    /// <value>拒绝响应配置对象</value>
    public RateLimitOnRejectedOption? OnRejected {
        get; set;
    }

    /// <summary>
    /// 默认限流策略
    /// <para>应用于所有未匹配到特定端点策略的请求</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>作为全局默认限流策略</para>
    /// </remarks>
    /// <value>默认限流策略配置</value>
    public RateLimitPolicyOption DefaultPolicy { get; set; } = null!;

    /// <summary>
    /// 端点限流策略列表
    /// <para>针对特定端点和 HTTP 方法的限流策略</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null</para>
    /// <para>策略名称必须唯一</para>
    /// </remarks>
    /// <value>端点限流策略列表</value>
    public List<EndpointRateLimitOption>? EndpointPolicies {
        get; set;
    }

    /// <summary>
    /// 分区类型枚举（只读，类型安全访问）
    /// <para>将字符串 PartitionType 转换为 RateLimitPartitionType 枚举</para>
    /// </summary>
    /// <remarks>
    /// <para>此属性为只读属性，提供类型安全的枚举访问</para>
    /// <para>当 PartitionType 为 null 或无效值时返回默认值 Ip</para>
    /// </remarks>
    /// <value>RateLimitPartitionType 枚举值</value>
    public RateLimitPartitionType PartitionTypeEnum =>
        EnumUtil.ToEnumNullable<RateLimitPartitionType>(PartitionType, RateLimitPartitionType.Ip) ?? RateLimitPartitionType.Ip;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>DefaultPolicy 不能为空（必填）</item>
    ///   <item>DefaultPolicy 必须通过验证</item>
    ///   <item>PartitionType 必须是有效的枚举值（可选）</item>
    ///   <item>当 PartitionType 为 Header 时，HeaderName 不能为空</item>
    ///   <item>OnRejected 如果配置则必须通过验证（可选）</item>
    ///   <item>所有 EndpointPolicies 必须通过验证（可选）</item>
    ///   <item>EndpointPolicies 中的策略名称必须唯一</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateNotNull(DefaultPolicy, nameof(DefaultPolicy), "默认限流策略必须配置");
        DefaultPolicy.Validate();

        if (!string.IsNullOrEmpty(PartitionType)) {
            _ = PartitionTypeEnum;
        }

        if (PartitionTypeEnum == RateLimitPartitionType.Header) {
            ValidateNotEmpty(HeaderName!, nameof(HeaderName), "当分区类型为 Header 时，Header 名称必须配置");
        }

        OnRejected?.Validate();

        if (EndpointPolicies != null && EndpointPolicies.Count > 0) {
            for (int i = 0; i < EndpointPolicies.Count; i++) {
                var endpoint = EndpointPolicies[i];
                ValidateNotNull(endpoint, $"{nameof(EndpointPolicies)}[{i}]",
                    $"端点策略 [{i}] 不能为空");
                endpoint.Validate();
            }

            ValidateUnique(
                EndpointPolicies.Where(e => !string.IsNullOrWhiteSpace(e.PolicyName)),
                e => e.PolicyName!,
                nameof(EndpointPolicies),
                $"端点限流策略名称必须唯一，发现重复的名称");
        }
    }
}

/// <summary>
/// 限流策略配置
/// <para>定义限流的基本参数</para>
/// </summary>
/// <remarks>
/// <para>职责：配置限流算法、请求限制、时间窗口等参数</para>
/// <para>配置说明：</para>
/// <list type="bullet">
///   <item>必填配置：PermitLimit、WindowSeconds</item>
///   <item>可选配置：Name、Algorithm、SegmentsPerWindow、QueueLimit、QueueProcessingOrder</item>
/// </list>
/// </remarks>
public class RateLimitPolicyOption : OptionBase {
    /// <summary>
    /// 策略名称
    /// <para>用于标识和引用此策略</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null</para>
    /// </remarks>
    /// <value>策略名称字符串</value>
    public string? Name {
        get; set;
    }

    /// <summary>
    /// 算法类型（字符串，用于配置绑定）
    /// <para>定义限流的算法策略</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 FixedWindow</para>
    /// <para>可选值：FixedWindow、SlidingWindow、TokenBucket、Concurrency</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>算法类型字符串</value>
    public string? Algorithm {
        get; init;
    }

    /// <summary>
    /// 允许的请求数量
    /// <para>在时间窗口内允许的最大请求数</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>取值范围：1-10000</para>
    /// <para>对于 TokenBucket 算法，表示令牌桶容量</para>
    /// <para>对于 Concurrency 算法，表示最大并发数</para>
    /// </remarks>
    /// <value>允许的请求数量</value>
    public int PermitLimit {
        get; set;
    }

    /// <summary>
    /// 时间窗口（秒）
    /// <para>限流策略的时间窗口大小</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>取值范围：1-3600 秒</para>
    /// <para>对于 TokenBucket 算法，配合 TokensPerPeriod 使用</para>
    /// <para>对于 Concurrency 算法，此配置无效</para>
    /// </remarks>
    /// <value>时间窗口（秒）</value>
    public int WindowSeconds {
        get; set;
    }

    /// <summary>
    /// 窗口分段数
    /// <para>仅用于 SlidingWindow 算法</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（使用默认分段）</para>
    /// <para>如果配置则取值范围：1-10</para>
    /// </remarks>
    /// <value>窗口分段数</value>
    public int SegmentsPerWindow {
        get; set;
    }

    /// <summary>
    /// 每周期补充令牌数
    /// <para>仅用于 TokenBucket 算法</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（使用 PermitLimit 作为补充速率）</para>
    /// <para>如果配置则取值范围：1-PermitLimit</para>
    /// <para>建议值：设置为期望的平均速率，PermitLimit 设置为允许的最大突发</para>
    /// <para>示例：PermitLimit=100, TokensPerPeriod=10, WindowSeconds=1</para>
    /// <para>效果：平均速率 10 请求/秒，最大突发 100 请求</para>
    /// </remarks>
    /// <value>每周期补充令牌数</value>
    public int TokensPerPeriod {
        get; set;
    }

    /// <summary>
    /// 排队限制
    /// <para>允许排队的最大请求数</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（不允许排队）</para>
    /// <para>如果配置则取值范围：0-1000</para>
    /// </remarks>
    /// <value>排队限制</value>
    public int QueueLimit {
        get; set;
    }

    /// <summary>
    /// 队列处理顺序（字符串，用于配置绑定）
    /// <para>定义排队请求的处理顺序</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 OldestFirst</para>
    /// <para>可选值：OldestFirst、NewestFirst</para>
    /// <para>仅当 QueueLimit 大于 0 时有效</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>队列处理顺序字符串</value>
    public string? QueueProcessingOrder {
        get; init;
    }

    /// <summary>
    /// 算法类型枚举（只读，类型安全访问）
    /// <para>将字符串 Algorithm 转换为 RateLimitAlgorithmType 枚举</para>
    /// </summary>
    /// <remarks>
    /// <para>此属性为只读属性，提供类型安全的枚举访问</para>
    /// <para>当 Algorithm 为 null 或无效值时返回默认值 FixedWindow</para>
    /// </remarks>
    /// <value>RateLimitAlgorithmType 枚举值</value>
    public RateLimitAlgorithmType AlgorithmEnum =>
        EnumUtil.ToEnumNullable<RateLimitAlgorithmType>(Algorithm, RateLimitAlgorithmType.FixedWindow) ?? RateLimitAlgorithmType.FixedWindow;

    /// <summary>
    /// 队列处理顺序枚举（只读，类型安全访问）
    /// <para>将字符串 QueueProcessingOrder 转换为 RateLimitQueueProcessingOrder 枚举</para>
    /// </summary>
    /// <remarks>
    /// <para>此属性为只读属性，提供类型安全的枚举访问</para>
    /// <para>当 QueueProcessingOrder 为 null 或无效值时返回默认值 OldestFirst</para>
    /// </remarks>
    /// <value>RateLimitQueueProcessingOrder 枚举值</value>
    public RateLimitQueueProcessingOrder QueueProcessingOrderEnum =>
        EnumUtil.ToEnumNullable<RateLimitQueueProcessingOrder>(QueueProcessingOrder, RateLimitQueueProcessingOrder.OldestFirst) ?? RateLimitQueueProcessingOrder.OldestFirst;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>PermitLimit 必须在 1-10000 之间（必填）</item>
    ///   <item>WindowSeconds 必须在 1-3600 之间（必填）</item>
    ///   <item>Algorithm 必须是有效的枚举值（可选）</item>
    ///   <item>SegmentsPerWindow 如果配置则必须在 1-10 之间（可选）</item>
    ///   <item>QueueLimit 如果配置则必须在 0-1000 之间（可选）</item>
    ///   <item>QueueProcessingOrder 必须是有效的枚举值（可选）</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateRange(PermitLimit, 1, 10000, nameof(PermitLimit),
            "限流策略允许的请求数量必须在 1-10000 之间");

        ValidateRange(WindowSeconds, 1, 3600, nameof(WindowSeconds),
            "限流策略时间窗口必须在 1-3600 秒之间");

        if (!string.IsNullOrEmpty(Algorithm)) {
            _ = AlgorithmEnum;
        }

        if (SegmentsPerWindow != 0) {
            ValidateRange(SegmentsPerWindow, 1, 10, nameof(SegmentsPerWindow),
                "限流策略窗口分段数必须在 1-10 之间");
        }

        if (QueueLimit != 0) {
            ValidateRange(QueueLimit, 0, 1000, nameof(QueueLimit),
                "限流策略排队限制必须在 0-1000 之间");
        }

        if (!string.IsNullOrEmpty(QueueProcessingOrder)) {
            _ = QueueProcessingOrderEnum;
        }
    }
}

/// <summary>
/// 端点限流配置
/// <para>针对特定端点和 HTTP 方法的限流策略</para>
/// </summary>
/// <remarks>
/// <para>职责：配置特定端点的限流策略</para>
/// <para>配置说明：</para>
/// <list type="bullet">
///   <item>必填配置：PathPattern、Method、PermitLimit、WindowSeconds</item>
///   <item>可选配置：PolicyName、Algorithm、SegmentsPerWindow、QueueLimit</item>
/// </list>
/// </remarks>
public class EndpointRateLimitOption : OptionBase {
    /// <summary>
    /// 端点路径模式
    /// <para>支持通配符 * 匹配多个端点</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>示例：/api/auth/* 匹配所有认证相关端点</para>
    /// </remarks>
    /// <value>端点路径模式字符串</value>
    public string PathPattern { get; set; } = null!;

    /// <summary>
    /// HTTP 方法
    /// <para>指定限流的 HTTP 方法</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>可选值：GET、POST、PUT、DELETE、PATCH 等</para>
    /// <para>* 表示匹配所有 HTTP 方法</para>
    /// </remarks>
    /// <value>HTTP 方法字符串</value>
    public string Method { get; set; } = null!;

    /// <summary>
    /// 限流策略名称
    /// <para>用于标识此端点策略</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null</para>
    /// <para>如果配置则必须唯一</para>
    /// </remarks>
    /// <value>限流策略名称字符串</value>
    public string? PolicyName {
        get; set;
    }

    /// <summary>
    /// 算法类型（字符串，用于配置绑定）
    /// <para>定义限流的算法策略</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 FixedWindow</para>
    /// <para>可选值：FixedWindow、SlidingWindow、TokenBucket、Concurrency</para>
    /// <para>此属性为 init-only，仅用于配置文件绑定</para>
    /// </remarks>
    /// <value>算法类型字符串</value>
    public string? Algorithm {
        get; init;
    }

    /// <summary>
    /// 允许的请求数量
    /// <para>在时间窗口内允许的最大请求数</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>取值范围：1-10000</para>
    /// </remarks>
    /// <value>允许的请求数量</value>
    public int PermitLimit {
        get; set;
    }

    /// <summary>
    /// 时间窗口（秒）
    /// <para>限流策略的时间窗口大小</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>取值范围：1-3600 秒</para>
    /// </remarks>
    /// <value>时间窗口（秒）</value>
    public int WindowSeconds {
        get; set;
    }

    /// <summary>
    /// 窗口分段数
    /// <para>仅用于 SlidingWindow 算法</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0</para>
    /// <para>如果配置则取值范围：1-10</para>
    /// </remarks>
    /// <value>窗口分段数</value>
    public int SegmentsPerWindow {
        get; set;
    }

    /// <summary>
    /// 每周期补充令牌数
    /// <para>仅用于 TokenBucket 算法</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0（使用 PermitLimit 作为补充速率）</para>
    /// <para>如果配置则取值范围：1-PermitLimit</para>
    /// <para>建议值：设置为期望的平均速率，PermitLimit 设置为允许的最大突发</para>
    /// </remarks>
    /// <value>每周期补充令牌数</value>
    public int TokensPerPeriod {
        get; set;
    }

    /// <summary>
    /// 排队限制
    /// <para>允许排队的最大请求数</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0</para>
    /// <para>如果配置则取值范围：0-1000</para>
    /// </remarks>
    /// <value>排队限制</value>
    public int QueueLimit {
        get; set;
    }

    /// <summary>
    /// 算法类型枚举（只读，类型安全访问）
    /// <para>将字符串 Algorithm 转换为 RateLimitAlgorithmType 枚举</para>
    /// </summary>
    /// <remarks>
    /// <para>此属性为只读属性，提供类型安全的枚举访问</para>
    /// <para>当 Algorithm 为 null 或无效值时返回默认值 FixedWindow</para>
    /// </remarks>
    /// <value>RateLimitAlgorithmType 枚举值</value>
    public RateLimitAlgorithmType AlgorithmEnum =>
        EnumUtil.ToEnumNullable<RateLimitAlgorithmType>(Algorithm, RateLimitAlgorithmType.FixedWindow) ?? RateLimitAlgorithmType.FixedWindow;

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>PathPattern 不能为空（必填）</item>
    ///   <item>Method 不能为空（必填）</item>
    ///   <item>PermitLimit 必须在 1-10000 之间（必填）</item>
    ///   <item>WindowSeconds 必须在 1-3600 之间（必填）</item>
    ///   <item>Algorithm 必须是有效的枚举值（可选）</item>
    ///   <item>SegmentsPerWindow 如果配置则必须在 1-10 之间（可选）</item>
    ///   <item>QueueLimit 如果配置则必须在 0-1000 之间（可选）</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateNotEmpty(PathPattern, nameof(PathPattern), "端点限流配置路径模式必须配置");
        ValidateNotEmpty(Method, nameof(Method), "端点限流配置 HTTP 方法必须配置");

        ValidateRange(PermitLimit, 1, 10000, nameof(PermitLimit),
            "端点限流允许的请求数量必须在 1-10000 之间");

        ValidateRange(WindowSeconds, 1, 3600, nameof(WindowSeconds),
            "端点限流时间窗口必须在 1-3600 秒之间");

        if (!string.IsNullOrEmpty(Algorithm)) {
            _ = AlgorithmEnum;
        }

        if (SegmentsPerWindow != 0) {
            ValidateRange(SegmentsPerWindow, 1, 10, nameof(SegmentsPerWindow),
                "端点限流窗口分段数必须在 1-10 之间");
        }

        if (QueueLimit != 0) {
            ValidateRange(QueueLimit, 0, 1000, nameof(QueueLimit),
                "端点限流排队限制必须在 0-1000 之间");
        }
    }
}

/// <summary>
/// 限流拒绝响应配置
/// <para>自定义请求被限流时的响应内容</para>
/// </summary>
/// <remarks>
/// <para>职责：配置限流响应的状态码和消息</para>
/// <para>配置说明：</para>
/// <list type="bullet">
///   <item>可选配置：StatusCode、Message</item>
/// </list>
/// </remarks>
public class RateLimitOnRejectedOption : OptionBase {
    /// <summary>
    /// HTTP 状态码
    /// <para>请求被限流时返回的 HTTP 状态码</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 429</para>
    /// <para>取值范围：100-999</para>
    /// <para>建议使用 429 (Too Many Requests)</para>
    /// </remarks>
    /// <value>HTTP 状态码</value>
    public int StatusCode { get; set; } = 429;

    /// <summary>
    /// 错误消息
    /// <para>请求被限流时返回的错误消息</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 null</para>
    /// </remarks>
    /// <value>错误消息字符串</value>
    public string? Message {
        get; set;
    }

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>StatusCode 必须在 100-999 之间</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateRange(StatusCode, 100, 999, nameof(StatusCode),
            "HTTP 状态码必须在 100-999 之间");
    }
}
