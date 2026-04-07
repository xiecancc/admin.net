/*
 * 文件名称：JwtOption.cs
 * 功能描述：JWT 配置选项类
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// JWT 配置选项
/// <para>用于配置 JWT 令牌的生成和验证参数</para>
/// <para>主要功能：令牌配置、安全设置、过期时间</para>
/// </summary>
/// <remarks>
/// <para>配置项说明：</para>
/// <list type="bullet">
///   <item>SecretKey：JWT 签名密钥（必填，至少 32 字符）</item>
///   <item>Issuer：令牌签发者（必填）</item>
///   <item>Audience：令牌受众（必填）</item>
///   <item>ExpiresInMinutes：令牌过期时间（可选，1-1440 分钟）</item>
///   <item>RefreshTokenExpiresInDays：刷新令牌过期时间（可选，1-365 天）</item>
/// </list>
/// <para>配置来源：appsettings.json 的 Jwt 节点</para>
/// <para>安全建议：
/// <list type="bullet">
///   <item>生产环境应使用至少 32 字符的强密钥</item>
///   <item>使用环境变量或密钥管理系统存储密钥</item>
///   <item>定期更换密钥</item>
///   <item>使用 HTTPS 传输令牌</item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "Jwt": {
///     "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
///     "Issuer": "Admin.NET",
///     "Audience": "Admin.NET",
///     "ExpiresInMinutes": 60,
///     "RefreshTokenExpiresInDays": 7
///   }
/// }
/// </code>
/// </example>
public class JwtOption : OptionBase {
    /// <summary>
    /// JWT 签名密钥
    /// <para>用于生成和验证 JWT 令牌的密钥</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>安全要求：长度至少 32 字符，建议包含大小写字母、数字和特殊字符</para>
    /// </remarks>
    /// <value>JWT 签名密钥字符串</value>
    public string SecretKey { get; set; } = null!;

    /// <summary>
    /// 令牌签发者
    /// <para>标识令牌的签发方</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>通常设置为应用程序的名称或 URL</para>
    /// </remarks>
    /// <value>令牌签发者字符串</value>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// 令牌受众
    /// <para>标识令牌的接收方</para>
    /// </summary>
    /// <remarks>
    /// <para>必填配置</para>
    /// <para>通常设置为应用程序的名称或 URL</para>
    /// </remarks>
    /// <value>令牌受众字符串</value>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// 令牌过期时间
    /// <para>JWT 令牌的有效期（分钟）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0，需要配置后才能生效</para>
    /// <para>取值范围：1-1440 分钟（1 分钟到 24 小时）</para>
    /// <para>根据应用程序的安全需求调整，建议 30-120 分钟</para>
    /// </remarks>
    /// <value>令牌过期时间（分钟）</value>
    public int ExpiresInMinutes {
        get; set;
    }

    /// <summary>
    /// 刷新令牌过期时间
    /// <para>用于获取新 JWT 令牌的刷新令牌的有效期（天）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置</para>
    /// <para>不配置时默认值为 0，需要配置后才能生效</para>
    /// <para>取值范围：1-365 天</para>
    /// <para>刷新令牌通常比访问令牌有效期长，但不建议超过 30 天</para>
    /// </remarks>
    /// <value>刷新令牌过期时间（天）</value>
    public int RefreshTokenExpiresInDays {
        get; set;
    }

    /// <summary>
    /// 验证配置有效性
    /// <para>检查所有必填配置项和配置约束</para>
    /// </summary>
    /// <remarks>
    /// 验证规则：
    /// <list type="bullet">
    ///   <item>SecretKey 不能为空，长度至少 32 字符（必填）</item>
    ///   <item>Issuer 不能为空（必填）</item>
    ///   <item>Audience 不能为空（必填）</item>
    ///   <item>ExpiresInMinutes 如果配置则必须在 1-1440 之间（可选）</item>
    ///   <item>RefreshTokenExpiresInDays 如果配置则必须在 1-365 之间（可选）</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateNotEmpty(SecretKey, nameof(SecretKey), "JWT 签名密钥必须配置");
        ValidateLength(SecretKey, 32, null, nameof(SecretKey), "JWT 签名密钥长度至少为 32 个字符，建议使用强密钥");

        ValidateNotEmpty(Issuer, nameof(Issuer), "令牌签发者必须配置");
        ValidateNotEmpty(Audience, nameof(Audience), "令牌受众必须配置");

        if (ExpiresInMinutes != 0) {
            ValidateRange(ExpiresInMinutes, 1, 1440, nameof(ExpiresInMinutes),
                "令牌过期时间必须在 1-1440 分钟之间（1 分钟到 24 小时）");
        }

        if (RefreshTokenExpiresInDays != 0) {
            ValidateRange(RefreshTokenExpiresInDays, 1, 365, nameof(RefreshTokenExpiresInDays),
                "刷新令牌过期时间必须在 1-365 天之间");
        }
    }
}
