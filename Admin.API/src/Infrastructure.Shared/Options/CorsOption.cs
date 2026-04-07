/*
 * 文件名称：CorsOption.cs
 * 功能描述：跨域配置选项类，用于配置跨域资源共享（CORS）策略
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

namespace Infrastructure.Shared.Options;

/// <summary>
/// 跨域配置选项
/// <para>用于配置跨域资源共享（CORS）策略</para>
/// </summary>
/// <remarks>
/// <para>配置来源：appsettings.json 的 Cors 节点</para>
/// <para>安全建议：</para>
/// <list type="bullet">
///   <item>生产环境应明确指定允许的源，避免使用 AllowAnyOrigin</item>
///   <item>AllowCredentials 与 AllowAnyOrigin 不能同时为 true</item>
///   <item>仅允许必要的 HTTP 方法和请求头</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // appsettings.json 配置示例
/// {
///   "Cors": {
///     "AllowedOrigins": ["https://example.com", "https://api.example.com"],
///     "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
///     "AllowedHeaders": ["Content-Type", "Authorization"],
///     "AllowCredentials": true,
///     "AllowAnyOrigin": false
///   }
/// }
/// </code>
/// </example>
public class CorsOption : OptionBase {
    /// <summary>
    /// 允许的源列表
    /// <para>指定允许跨域访问的源（域名）</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为空列表</para>
    /// <para>每个源必须是完整的 URL，包含协议（http 或 https）</para>
    /// <para>示例：["https://example.com", "https://api.example.com"]</para>
    /// <para>当 AllowAnyOrigin 为 true 时，此配置将被忽略</para>
    /// </remarks>
    /// <value>允许的源 URL 列表</value>
    public List<string> AllowedOrigins { get; set; } = [];

    /// <summary>
    /// 允许的 HTTP 方法列表
    /// <para>指定跨域请求允许使用的 HTTP 方法</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为空列表</para>
    /// <para>常见的 HTTP 方法：GET、POST、PUT、DELETE、PATCH、OPTIONS</para>
    /// <para>为空时允许所有方法</para>
    /// </remarks>
    /// <value>允许的 HTTP 方法列表</value>
    public List<string> AllowedMethods { get; set; } = [];

    /// <summary>
    /// 允许的请求头列表
    /// <para>指定跨域请求允许携带的请求头</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为空列表</para>
    /// <para>常见的请求头：Content-Type、Authorization、Accept</para>
    /// <para>为空时允许所有请求头</para>
    /// </remarks>
    /// <value>允许的请求头列表</value>
    public List<string> AllowedHeaders { get; set; } = [];

    /// <summary>
    /// 是否允许携带凭据
    /// <para>控制跨域请求是否可以携带 Cookie、HTTP 认证等凭据</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为 false</para>
    /// <para>当为 true 时：</para>
    /// <list type="bullet">
    ///   <item>允许客户端发送 Cookie 和 HTTP 认证数据</item>
    ///   <item>AllowAnyOrigin 必须为 false</item>
    /// </list>
    /// <para>安全建议：仅在需要用户会话的场景下启用</para>
    /// </remarks>
    /// <value>是否允许携带凭据</value>
    public bool AllowCredentials {
        get; set;
    }

    /// <summary>
    /// 是否允许任意源访问
    /// <para>控制是否允许来自任何源的跨域请求</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为 false</para>
    /// <para>当为 true 时：</para>
    /// <list type="bullet">
    ///   <item>允许来自任何源的跨域请求</item>
    ///   <item>AllowedOrigins 配置将被忽略</item>
    ///   <item>AllowCredentials 必须为 false</item>
    /// </list>
    /// <para>安全警告：生产环境不建议启用此选项，应明确指定允许的源</para>
    /// </remarks>
    /// <value>是否允许任意源访问</value>
    public bool AllowAnyOrigin {
        get; set;
    }

    /// <summary>
    /// 暴露给客户端的响应头列表
    /// <para>指定哪些响应头可以被客户端 JavaScript 访问</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为空列表</para>
    /// <para>默认情况下，浏览器只允许 JavaScript 访问标准响应头</para>
    /// <para>如果服务器返回自定义响应头，需要在此配置才能被客户端读取</para>
    /// <para>常见的暴露响应头：X-Trace-Id、X-Request-Id、Retry-After</para>
    /// </remarks>
    /// <value>暴露的响应头列表</value>
    public List<string> ExposedHeaders { get; set; } = [];

    /// <summary>
    /// 预检请求缓存时间（秒）
    /// <para>指定预检请求（OPTIONS 请求）的缓存时间</para>
    /// </summary>
    /// <remarks>
    /// <para>可选配置，默认为 0（不缓存）</para>
    /// <para>预检请求是浏览器在发送实际请求前，先发送 OPTIONS 请求询问服务器是否允许跨域</para>
    /// <para>配置缓存时间可以减少预检请求次数，降低网络延迟和服务器负载</para>
    /// <para>建议值：300-600 秒（5-10 分钟）</para>
    /// <para>注意事项：</para>
    /// <list type="bullet">
    ///   <item>过长的缓存时间可能导致 CORS 策略更新不及时</item>
    ///   <item>部分浏览器有最大缓存时间限制（如 Chrome 约为 10 分钟）</item>
    /// </list>
    /// </remarks>
    /// <value>预检请求缓存时间（秒）</value>
    public int PreflightMaxAge {
        get; set;
    }

    /// <summary>
    /// 验证配置有效性
    /// </summary>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    public override void Validate() {
        ValidateCondition(!(AllowCredentials && AllowAnyOrigin),
            "AllowCredentials 与 AllowAnyOrigin 不能同时为 true，这是 CORS 规范的安全限制");

        ValidateCollection(AllowedOrigins, (origin, index) => {
            ValidateCondition(!string.IsNullOrWhiteSpace(origin),
                $"AllowedOrigins[{index}]: 源地址不能为空");

            ValidateUrl(origin, $"AllowedOrigins[{index}]",
                $"AllowedOrigins[{index}]: 源地址 '{origin}' 格式无效，必须是有效的 URL（如 https://example.com）");
        }, nameof(AllowedOrigins));
    }
}
