/*
 * 文件名称: PermissionDomainEventHandlers.cs
 * 功能描述: 权限领域事件处理器，处理权限相关事件并清除缓存
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Domain.Entities;
using Domain.Shared.Constants;
using Domain.Shared.Events;
using Infrastructure.Shared.Caches;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

#region 菜单权限事件处理器

/// <summary>
/// 菜单权限创建事件处理器
/// <para>处理菜单权限创建后的缓存清除</para>
/// </summary>
public class MenuPermissionCreatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionCreatedEventHandler> logger) : DomainEventHandlerBase<MenuPermissionCreatedEventHandler>(logger), IDomainEventHandler<DomainCreatedEvent<MenuPermission>> {
    /// <summary>
    /// 处理菜单权限创建事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainCreatedEvent<MenuPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("创建", "菜单权限", @event.Description, @event.Domains.Count());

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.MENU_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除菜单权限列表和分页缓存（新增 {Count} 个）", @event.Domains.Count());
    }
}

/// <summary>
/// 菜单权限更新事件处理器
/// <para>处理菜单权限更新后的缓存清除</para>
/// </summary>
public class MenuPermissionUpdatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionUpdatedEventHandler> logger) : DomainEventHandlerBase<MenuPermissionUpdatedEventHandler>(logger), IDomainEventHandler<DomainUpdatedEvent<MenuPermission>> {
    /// <summary>
    /// 处理菜单权限更新事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainUpdatedEvent<MenuPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("更新", "菜单权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除菜单权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.MENU_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除菜单权限列表和分页缓存");
    }
}

/// <summary>
/// 菜单权限删除事件处理器
/// <para>处理菜单权限删除后的缓存清除</para>
/// </summary>
public class MenuPermissionDeletedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionDeletedEventHandler> logger) : DomainEventHandlerBase<MenuPermissionDeletedEventHandler>(logger), IDomainEventHandler<DomainDeletedEvent<MenuPermission>> {
    /// <summary>
    /// 处理菜单权限删除事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainDeletedEvent<MenuPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("删除", "菜单权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除菜单权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.MENU_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除菜单权限列表和分页缓存");
    }
}

/// <summary>
/// 菜单权限恢复事件处理器
/// <para>处理菜单权限恢复后的缓存清除</para>
/// </summary>
public class MenuPermissionRestoredEventHandler(
    ICacheProvider cacheProvider,
    ILogger<MenuPermissionRestoredEventHandler> logger) : DomainEventHandlerBase<MenuPermissionRestoredEventHandler>(logger), IDomainEventHandler<DomainRestoredEvent<MenuPermission>> {
    /// <summary>
    /// 处理菜单权限恢复事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainRestoredEvent<MenuPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("恢复", "菜单权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除菜单权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.MENU_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除菜单权限列表和分页缓存");
    }
}

#endregion

#region API 权限事件处理器

/// <summary>
/// API 权限创建事件处理器
/// <para>处理 API 权限创建后的缓存清除</para>
/// </summary>
public class ApiPermissionCreatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ApiPermissionCreatedEventHandler> logger) : DomainEventHandlerBase<ApiPermissionCreatedEventHandler>(logger), IDomainEventHandler<DomainCreatedEvent<ApiPermission>> {
    /// <summary>
    /// 处理 API 权限创建事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainCreatedEvent<ApiPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("创建", "API权限", @event.Description, @event.Domains.Count());

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.API_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除 API 权限列表和分页缓存（新增 {Count} 个）", @event.Domains.Count());
    }
}

/// <summary>
/// API 权限更新事件处理器
/// <para>处理 API 权限更新后的缓存清除</para>
/// </summary>
public class ApiPermissionUpdatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ApiPermissionUpdatedEventHandler> logger) : DomainEventHandlerBase<ApiPermissionUpdatedEventHandler>(logger), IDomainEventHandler<DomainUpdatedEvent<ApiPermission>> {
    /// <summary>
    /// 处理 API 权限更新事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainUpdatedEvent<ApiPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("更新", "API权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除 API 权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.API_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除 API 权限列表和分页缓存");
    }
}

/// <summary>
/// API 权限删除事件处理器
/// <para>处理 API 权限删除后的缓存清除</para>
/// </summary>
public class ApiPermissionDeletedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ApiPermissionDeletedEventHandler> logger) : DomainEventHandlerBase<ApiPermissionDeletedEventHandler>(logger), IDomainEventHandler<DomainDeletedEvent<ApiPermission>> {
    /// <summary>
    /// 处理 API 权限删除事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainDeletedEvent<ApiPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("删除", "API权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除 API 权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.API_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除 API 权限列表和分页缓存");
    }
}

/// <summary>
/// API 权限恢复事件处理器
/// <para>处理 API 权限恢复后的缓存清除</para>
/// </summary>
public class ApiPermissionRestoredEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ApiPermissionRestoredEventHandler> logger) : DomainEventHandlerBase<ApiPermissionRestoredEventHandler>(logger), IDomainEventHandler<DomainRestoredEvent<ApiPermission>> {
    /// <summary>
    /// 处理 API 权限恢复事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainRestoredEvent<ApiPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("恢复", "API权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除 API 权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.API_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除 API 权限列表和分页缓存");
    }
}

#endregion

#region 按钮权限事件处理器

/// <summary>
/// 按钮权限创建事件处理器
/// <para>处理按钮权限创建后的缓存清除</para>
/// </summary>
public class ButtonPermissionCreatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ButtonPermissionCreatedEventHandler> logger) : DomainEventHandlerBase<ButtonPermissionCreatedEventHandler>(logger), IDomainEventHandler<DomainCreatedEvent<ButtonPermission>> {
    /// <summary>
    /// 处理按钮权限创建事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainCreatedEvent<ButtonPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("创建", "按钮权限", @event.Description, @event.Domains.Count());

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.BUTTON_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除按钮权限列表和分页缓存（新增 {Count} 个）", @event.Domains.Count());
    }
}

/// <summary>
/// 按钮权限更新事件处理器
/// <para>处理按钮权限更新后的缓存清除</para>
/// </summary>
public class ButtonPermissionUpdatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ButtonPermissionUpdatedEventHandler> logger) : DomainEventHandlerBase<ButtonPermissionUpdatedEventHandler>(logger), IDomainEventHandler<DomainUpdatedEvent<ButtonPermission>> {
    /// <summary>
    /// 处理按钮权限更新事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainUpdatedEvent<ButtonPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("更新", "按钮权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除按钮权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.BUTTON_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除按钮权限列表和分页缓存");
    }
}

/// <summary>
/// 按钮权限删除事件处理器
/// <para>处理按钮权限删除后的缓存清除</para>
/// </summary>
public class ButtonPermissionDeletedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ButtonPermissionDeletedEventHandler> logger) : DomainEventHandlerBase<ButtonPermissionDeletedEventHandler>(logger), IDomainEventHandler<DomainDeletedEvent<ButtonPermission>> {
    /// <summary>
    /// 处理按钮权限删除事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainDeletedEvent<ButtonPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("删除", "按钮权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除按钮权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.BUTTON_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除按钮权限列表和分页缓存");
    }
}

/// <summary>
/// 按钮权限恢复事件处理器
/// <para>处理按钮权限恢复后的缓存清除</para>
/// </summary>
public class ButtonPermissionRestoredEventHandler(
    ICacheProvider cacheProvider,
    ILogger<ButtonPermissionRestoredEventHandler> logger) : DomainEventHandlerBase<ButtonPermissionRestoredEventHandler>(logger), IDomainEventHandler<DomainRestoredEvent<ButtonPermission>> {
    /// <summary>
    /// 处理按钮权限恢复事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainRestoredEvent<ButtonPermission> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("恢复", "按钮权限", @event.Description, @event.Domains.Count());

        var permissionTasks = @event.Domains.Select(permission => Task.Run(async () => {
            await cacheProvider.RemoveAsync(CacheKeyConstants.Permission.Detail(permission.Id));
            Logger.LogInformation("已清除按钮权限 {PermissionId} 的缓存", permission.Id);
        }));

        await Task.WhenAll(permissionTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.LIST}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.BUTTON_PREFIX}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.Permission.PREFIX}:paged*")
        );
        Logger.LogInformation("已清除按钮权限列表和分页缓存");
    }
}

#endregion
