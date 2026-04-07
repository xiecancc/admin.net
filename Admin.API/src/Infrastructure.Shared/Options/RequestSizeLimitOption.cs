/*
 * 文件名称: RequestSizeLimitOption.cs
 * 功能描述: 请求体大小限制配置选项
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// 请求体大小限制配置选项
/// <para>配置 ASP.NET Core 内置请求体大小限制</para>
/// </summary>
public sealed class RequestSizeLimitOption : OptionBase {
    /// <summary>
    /// 是否启用请求体大小限制
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 全局最大请求体大小（字节），默认 30MB
    /// </summary>
    public long MaxRequestBodySizeBytes { get; set; } = 30 * 1024 * 1024;

    /// <summary>
    /// Multipart 表单最大长度（字节），默认 30MB
    /// </summary>
    public long MultipartBodyLengthLimitBytes { get; set; } = 30 * 1024 * 1024;

    /// <summary>
    /// 端点级别大小限制策略
    /// </summary>
    public List<EndpointSizeLimitPolicy> EndpointPolicies { get; set; } = [];

    /// <summary>
    /// 验证配置有效性
    /// </summary>
    public override void Validate() {
        if (!Enabled) {
            return;
        }

        ValidateMin((int)(MaxRequestBodySizeBytes / 1024), 1, nameof(MaxRequestBodySizeBytes), "最大请求体大小必须大于 0");
        ValidateMin((int)(MultipartBodyLengthLimitBytes / 1024), 1, nameof(MultipartBodyLengthLimitBytes), "Multipart 表单最大长度必须大于 0");

        ValidateCollection(EndpointPolicies, (policy, index) => {
            ValidateNotEmpty(policy.Name, $"{nameof(EndpointPolicies)}[{index}].{nameof(policy.Name)}", "策略名称不能为空");
            ValidateMin((int)(policy.MaxRequestBodySizeBytes / 1024), 1, $"{nameof(EndpointPolicies)}[{index}].{nameof(policy.MaxRequestBodySizeBytes)}", "最大请求体大小必须大于 0");
        }, nameof(EndpointPolicies));

        ValidateUnique(EndpointPolicies, p => p.Name, nameof(EndpointPolicies), "策略名称必须唯一");
    }
}

/// <summary>
/// 端点级别大小限制策略
/// </summary>
public sealed class EndpointSizeLimitPolicy {
    /// <summary>
    /// 策略名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 最大请求体大小（字节）
    /// </summary>
    public long MaxRequestBodySizeBytes {
        get; set;
    }

    /// <summary>
    /// 路径模式（支持通配符，如 /api/upload/*）
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
