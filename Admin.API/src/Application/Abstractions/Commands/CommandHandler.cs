/*
 * 文件名称: CommandHandler.cs
 * 功能描述: 请求处理器基类，所有命令和查询处理器的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Units;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Commands;

/// <summary>
/// 领域命令处理器基类
/// 用于处理所有领域实体的命令操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或日志记录器为 null 时抛出</exception>
public abstract class CommandHandler<TDomain, TRepository, TCommand, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger) : RequestHandler<TDomain, TRepository, TCommand, TResponse>(unitOfWork, mapper)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TCommand : IRequest<TResponse> {

    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 获取实体类型名称，用于日志和异常信息
    /// </summary>
    protected virtual string EntityTypeName => typeof(TDomain).Name;

    /// <summary>
    /// 验证命令参数
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <exception cref="ArgumentNullException">当命令请求为 null 时抛出</exception>
    /// <exception cref="OperationCanceledException">当取消令牌已取消时抛出</exception>
    protected virtual void ValidateRequest(TCommand request, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// 处理异常，将原始异常转换为业务异常
    /// </summary>
    /// <param name="ex">原始异常</param>
    /// <param name="operation">操作名称</param>
    /// <param name="context">异常上下文信息</param>
    /// <returns>转换后的异常</returns>
    protected virtual Exception HandleException(Exception ex, string operation, string? context = null) {
        var errorMessage = string.IsNullOrEmpty(context)
            ? $"{EntityTypeName} {operation}操作失败"
            : $"{EntityTypeName} {operation}操作失败: {context}";

        return ex switch {
            ArgumentNullException or ArgumentException or InvalidOperationException
                or KeyNotFoundException => ex,
            OperationCanceledException => new OperationCanceledException($"操作被取消: {errorMessage}", ex),
            TimeoutException => new InvalidOperationException($"{errorMessage}，操作超时", ex),
            _ => new InvalidOperationException(errorMessage, ex)
        };
    }

    /// <summary>
    /// 记录异常日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="operation">操作名称</param>
    /// <param name="context">异常上下文信息</param>
    protected virtual void LogException(Exception ex, string operation, string? context = null) {
        var errorMessage = string.IsNullOrEmpty(context)
            ? $"{EntityTypeName} {operation}操作异常"
            : $"{EntityTypeName} {operation}操作异常: {context}";

        Logger.LogError(ex, "{ErrorMessage}", errorMessage);
    }
}
