/*
 * 文件名称: RolePermissionRepository.cs
 * 功能描述: 角色权限关联仓储实现，用于处理角色和权限之间的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IRolePermissionRepository"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="logger">日志记录器</param>
public class RolePermissionRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IHttpContextProvider httpContextProvider,
    ILogger<RolePermissionRepository> logger)
    : DomainRepository<RolePermission>(client, eventBus, httpContextProvider, "RolePermission", logger), IRolePermissionRepository {
    /// <inheritdoc/>
    public async Task<List<Guid>> GetPermissionIdsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default) {
        return await _client.Queryable<RolePermission>()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default) {
        var permissionIds = await GetPermissionIdsByRoleIdAsync(roleId, cancellationToken);
        
        if (permissionIds.Count == 0) {
            return [];
        }

        return await _client.Queryable<Permission>()
            .Where(p => permissionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetRoleIdsByPermissionIdAsync(Guid permissionId, CancellationToken cancellationToken = default) {
        return await _client.Queryable<RolePermission>()
            .Where(rp => rp.PermissionId == permissionId)
            .Select(rp => rp.RoleId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default) {
        if (permissionIds.Count == 0) {
            return true;
        }

        var rolePermissions = permissionIds.Select(permissionId => new RolePermission {
            RoleId = roleId,
            PermissionId = permissionId
        }).ToList();

        return await InsertAsync(rolePermissions, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AssignRolesToPermissionAsync(Guid permissionId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        if (roleIds.Count == 0) {
            return true;
        }

        var rolePermissions = roleIds.Select(roleId => new RolePermission {
            RoleId = roleId,
            PermissionId = permissionId
        }).ToList();

        return await InsertAsync(rolePermissions, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> RemovePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default) {
        return permissionIds.Count == 0
            ? true
            : await DeleteAsync(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRolesFromPermissionAsync(Guid permissionId, List<Guid> roleIds, CancellationToken cancellationToken = default) {
        return roleIds.Count == 0
            ? true
            : await DeleteAsync(rp => rp.PermissionId == permissionId && roleIds.Contains(rp.RoleId), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default) {
        return await ExistsAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
    }
}
