/*
 * 文件名称: PermissionCacheEventHandler.cs
 * 功能描述: 权限缓存事件处理器,处理权限变更时清除缓存
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Constants;
using Domain.Shared.Enums;
using Domain.Shared.Events;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 角色权限变更事件处理器
/// <para>处理角色权限变更后清除相关用户缓存</para>
/// </summary>
/// <param name="userRoleRepository">用户角色关联仓储</param>
/// <param name="permissionCacheService">权限缓存服务</param>
/// <param name="logger">日志记录器</param>
public class RolePermissionCacheEventHandler(
    IUserRoleRepository userRoleRepository,
    IPermissionCacheService permissionCacheService,
    ILogger<RolePermissionCacheEventHandler> logger) :
    IDomainEventHandler<DomainCreatedEvent<RolePermission>>,
    IDomainEventHandler<DomainUpdatedEvent<RolePermission>>,
    IDomainEventHandler<DomainDeletedEvent<RolePermission>> {

    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IPermissionCacheService _permissionCacheService = permissionCacheService;
    private readonly ILogger<RolePermissionCacheEventHandler> _logger = logger;

    /// <summary>
    /// 处理角色权限创建事件
    /// </summary>
    public async Task HandleAsync(DomainCreatedEvent<RolePermission> @event, CancellationToken cancellationToken = default) {
        await ClearRoleUsersCacheAsync(@event.Domains.Select(rp => rp.RoleId), cancellationToken);
    }

    /// <summary>
    /// 处理角色权限更新事件
    /// </summary>
    public async Task HandleAsync(DomainUpdatedEvent<RolePermission> @event, CancellationToken cancellationToken = default) {
        await ClearRoleUsersCacheAsync(@event.Domains.Select(rp => rp.RoleId), cancellationToken);
    }

    /// <summary>
    /// 处理角色权限删除事件
    /// </summary>
    public async Task HandleAsync(DomainDeletedEvent<RolePermission> @event, CancellationToken cancellationToken = default) {
        await ClearRoleUsersCacheAsync(@event.Domains.Select(rp => rp.RoleId), cancellationToken);
    }

    private async Task ClearRoleUsersCacheAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken) {
        var roleIdList = roleIds.Distinct().ToList();
        var allUserIds = new HashSet<Guid>();

        foreach (var roleId in roleIdList) {
            await _permissionCacheService.RemoveRoleInheritedPermissionsAsync(roleId, cancellationToken);

            var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
            foreach (var userId in userIds) {
                allUserIds.Add(userId);
            }
        }

        if (allUserIds.Count > 0) {
            await _permissionCacheService.RemoveUserPermissionsBatchAsync(allUserIds, cancellationToken);
        }

        _logger.LogInformation("角色权限变更，清除相关缓存 | RoleCount: {RoleCount} | AffectedUserCount: {UserCount}",
            roleIdList.Count, allUserIds.Count);
    }
}

/// <summary>
/// 用户角色变更事件处理器
/// <para>处理用户角色变更后清除用户缓存</para>
/// </summary>
/// <param name="permissionCacheService">权限缓存服务</param>
/// <param name="logger">日志记录器</param>
public class UserRoleCacheEventHandler(
    IPermissionCacheService permissionCacheService,
    ILogger<UserRoleCacheEventHandler> logger) :
    IDomainEventHandler<DomainCreatedEvent<UserRole>>,
    IDomainEventHandler<DomainUpdatedEvent<UserRole>>,
    IDomainEventHandler<DomainDeletedEvent<UserRole>> {

    private readonly IPermissionCacheService _permissionCacheService = permissionCacheService;
    private readonly ILogger<UserRoleCacheEventHandler> _logger = logger;

    /// <summary>
    /// 处理用户角色创建事件
    /// </summary>
    public async Task HandleAsync(DomainCreatedEvent<UserRole> @event, CancellationToken cancellationToken = default) {
        await ClearUsersCacheAsync(@event.Domains.Select(ur => ur.UserId), cancellationToken);
    }

    /// <summary>
    /// 处理用户角色更新事件
    /// </summary>
    public async Task HandleAsync(DomainUpdatedEvent<UserRole> @event, CancellationToken cancellationToken = default) {
        var userIds = @event.Domains.Select(ur => ur.UserId)
            .Union(@event.Domains.Select(ur => ur.UserId))
            .Distinct();
        await ClearUsersCacheAsync(userIds, cancellationToken);
    }

    /// <summary>
    /// 处理用户角色删除事件
    /// </summary>
    public async Task HandleAsync(DomainDeletedEvent<UserRole> @event, CancellationToken cancellationToken = default) {
        await ClearUsersCacheAsync(@event.Domains.Select(ur => ur.UserId), cancellationToken);
    }

    private async Task ClearUsersCacheAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken) {
        var userIdList = userIds.Distinct().ToList();
        await _permissionCacheService.RemoveUserPermissionsBatchAsync(userIdList, cancellationToken);

        _logger.LogInformation("用户角色变更，清除权限缓存 | UserCount: {UserCount}", userIdList.Count);
    }
}

/// <summary>
/// 角色变更事件处理器
/// <para>处理角色继承关系变更后清除相关缓存</para>
/// </summary>
/// <param name="permissionCacheService">权限缓存服务</param>
/// <param name="userRoleRepository">用户角色关联仓储</param>
/// <param name="roleRepository">角色仓储</param>
/// <param name="logger">日志记录器</param>
public class RoleCacheEventHandler(
    IPermissionCacheService permissionCacheService,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    ILogger<RoleCacheEventHandler> logger) :
    IDomainEventHandler<DomainUpdatedEvent<Role>>,
    IDomainEventHandler<DomainDeletedEvent<Role>> {

    private readonly IPermissionCacheService _permissionCacheService = permissionCacheService;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly ILogger<RoleCacheEventHandler> _logger = logger;

    /// <summary>
    /// 处理角色更新事件
    /// </summary>
    public async Task HandleAsync(DomainUpdatedEvent<Role> @event, CancellationToken cancellationToken = default) {
        foreach (var role in @event.Domains) {
            await ClearRoleInheritanceCacheAsync(role, cancellationToken);
        }
    }

    /// <summary>
    /// 处理角色删除事件
    /// </summary>
    public async Task HandleAsync(DomainDeletedEvent<Role> @event, CancellationToken cancellationToken = default) {
        foreach (var role in @event.Domains) {
            await ClearRoleInheritanceCacheAsync(role, cancellationToken);
        }
    }

    private async Task ClearRoleInheritanceCacheAsync(Role role, CancellationToken cancellationToken) {
        var affectedRoleIds = new HashSet<Guid> { role.Id };

        if (role.InheritanceType == InheritanceType.Upward && role.ParentId.HasValue) {
            var inheritanceChain = await _roleRepository.GetInheritanceChainAsync(role.Id, cancellationToken);
            foreach (var parentRole in inheritanceChain) {
                if (parentRole.Id != role.Id) {
                    affectedRoleIds.Add(parentRole.Id);
                }
            }
        }

        if (role.InheritanceType == InheritanceType.Downward) {
            var allChildren = await _roleRepository.GetAllChildrenAsync(role.Id, cancellationToken);
            foreach (var child in allChildren) {
                affectedRoleIds.Add(child.Id);
            }
        }

        var parentIds = await GetParentRoleIdsAsync(role.Id, cancellationToken);
        foreach (var parentId in parentIds) {
            affectedRoleIds.Add(parentId);
        }

        var allUserIds = new HashSet<Guid>();
        foreach (var roleId in affectedRoleIds) {
            await _permissionCacheService.RemoveRoleInheritedPermissionsAsync(roleId, cancellationToken);

            var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
            foreach (var userId in userIds) {
                allUserIds.Add(userId);
            }
        }

        if (allUserIds.Count > 0) {
            await _permissionCacheService.RemoveUserPermissionsBatchAsync(allUserIds, cancellationToken);
        }

        _logger.LogInformation("角色继承关系变更，清除相关缓存 | RoleId: {RoleId} | RoleCount: {RoleCount} | AffectedUserCount: {UserCount}",
            role.Id, affectedRoleIds.Count, allUserIds.Count);
    }

    private async Task<List<Guid>> GetParentRoleIdsAsync(Guid roleId, CancellationToken cancellationToken) {
        var parentIds = new List<Guid>();
        var currentRole = await _roleRepository.GetAsync(roleId, cancellationToken);

        while (currentRole?.ParentId.HasValue == true) {
            parentIds.Add(currentRole.ParentId.Value);
            currentRole = await _roleRepository.GetAsync(currentRole.ParentId.Value, cancellationToken);
        }

        return parentIds;
    }
}

/// <summary>
/// 权限变更事件处理器
/// <para>处理权限本身变更后清除所有相关缓存</para>
/// </summary>
/// <param name="permissionCacheService">权限缓存服务</param>
/// <param name="rolePermissionRepository">角色权限关联仓储</param>
/// <param name="userRoleRepository">用户角色关联仓储</param>
/// <param name="logger">日志记录器</param>
public class PermissionCacheEventHandler(
    IPermissionCacheService permissionCacheService,
    IRolePermissionRepository rolePermissionRepository,
    IUserRoleRepository userRoleRepository,
    ILogger<PermissionCacheEventHandler> logger) :
    IDomainEventHandler<DomainUpdatedEvent<Permission>>,
    IDomainEventHandler<DomainDeletedEvent<Permission>> {

    private readonly IPermissionCacheService _permissionCacheService = permissionCacheService;
    private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<PermissionCacheEventHandler> _logger = logger;

    /// <summary>
    /// 处理权限更新事件
    /// </summary>
    public async Task HandleAsync(DomainUpdatedEvent<Permission> @event, CancellationToken cancellationToken = default) {
        foreach (var permission in @event.Domains) {
            await ClearPermissionRelatedCacheAsync(permission.Id, cancellationToken);
        }
    }

    /// <summary>
    /// 处理权限删除事件
    /// </summary>
    public async Task HandleAsync(DomainDeletedEvent<Permission> @event, CancellationToken cancellationToken = default) {
        foreach (var permission in @event.Domains) {
            await ClearPermissionRelatedCacheAsync(permission.Id, cancellationToken);
        }
    }

    private async Task ClearPermissionRelatedCacheAsync(Guid permissionId, CancellationToken cancellationToken) {
        var roleIds = await _rolePermissionRepository.GetRoleIdsByPermissionIdAsync(permissionId, cancellationToken);
        if (roleIds.Count == 0) {
            _logger.LogDebug("权限未关联任何角色，无需清除缓存 | PermissionId: {PermissionId}", permissionId);
            return;
        }

        var allUserIds = new HashSet<Guid>();
        foreach (var roleId in roleIds) {
            await _permissionCacheService.RemoveRoleInheritedPermissionsAsync(roleId, cancellationToken);

            var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
            foreach (var userId in userIds) {
                allUserIds.Add(userId);
            }
        }

        if (allUserIds.Count > 0) {
            await _permissionCacheService.RemoveUserPermissionsBatchAsync(allUserIds, cancellationToken);
        }

        _logger.LogInformation("权限变更，清除相关缓存 | PermissionId: {PermissionId} | AffectedRoleCount: {RoleCount} | AffectedUserCount: {UserCount}",
            permissionId, roleIds.Count, allUserIds.Count);
    }
}
