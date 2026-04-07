namespace Domain.Shared.Events;

/// <summary>
/// 领域事件处理器接口
/// </summary>
/// <typeparam name="TEvent">领域事件类型</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent {
    /// <summary>
    /// 处理领域事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>处理任务</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
