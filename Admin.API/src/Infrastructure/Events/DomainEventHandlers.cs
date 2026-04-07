using Domain.Shared.Entities;
using Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 领域模型审计事件处理器
/// 统一处理实体的创建、更新、删除、恢复等审计事件
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
public class DomainEventHandlers<TDomain>(ILogger<DomainEventHandlers<TDomain>> logger) :
    IDomainEventHandler<DomainCreatedEvent<TDomain>>,
    IDomainEventHandler<DomainUpdatedEvent<TDomain>>,
    IDomainEventHandler<DomainDeletedEvent<TDomain>>,
    IDomainEventHandler<DomainRestoredEvent<TDomain>>
    where TDomain : DomainBase {
    /// <summary>
    /// 处理创建事件
    /// </summary>
    public Task HandleAsync(DomainCreatedEvent<TDomain> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);
        LogEvent("创建", @event.Description, @event.Domains.Count());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理更新事件
    /// </summary>
    public Task HandleAsync(DomainUpdatedEvent<TDomain> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);
        LogEvent("更新", @event.Description, @event.Domains.Count());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理软删除事件
    /// </summary>
    public Task HandleAsync(DomainDeletedEvent<TDomain> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);
        LogEvent("软删除", @event.Description, @event.Domains.Count());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理恢复事件
    /// </summary>
    public Task HandleAsync(DomainRestoredEvent<TDomain> @event, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(@event);
        LogEvent("恢复", @event.Description, @event.Domains.Count());
        return Task.CompletedTask;
    }

    /// <summary>
    /// 统一记录审计日志
    /// </summary>
    private void LogEvent(string operation, string description, int entityCount) {
        logger.LogInformation(
            "[审计] {Operation}事件：{Description}，影响实体数：{Count}",
            operation,
            description,
            entityCount);
    }
}
