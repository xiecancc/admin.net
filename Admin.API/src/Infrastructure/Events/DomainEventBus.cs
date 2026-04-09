using System.Collections.Concurrent;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 内存领域事件总线实现
/// 基于依赖注入的事件总线，支持同步和异步发布事件
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
public class DomainEventBus(IServiceProvider serviceProvider, ILogger<DomainEventBus> logger) : IDomainEventBus {

    // 存储事件类型到处理器委托的映射，避免运行时反射
    private readonly ConcurrentDictionary<Type, List<Func<object, CancellationToken, Task>>> _handlers = new();

    /// <summary>
    /// 发布领域事件
    /// </summary>
#pragma warning disable CA1031 // 事件处理需要捕获所有异常以避免影响其他处理器
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : DomainEvent {
        ArgumentNullException.ThrowIfNull(@event);

        var eventType = typeof(TEvent);
        logger.LogInformation("发布领域事件：{EventType}，事件 ID: {EventId}", eventType.Name, @event.EventId);

        if (!_handlers.TryGetValue(eventType, out var handlers)) {
            logger.LogDebug("事件 {EventType} 没有注册的处理器", eventType.Name);
            return;
        }

        // 创建副本以避免在迭代期间修改集合
        var handlersSnapshot = handlers.ToList();
        var exceptions = new List<Exception>();

        foreach (var handler in handlersSnapshot) {
            try {
                await handler(@event, cancellationToken);
                logger.LogDebug("事件处理器处理完成");
            }
            catch (Exception ex) {
                logger.LogError(ex, "处理事件 {EventType} 时发生错误", eventType.Name);
                exceptions.Add(ex);
            }
        }

        // 如果有异常，抛出聚合异常
        if (exceptions.Count > 0) {
            logger.LogWarning("领域事件 {EventType} 处理完成，但有 {ErrorCount} 个处理器失败", eventType.Name, exceptions.Count);
            throw new AggregateException($"处理事件 {eventType.Name} 时发生 {exceptions.Count} 个错误", exceptions);
        }

        logger.LogInformation("领域事件 {EventType} 发布完成，共 {HandlerCount} 个处理器", eventType.Name, handlersSnapshot.Count);
    }
#pragma warning restore CA1031

    /// <summary>
    /// 注册领域事件处理器
    /// </summary>
    public void Register<TEvent, THandler>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent> {
        var eventType = typeof(TEvent);
        var handlerType = typeof(THandler);

        // 编译时创建委托，避免运行时反射
        Func<object, CancellationToken, Task> handlerDelegate = async (eventObj, cancellationToken) => {
            using var scope = serviceProvider.CreateScope();
            var handler = (THandler)scope.ServiceProvider.GetRequiredService(handlerType);
            await handler.HandleAsync((TEvent)eventObj, cancellationToken);
        };

        _ = _handlers.AddOrUpdate(
            eventType,
            _ => [handlerDelegate],
            (_, existing) => {
                existing.Add(handlerDelegate);
                return existing;
            });

        logger.LogInformation("注册事件处理器：{HandlerType} 处理 {EventType}", handlerType.Name, eventType.Name);
    }
}
