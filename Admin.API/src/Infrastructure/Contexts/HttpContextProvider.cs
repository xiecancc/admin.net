/*
 * 文件名称: HttpContextProvider.cs
 * 功能描述: HTTP 上下文提供者实现，从 HttpContext 中获取上下文信息
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-09
 */

using Infrastructure.Shared.Contexts;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Infrastructure.Contexts;

/// <summary>
/// HTTP 上下文提供者实现，从 HttpContext 中获取上下文信息
/// </summary>
/// <remarks>
/// <para>职责：</para>
/// <list type="bullet">
///   <item>提供当前请求的上下文信息</item>
/// </list>
/// <para>依赖：</para>
/// <list type="bullet">
///   <item>IHttpContextAccessor - HTTP 上下文访问器</item>
/// </list>
/// </remarks>
/// <param name="httpContextAccessor">HTTP 上下文访问器</param>
public class HttpContextProvider(
    IHttpContextAccessor httpContextAccessor) : IHttpContextProvider {
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    /// <inheritdoc/>
    public string? TraceId => Activity.Current?.TraceId.ToString() ?? HttpContext?.TraceIdentifier;

    /// <inheritdoc/>
    public string? RequestId => HttpContext?.TraceIdentifier;

    /// <inheritdoc/>
    public string? ClientIp => HttpContext?.Connection?.RemoteIpAddress?.ToString();

    /// <inheritdoc/>
    public string? UserAgent => HttpContext?.Request.Headers["User-Agent"].ToString();

    /// <inheritdoc/>
    public string? RequestPath => HttpContext?.Request.Path.Value;

    /// <inheritdoc/>
    public string? RequestMethod => HttpContext?.Request.Method;
}
