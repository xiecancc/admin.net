/*
 * 文件名称: UserDomainEventHandlers.cs
 * 功能描述: 用户领域事件处理器，处理用户相关事件并清除缓存
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Domain.Entities;
using Domain.Events;
using Domain.Shared.Constants;
using Domain.Shared.Events;
using Infrastructure.Shared.Caches;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 用户状态变更事件处理器
/// <para>处理用户状态变更后的缓存清除</para>
/// </summary>
public class UserStatusChangedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserStatusChangedEventHandler> logger) : DomainEventHandlerBase<UserStatusChangedEventHandler>(logger), IDomainEventHandler<UserStatusChangedEvent> {
    /// <summary>
    /// 处理用户状态变更事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(UserStatusChangedEvent @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("状态变更", @event.Description, @event.Domains.Count());

        var tasks = @event.Domains.Select(user => Task.Run(async () => {
            await Task.WhenAll(
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Status(user.Id))
            );
            Logger.LogInformation("已清除用户 {UserId} 的缓存（状态变更）", user.Id);
        }));

        await Task.WhenAll(tasks);
    }
}

/// <summary>
/// 用户角色分配事件处理器
/// <para>处理用户角色分配后的缓存清除</para>
/// </summary>
public class UserRolesAssignedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserRolesAssignedEventHandler> logger) : DomainEventHandlerBase<UserRolesAssignedEventHandler>(logger), IDomainEventHandler<UserRolesAssignedEvent> {
    /// <summary>
    /// 处理用户角色分配事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(UserRolesAssignedEvent @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("角色分配", @event.Description, @event.RoleIds.Count());

        await Task.WhenAll(
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(@event.UserId)),
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Permissions(@event.UserId))
        );
        Logger.LogInformation("已清除用户 {UserId} 的缓存（角色分配）", @event.UserId);
    }
}

/// <summary>
/// 用户角色撤销事件处理器
/// <para>处理用户角色撤销后的缓存清除</para>
/// </summary>
public class UserRolesRevokedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserRolesRevokedEventHandler> logger) : DomainEventHandlerBase<UserRolesRevokedEventHandler>(logger), IDomainEventHandler<UserRolesRevokedEvent> {
    /// <summary>
    /// 处理用户角色撤销事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(UserRolesRevokedEvent @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("角色撤销", @event.Description, @event.RoleIds.Count());

        await Task.WhenAll(
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(@event.UserId)),
            cacheProvider.RemoveAsync(CacheKeyConstants.User.Permissions(@event.UserId))
        );
        Logger.LogInformation("已清除用户 {UserId} 的缓存（角色撤销）", @event.UserId);
    }
}

/// <summary>
/// 用户创建事件处理器
/// <para>处理用户创建后的缓存清除</para>
/// </summary>
public class UserCreatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserCreatedEventHandler> logger) : DomainEventHandlerBase<UserCreatedEventHandler>(logger), IDomainEventHandler<DomainCreatedEvent<User>> {
    /// <summary>
    /// 处理用户创建事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainCreatedEvent<User> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("创建", @event.Description, @event.Domains.Count());

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.List}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.Prefix}:paged*")
        );
        Logger.LogInformation("已清除用户列表和分页缓存（新增 {Count} 个用户）", @event.Domains.Count());
    }
}

/// <summary>
/// 用户更新事件处理器
/// <para>处理用户更新后的缓存清除</para>
/// </summary>
public class UserUpdatedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserUpdatedEventHandler> logger) : DomainEventHandlerBase<UserUpdatedEventHandler>(logger), IDomainEventHandler<DomainUpdatedEvent<User>> {
    /// <summary>
    /// 处理用户更新事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainUpdatedEvent<User> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("更新", @event.Description, @event.Domains.Count());

        var userTasks = @event.Domains.Select(user => Task.Run(async () => {
            await Task.WhenAll(
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Status(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Detail(user.Id))
            );
            Logger.LogInformation("已清除用户 {UserId} 的缓存", user.Id);
        }));

        await Task.WhenAll(userTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.List}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.Prefix}:paged*")
        );
        Logger.LogInformation("已清除用户列表和分页缓存");
    }
}

/// <summary>
/// 用户删除事件处理器
/// <para>处理用户删除后的缓存清除</para>
/// </summary>
public class UserDeletedEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserDeletedEventHandler> logger) : DomainEventHandlerBase<UserDeletedEventHandler>(logger), IDomainEventHandler<DomainDeletedEvent<User>> {
    /// <summary>
    /// 处理用户删除事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainDeletedEvent<User> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("删除", @event.Description, @event.Domains.Count());

        var userTasks = @event.Domains.Select(user => Task.Run(async () => {
            await Task.WhenAll(
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Status(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Detail(user.Id))
            );
            Logger.LogInformation("已清除用户 {UserId} 的缓存", user.Id);
        }));

        await Task.WhenAll(userTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.List}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.Prefix}:paged*")
        );
        Logger.LogInformation("已清除用户列表和分页缓存");
    }
}

/// <summary>
/// 用户恢复事件处理器
/// <para>处理用户恢复后的缓存清除</para>
/// </summary>
public class UserRestoredEventHandler(
    ICacheProvider cacheProvider,
    ILogger<UserRestoredEventHandler> logger) : DomainEventHandlerBase<UserRestoredEventHandler>(logger), IDomainEventHandler<DomainRestoredEvent<User>> {
    /// <summary>
    /// 处理用户恢复事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task HandleAsync(DomainRestoredEvent<User> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);

        LogEvent("恢复", @event.Description, @event.Domains.Count());

        var userTasks = @event.Domains.Select(user => Task.Run(async () => {
            await Task.WhenAll(
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Info(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Status(user.Id)),
                cacheProvider.RemoveAsync(CacheKeyConstants.User.Detail(user.Id))
            );
            Logger.LogInformation("已清除用户 {UserId} 的缓存", user.Id);
        }));

        await Task.WhenAll(userTasks);

        _ = await Task.WhenAll(
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.List}*"),
            cacheProvider.RemoveByPatternAsync($"{CacheKeyConstants.User.Prefix}:paged*")
        );
        Logger.LogInformation("已清除用户列表和分页缓存");
    }
}
