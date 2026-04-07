/*
 * 文件名称: DomainEventHandlerBase.cs
 * 功能描述: 领域事件处理器基类，提供公共的日志记录功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>
/// 领域事件处理器基类
/// <para>提供公共的日志记录功能</para>
/// </summary>
/// <typeparam name="THandler">处理器类型</typeparam>
/// <remarks>
/// <para>职责：统一事件处理器的日志格式</para>
/// </remarks>
public abstract class DomainEventHandlerBase<THandler>(ILogger<THandler> logger) {
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger<THandler> Logger = logger;

    /// <summary>
    /// 记录事件日志
    /// </summary>
    /// <param name="operation">操作类型</param>
    /// <param name="description">事件描述</param>
    /// <param name="entityCount">影响实体数</param>
    protected void LogEvent(string operation, string description, int entityCount) {
        Logger.LogInformation(
            "[审计] {Operation}事件：{Description}，影响实体数：{Count}",
            operation, description, entityCount);
    }

    /// <summary>
    /// 记录事件日志（带实体类型）
    /// </summary>
    /// <param name="operation">操作类型</param>
    /// <param name="entityType">实体类型名称</param>
    /// <param name="description">事件描述</param>
    /// <param name="entityCount">影响实体数</param>
    protected void LogEvent(string operation, string entityType, string description, int entityCount) {
        Logger.LogInformation(
            "[审计] {EntityType}{Operation}事件：{Description}，影响实体数：{Count}",
            entityType, operation, description, entityCount);
    }
}
