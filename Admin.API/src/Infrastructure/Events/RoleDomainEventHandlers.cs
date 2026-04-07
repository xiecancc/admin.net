/*
 * 文件名称: RoleDomainEventHandlers.cs
 * 功能描述: 角色领域事件处理器，处理角色相关事件并清除缓存
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Shared.Constants;
using Domain.Shared.Events;
using Infrastructure.Shared.Caches;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 角色创建事件处理器
/// <para>处理角色创建后的缓存清除</para>
/// </summary>
public class RoleCreatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<RoleCreatedEventHandler> logger) : DomainEventHandlerBase<RoleCreatedEventHandler>(logger), IDomainEventHandler<DomainCreatedEvent<Role>> {
    /// <summary>
    /// 处理角色创建事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainCreatedEvent<Role> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("创建", @event.Description, @event.Domains.Count());

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除角色列表和分页缓存（新增 {Count} 个角色）", @event.Domains.Count());
    }
}

/// <summary>
/// 角色更新事件处理器
/// <para>处理角色更新后的缓存清除</para>
/// </summary>
public class RoleUpdatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<RoleUpdatedEventHandler> logger) : DomainEventHandlerBase<RoleUpdatedEventHandler>(logger), IDomainEventHandler<DomainUpdatedEvent<Role>> {
    /// <summary>
    /// 处理角色更新事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainUpdatedEvent<Role> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("更新", @event.Description, @event.Domains.Count());

        var roleTasks = @event.Domains.Select(role => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Role.Detail(role.Id));
            Logger.LogInformation("已清除角色 {RoleId} 的缓存", role.Id);
        }));

        await Task.WhenAll(roleTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除角色列表和分页缓存");
    }
}

/// <summary>
/// 角色删除事件处理器
/// <para>处理角色删除后的缓存清除</para>
/// </summary>
public class RoleDeletedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<RoleDeletedEventHandler> logger) : DomainEventHandlerBase<RoleDeletedEventHandler>(logger), IDomainEventHandler<DomainDeletedEvent<Role>> {
    /// <summary>
    /// 处理角色删除事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainDeletedEvent<Role> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("删除", @event.Description, @event.Domains.Count());

        var roleTasks = @event.Domains.Select(role => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Role.Detail(role.Id));
            Logger.LogInformation("已清除角色 {RoleId} 的缓存", role.Id);
        }));

        await Task.WhenAll(roleTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除角色列表和分页缓存");
    }
}

/// <summary>
/// 角色恢复事件处理器
/// <para>处理角色恢复后的缓存清除</para>
/// </summary>
public class RoleRestoredEventHandler(
    ICacheProvider cacheProvider,
    ILogger<RoleRestoredEventHandler> logger) : DomainEventHandlerBase<RoleRestoredEventHandler>(logger), IDomainEventHandler<DomainRestoredEvent<Role>> {
    /// <summary>
    /// 处理角色恢复事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainRestoredEvent<Role> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("恢复", @event.Description, @event.Domains.Count());

        var roleTasks = @event.Domains.Select(role => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Role.Detail(role.Id));
            Logger.LogInformation("已清除角色 {RoleId} 的缓存", role.Id);
        }));

        await Task.WhenAll(roleTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Role.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除角色列表和分页缓存");
    }
}

/// <summary>
/// 角色权限分配事件处理器
/// <para>处理角色权限分配后的缓存清除</para>
/// </summary>
public class RolePermissionsAssignedEventHandler(
    ICacheProvider cacheProvider,
    IUserRoleRepository userRoleRepository,
    ILogger<RolePermissionsAssignedEventHandler> logger) : DomainEventHandlerBase<RolePermissionsAssignedEventHandler>(logger), IDomainEventHandler<RolePermissionsAssignedEvent> {
    /// <summary>
    /// 处理角色权限分配事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(RolePermissionsAssignedEvent @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("权限分配", @event.Description, @event.PermissionIds.Count());

        await cacheProvider.RemoveAsync(CacheKeyConstants.Role.Permissions(@event.RoleId));
        Logger.LogInformation("已清除角色 {RoleId} 的权限缓存", @event.RoleId);

        var userIds = await userRoleRepository.GetUserIdsByRoleIdAsync(@event.RoleId, cancellationToken);
        var userTasks = userIds.Select(userId => Task.WhenAll(
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(userId)),
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Permissions(userId))
        ));

        await Task.WhenAll(userTasks);
        Logger.LogInformation("已清除关联用户的缓存");
    }
}

/// <summary>
/// 角色权限撤销事件处理器
/// <para>处理角色权限撤销后的缓存清除</para>
/// </summary>
public class RolePermissionsRevokedEventHandler(
    ICacheProvider cacheProvider,
    IUserRoleRepository userRoleRepository,
    ILogger<RolePermissionsRevokedEventHandler> logger) : DomainEventHandlerBase<RolePermissionsRevokedEventHandler>(logger), IDomainEventHandler<RolePermissionsRevokedEvent> {
    /// <summary>
    /// 处理角色权限撤销事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(RolePermissionsRevokedEvent @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("权限撤销", @event.Description, @event.PermissionIds.Count());

        await cacheProvider.RemoveAsync(CacheKeyConstants.Role.Permissions(@event.RoleId));
        Logger.LogInformation("已清除角色 {RoleId} 的权限缓存", @event.RoleId);

        var userIds = await userRoleRepository.GetUserIdsByRoleIdAsync(@event.RoleId, cancellationToken);
        var userTasks = userIds.Select(userId => Task.WhenAll(
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(userId)),
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Permissions(userId))
        ));

        await Task.WhenAll(userTasks);
        Logger.LogInformation("已清除关联用户的缓存");
    }
}
