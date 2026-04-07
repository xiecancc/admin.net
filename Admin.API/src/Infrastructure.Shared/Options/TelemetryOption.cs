/*
 * 文件名称: TelemetryOption.cs
 * 功能描述: OpenTelemetry 遥测配置选项
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// 遥测配置选项
/// <para>配置 OpenTelemetry 分布式追踪和指标收集</para>
/// </summary>
public sealed class TelemetryOption : OptionBase {
    /// <summary>
    /// 是否启用遥测
    /// </summary>
    public bool Enabled {
        get; set;
    }

    /// <summary>
    /// OTLP 导出器端点
    /// </summary>
    public string? OtlpEndpoint {
        get; set;
    }

    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; } = "Admin.NET";

    /// <summary>
    /// 验证配置有效性
    /// </summary>
    public override void Validate() {
        if (!Enabled) {
            return;
        }

        if (!string.IsNullOrEmpty(OtlpEndpoint)) {
            ValidateUrl(OtlpEndpoint, nameof(OtlpEndpoint), "OTLP 端点格式无效");
        }
    }
}
