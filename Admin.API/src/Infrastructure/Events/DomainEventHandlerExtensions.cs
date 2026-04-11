/*
 * 文件名称: DomainEventHandlerExtensions.cs
 * 功能描述: 领域事件处理器自动扫描注册扩展方法
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.Events;

/// <summary>
/// 领域事件处理器扩展方法
/// <para>提供自动扫描和注册事件处理器的功能</para>
/// </summary>
public static class DomainEventHandlerExtensions {
    /// <summary>
    /// 扫描并注册所有领域事件处理器
    /// <para>自动扫描指定程序集中所有实现 IDomainEventHandler 接口的类型</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">要扫描的程序集</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>扫描规则：</para>
    /// <list type="bullet">
    ///   <item>查找所有实现 IDomainEventHandler&lt;TEvent&gt; 接口的类</item>
    ///   <item>自动注册为 Scoped 生命周期</item>
    ///   <item>支持一个处理器处理多个事件类型</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection ScanAndRegisterDomainEventHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies) {
        ArgumentNullException.ThrowIfNull(assemblies);

        var handlerInterfaceType = typeof(IDomainEventHandler<>);
        var registeredHandlers = new HashSet<Type>();

        foreach (var assembly in assemblies) {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false })
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType));

            foreach (var handlerType in handlerTypes) {
                if (registeredHandlers.Contains(handlerType)) {
                    continue;
                }

                services.AddScoped(handlerType);
                registeredHandlers.Add(handlerType);
            }
        }

        return services;
    }

    /// <summary>
    /// 将已注册的事件处理器绑定到事件总线
    /// <para>在应用启动时调用，将所有处理器注册到 DomainEventBus</para>
    /// </summary>
    /// <param name="eventBus">事件总线实例</param>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="assemblies">要扫描的程序集</param>
    /// <returns>注册的处理器数量</returns>
    /// <remarks>
    /// <para>绑定规则：</para>
    /// <list type="bullet">
    ///   <item>查找所有实现 IDomainEventHandler&lt;TEvent&gt; 接口的类</item>
    ///   <item>提取每个处理器支持的事件类型</item>
    ///   <item>调用 eventBus.Register 方法完成绑定</item>
    /// </list>
    /// </remarks>
    public static int BindDomainEventHandlers(
        this IDomainEventBus eventBus,
        IServiceProvider serviceProvider,
        params Assembly[] assemblies) {
        ArgumentNullException.ThrowIfNull(eventBus);
        ArgumentNullException.ThrowIfNull(assemblies);

        var handlerInterfaceType = typeof(IDomainEventHandler<>);
        var registerMethod = typeof(IDomainEventBus).GetMethod(nameof(IDomainEventBus.Register))!;
        var registeredCount = 0;

        foreach (var assembly in assemblies) {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false });

            foreach (var handlerType in handlerTypes) {
                var handlerInterfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType);

                foreach (var handlerInterface in handlerInterfaces) {
                    var eventType = handlerInterface.GetGenericArguments()[0];

                    if (eventType.ContainsGenericParameters || handlerType.ContainsGenericParameters) {
                        continue;
                    }

                    try {
                        var genericRegisterMethod = registerMethod.MakeGenericMethod(eventType, handlerType);
                        genericRegisterMethod.Invoke(eventBus, null);
                        registeredCount++;
                    }
                    catch (ArgumentException) {
                        continue;
                    }
                }
            }
        }

        return registeredCount;
    }

    /// <summary>
    /// 验证所有领域事件都有对应的处理器
    /// <para>在开发环境或测试中使用，确保事件处理的完整性</para>
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="assemblies">要扫描的程序集</param>
    /// <returns>验证结果，包含未处理的事件类型列表</returns>
    /// <remarks>
    /// <para>验证规则：</para>
    /// <list type="bullet">
    ///   <item>扫描所有继承自 DomainEvent 的类型</item>
    ///   <item>检查是否有对应的 IDomainEventHandler&lt;TEvent&gt; 实现</item>
    ///   <item>返回没有处理器的事件类型</item>
    /// </list>
    /// </remarks>
    public static DomainEventHandlerValidationResult ValidateDomainEventHandlers(
        this IServiceProvider serviceProvider,
        params Assembly[] assemblies) {
        ArgumentNullException.ThrowIfNull(assemblies);

        var domainEventType = typeof(DomainEvent);
        var handlerInterfaceType = typeof(IDomainEventHandler<>);

        var allEventTypes = new HashSet<Type>();
        var handledEventTypes = new HashSet<Type>();

        foreach (var assembly in assemblies) {
            var eventTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && domainEventType.IsAssignableFrom(t));

            foreach (var eventType in eventTypes) {
                allEventTypes.Add(eventType);
            }

            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false });

            foreach (var handlerType in handlerTypes) {
                var handlerInterfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType);

                foreach (var handlerInterface in handlerInterfaces) {
                    var eventType = handlerInterface.GetGenericArguments()[0];
                    handledEventTypes.Add(eventType);
                }
            }
        }

        var unhandledEventTypes = allEventTypes.Except(handledEventTypes).ToList();

        return new DomainEventHandlerValidationResult(
            allEventTypes.Count,
            handledEventTypes.Count,
            unhandledEventTypes);
    }
}

/// <summary>
/// 领域事件处理器验证结果
/// </summary>
/// <param name="TotalEventCount">事件类型总数</param>
/// <param name="HandledEventCount">已处理事件数量</param>
/// <param name="UnhandledEventTypes">未处理的事件类型列表</param>
public record DomainEventHandlerValidationResult(
    int TotalEventCount,
    int HandledEventCount,
    IReadOnlyList<Type> UnhandledEventTypes) {
    /// <summary>
    /// 是否所有事件都有处理器
    /// </summary>
    public bool IsValid => UnhandledEventTypes.Count == 0;

    /// <summary>
    /// 获取验证摘要
    /// </summary>
    public string GetSummary() {
        if (IsValid) {
            return $"验证通过：所有 {TotalEventCount} 个事件类型都有对应的处理器";
        }

        var unhandledNames = string.Join(", ", UnhandledEventTypes.Select(t => t.Name));
        return $"验证失败：{TotalEventCount} 个事件类型中有 {UnhandledEventTypes.Count} 个没有处理器 ({unhandledNames})";
    }
}
