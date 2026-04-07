/*
 * 文件名称: DomainEventBusExtensions.cs
 * 功能描述: 领域事件总线扩展方法，用于注册事件处理器
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using System.Reflection;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 领域事件总线扩展方法
/// <para>用于注册事件处理器到事件总线</para>
/// </summary>
public static class DomainEventBusExtensions {
    /// <summary>
    /// 添加领域事件处理器绑定托管服务
    /// <para>在应用启动时自动绑定所有事件处理器</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">要扫描的程序集</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>使用方式：</para>
    /// <code>
    /// services.AddDomainEventHandlerBinding(Assembly.GetExecutingAssembly());
    /// </code>
    /// </remarks>
    public static IServiceCollection AddDomainEventHandlerBinding(
        this IServiceCollection services,
        params Assembly[] assemblies) {
        _ = services.AddHostedService<DomainEventHandlerBindingService>(sp => {
            var eventBus = sp.GetRequiredService<IDomainEventBus>();
            var logger = sp.GetRequiredService<ILogger<DomainEventHandlerBindingService>>();
            return new DomainEventHandlerBindingService(eventBus, logger, sp, assemblies);
        });

        return services;
    }
}

/// <summary>
/// 领域事件处理器绑定托管服务
/// <para>在应用启动时自动绑定所有事件处理器</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="eventBus">领域事件总线</param>
/// <param name="logger">日志记录器</param>
/// <param name="serviceProvider">服务提供者</param>
/// <param name="assemblies">要扫描的程序集</param>
public class DomainEventHandlerBindingService(
    IDomainEventBus eventBus,
    ILogger<DomainEventHandlerBindingService> logger,
    IServiceProvider serviceProvider,
    Assembly[] assemblies) : IHostedService {
    private readonly IDomainEventBus _eventBus = eventBus;
    private readonly ILogger<DomainEventHandlerBindingService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly Assembly[] _assemblies = assemblies;

    /// <summary>
    /// 启动时绑定事件处理器
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("开始绑定领域事件处理器...");

        var count = _eventBus.BindDomainEventHandlers(_serviceProvider, _assemblies);

        _logger.LogInformation("领域事件处理器绑定完成，共注册 {Count} 个处理器", count);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止时无需处理
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}
