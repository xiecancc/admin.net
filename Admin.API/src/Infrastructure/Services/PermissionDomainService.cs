// ==================================================================================================
// FileName: PermissionDomainService.cs
// 功能描述: 权限领域服务实现，处理跨聚合的权限相关业务逻辑
// 作    者: Admin.NET
// 最近修订: 2026-04-05
// ==================================================================================================

using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Domain.Shared.Enums;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Services;

/// <inheritdoc cref="IPermissionDomainService"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="userRoleRepository">用户角色关联仓储</param>
/// <param name="rolePermissionRepository">角色权限关联仓储</param>
/// <param name="permissionCacheService">权限缓存服务</param>
/// <param name="logger">日志记录器</param>
public class PermissionDomainService(
    ISqlSugarClient client,
    IUserRoleRepository userRoleRepository,
    IRolePermissionRepository rolePermissionRepository,
    IPermissionCacheService permissionCacheService,
    ILogger<PermissionDomainService> logger) : IPermissionDomainService {
    private readonly ILogger<PermissionDomainService> _logger = logger;
    /// <inheritdoc/>
    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(permissionCode);

        // 先从缓存获取权限
        var cachedPermissions = await permissionCacheService.GetUserPermissionsAsync(userId, cancellationToken);
        if (cachedPermissions != null) {
            var hasPermission = cachedPermissions.Contains(permissionCode);
            _logger.LogDebug("从缓存校验权限 | UserId: {UserId} | PermissionCode: {PermissionCode} | Result: {Result}", 
                userId, permissionCode, hasPermission);
            return hasPermission;
        }

        // 缓存未命中,从数据库查询
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(userId, cancellationToken);
        if (roleIds.Count == 0) {
            return false;
        }

        var hasPermissionDb = await client.Queryable<RolePermission>()
            .InnerJoin<Permission>((rp, p) => rp.PermissionId == p.Id)
            .Where((rp, p) => roleIds.Contains(rp.RoleId) && p.Code == permissionCode)
            .AnyAsync(cancellationToken);

        _logger.LogDebug("从数据库校验权限 | UserId: {UserId} | PermissionCode: {PermissionCode} | Result: {Result}", 
            userId, permissionCode, hasPermissionDb);

        return hasPermissionDb;
    }

    /// <inheritdoc/>
    public async Task<bool> UserHasPermissionAsync(Guid userId, Guid permissionId, CancellationToken cancellationToken = default) {
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(userId, cancellationToken);
        return roleIds.Count == 0
            ? false
            : await rolePermissionRepository.RoleHasPermissionAsync(roleIds.First(), permissionId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default) {
        // 先从缓存获取权限
        var cachedPermissions = await permissionCacheService.GetUserPermissionsAsync(userId, cancellationToken);
        if (cachedPermissions != null) {
            _logger.LogDebug("从缓存获取用户权限列表 | UserId: {UserId} | PermissionCount: {Count}", 
                userId, cachedPermissions.Count);
            return cachedPermissions.ToList();
        }

        // 缓存未命中,从数据库查询
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(userId, cancellationToken);
        if (roleIds.Count == 0) {
            return [];
        }

        var permissionCodes = await client.Queryable<RolePermission>()
            .InnerJoin<Permission>((rp, p) => rp.PermissionId == p.Id)
            .Where((rp, p) => roleIds.Contains(rp.RoleId))
            .Select((rp, p) => p.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

        // 缓存查询结果
        if (permissionCodes.Count > 0) {
            await permissionCacheService.SetUserPermissionsAsync(userId, new HashSet<string>(permissionCodes), cancellationToken: cancellationToken);
            _logger.LogDebug("缓存用户权限列表 | UserId: {UserId} | PermissionCount: {Count}", 
                userId, permissionCodes.Count);
        }

        return permissionCodes;
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetUserPermissionIdsAsync(Guid userId, CancellationToken cancellationToken = default) {
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(userId, cancellationToken);
        if (roleIds.Count == 0) {
            return [];
        }

        var permissionIds = await client.Queryable<RolePermission>()
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissionIds;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetRolePermissionCodesAsync(Guid roleId, CancellationToken cancellationToken = default) {
        var permissionCodes = await client.Queryable<RolePermission>()
            .InnerJoin<Permission>((rp, p) => rp.PermissionId == p.Id)
            .Where((rp, p) => rp.RoleId == roleId)
            .Select((rp, p) => p.Code)
            .ToListAsync(cancellationToken);

        return permissionCodes;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetPermissionCodesByTypeAsync(PermissionType permissionType, CancellationToken cancellationToken = default) {
        var permissionCodes = await client.Queryable<Permission>()
            .Where(p => p.Type == permissionType)
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        return permissionCodes;
    }
}
