/*
 * 文件名称: PermissionCacheEventHandler.cs
 * 功能描述: 权限缓存事件处理器,处理权限变更时清除缓存
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 角色权限变更事件处理器
/// <para>处理角色权限变更后清除相关用户缓存</para>
/// </summary>
public class RolePermissionCacheEventHandler(
    ICacheProvider cacheProvider,
    IUserRoleRepository userRoleRepository,
    ILogger<RolePermissionCacheEventHandler> logger) : 
    IDomainEventHandler<DomainCreatedEvent<RolePermission>>,
    IDomainEventHandler<DomainUpdatedEvent<RolePermission>>,
    IDomainEventHandler<DomainDeletedEvent<RolePermission>> {
    
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<RolePermissionCacheEventHandler> _logger = logger;
    private const string CACHE_KEY_PREFIX = "user_permissions:";

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
        foreach (var roleId in roleIds.Distinct()) {
            var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(roleId, cancellationToken);
            foreach (var userId in userIds) {
                var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
                await _cacheProvider.RemoveAsync(cacheKey);
                _logger.LogInformation("清除用户权限缓存 | UserId: {UserId} | RoleId: {RoleId}", userId, roleId);
            }
        }
    }
}

/// <summary>
/// 用户角色变更事件处理器
/// <para>处理用户角色变更后清除用户缓存</para>
/// </summary>
public class UserRoleCacheEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserRoleCacheEventHandler> logger) : 
    IDomainEventHandler<DomainCreatedEvent<UserRole>>,
    IDomainEventHandler<DomainUpdatedEvent<UserRole>>,
    IDomainEventHandler<DomainDeletedEvent<UserRole>> {
    
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private readonly ILogger<UserRoleCacheEventHandler> _logger = logger;
    private const string CACHE_KEY_PREFIX = "user_permissions:";

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
        await ClearUsersCacheAsync(@event.Domains.Select(ur => ur.UserId), cancellationToken);
    }

    /// <summary>
    /// 处理用户角色删除事件
    /// </summary>
    public async Task HandleAsync(DomainDeletedEvent<UserRole> @event, CancellationToken cancellationToken = default) {
        await ClearUsersCacheAsync(@event.Domains.Select(ur => ur.UserId), cancellationToken);
    }

    private async Task ClearUsersCacheAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken) {
        foreach (var userId in userIds.Distinct()) {
            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
            await _cacheProvider.RemoveAsync(cacheKey);
            _logger.LogInformation("清除用户权限缓存 | UserId: {UserId}", userId);
        }
    }
}
