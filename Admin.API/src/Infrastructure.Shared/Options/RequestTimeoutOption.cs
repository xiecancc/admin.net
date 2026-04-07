/*
 * 文件名称: RequestTimeoutOption.cs
 * 功能描述: 请求超时配置选项
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// 请求超时配置选项
/// <para>配置 ASP.NET Core 内置请求超时中间件</para>
/// </summary>
public sealed class RequestTimeoutOption : OptionBase {
    /// <summary>
    /// 是否启用请求超时
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 默认超时时间（秒）
    /// </summary>
    public int DefaultTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 端点级别超时策略
    /// </summary>
    public List<EndpointTimeoutPolicy> EndpointPolicies { get; set; } = [];

    /// <summary>
    /// 验证配置有效性
    /// </summary>
    public override void Validate() {
        if (!Enabled) {
            return;
        }

        ValidateMin(DefaultTimeoutSeconds, 1, nameof(DefaultTimeoutSeconds), "默认超时时间必须大于 0");

        ValidateCollection(EndpointPolicies, (policy, index) => {
            ValidateNotEmpty(policy.Name, $"{nameof(EndpointPolicies)}[{index}].{nameof(policy.Name)}", "策略名称不能为空");
            ValidateMin(policy.TimeoutSeconds, 1, $"{nameof(EndpointPolicies)}[{index}].{nameof(policy.TimeoutSeconds)}", "超时时间必须大于 0");
        }, nameof(EndpointPolicies));

        ValidateUnique(EndpointPolicies, p => p.Name, nameof(EndpointPolicies), "策略名称必须唯一");
    }
}

/// <summary>
/// 端点级别超时策略
/// </summary>
public sealed class EndpointTimeoutPolicy {
    /// <summary>
    /// 策略名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds {
        get; set;
    }

    /// <summary>
    /// 路径模式（支持通配符，如 /api/auth/*）
    /// </summary>
    public string? PathPattern {
        get; set;
    }

    /// <summary>
    /// HTTP 方法（GET、POST、PUT、DELETE 等，* 表示所有方法）
    /// </summary>
    public string? Method {
        get; set;
    }
}
