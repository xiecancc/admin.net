namespace Domain.Shared.Events;

/// <summary>
/// 领域事件总线接口
/// 用于发布和订阅领域事件
/// </summary>
public interface IDomainEventBus {
    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <param name="event">领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发布任务</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : DomainEvent;

    /// <summary>
    /// 注册领域事件处理器
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    void Register<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>;
}
