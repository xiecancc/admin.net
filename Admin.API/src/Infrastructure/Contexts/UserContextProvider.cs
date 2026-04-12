/*
 * 文件名称: UserContextProvider.cs
 * 功能描述: 用户上下文提供者实现，从 HttpContext 中获取用户信息和验证权限
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-09
 */

using Domain.Services;
using Infrastructure.Shared.Contexts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Contexts;

/// <summary>
/// 用户上下文提供者实现，从 HttpContext 中获取用户信息和验证权限
/// </summary>
/// <remarks>
/// <para>职责：</para>
/// <list type="bullet">
///   <item>提供当前用户的信息</item>
///   <item>验证当前用户的权限</item>
/// </list>
/// <para>依赖：</para>
/// <list type="bullet">
///   <item>IHttpContextAccessor - HTTP 上下文访问器</item>
///   <item>IPermissionDomainService - 权限领域服务</item>
/// </list>
/// </remarks>
/// <param name="httpContextAccessor">HTTP 上下文访问器</param>
/// <param name="permissionService">权限领域服务</param>
public class UserContextProvider(
    IHttpContextAccessor httpContextAccessor,
    IPermissionDomainService permissionService) : IUserContextProvider {
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IPermissionDomainService _permissionService = permissionService;

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    /// <inheritdoc/>
    public Guid? UserId {
        get {
            var userIdClaim = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public async Task<bool> HasPermissionAsync(string permissionCode, CancellationToken cancellationToken = default) {
        if (UserId == null) {
            return false;
        }
        return await _permissionService.UserHasPermissionAsync(UserId.Value, permissionCode, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> HasAnyPermissionAsync(string[] permissionCodes, CancellationToken cancellationToken = default) {
        if (UserId == null) {
            return false;
        }
        foreach (var code in permissionCodes) {
            var hasPermission = await _permissionService.UserHasPermissionAsync(UserId.Value, code, cancellationToken);
            if (hasPermission) {
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> HasAllPermissionsAsync(string[] permissionCodes, CancellationToken cancellationToken = default) {
        if (UserId == null) {
            return false;
        }
        foreach (var code in permissionCodes) {
            var hasPermission = await _permissionService.UserHasPermissionAsync(UserId.Value, code, cancellationToken);
            if (!hasPermission) {
                return false;
            }
        }
        return true;
    }
}
