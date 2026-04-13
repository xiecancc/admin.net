/*
 * 文件名称: CommandHandler.cs
 * 功能描述: 请求处理器基类，所有命令和查询处理器的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Units;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Commands;

/// <summary>
/// 领域命令处理器基类（Template Method 模式）
/// 用于处理所有领域实体的命令操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <remarks>
/// <para>使用 Template Method 模式：</para>
/// <para>- Handle() 实现公共框架（验证、事务、异常处理）</para>
/// <para>- ExecuteInTransactionAsync() 为抽象方法，由子类实现具体业务逻辑</para>
/// <para></para>
/// <para>验证职责边界：</para>
/// <para>1. FluentValidation（ValidationBehavior 管道）：数据格式验证（字段非空、长度限制、正则匹配等）</para>
/// <para>2. ValidateRequest 方法：请求参数基本验证（数据列表非空等轻量级验证，无需数据库查询）</para>
/// <para>3. ExecuteInTransactionAsync 方法：业务规则验证（编码唯一性、循环继承检测等需要数据库查询的验证）</para>
/// </remarks>
public abstract class CommandHandler<TDomain, TRepository, TCommand>(
    IUnitOfWork unitOfWork,
    TRepository repository,
    IMapper mapper,
    ILogger logger) : RequestHandler<TDomain, TRepository, TCommand, bool>(unitOfWork, repository, mapper)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TCommand : IRequest<bool> {

    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 在事务内执行具体的业务逻辑（由子类实现）
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作是否成功</returns>
    protected abstract Task<bool> ExecuteInTransactionAsync(TCommand request, CancellationToken cancellationToken);

    /// <summary>
    /// 验证命令参数（可在子类中扩展）
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    protected virtual void ValidateRequest(TCommand request, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// 处理命令请求（Template Method - 公共框架）
    /// 包含验证、事务管理、异常处理的完整流程
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作是否成功</returns>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);

        try {
            var success = await UnitOfWork.ExecuteInTransactionAsync(
                () => ExecuteInTransactionAsync(request, cancellationToken),
                cancellationToken);

            return success;
        }
        catch (Exception ex) {
            var errorMessage = $"{typeof(TDomain).Name} 操作失败";
            Logger.LogError(ex, "{ErrorMessage}", errorMessage);

            switch (ex) {
                case ArgumentNullException:
                case ArgumentException:
                case InvalidOperationException:
                case KeyNotFoundException:
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                case OperationCanceledException:
                    throw new OperationCanceledException($"操作被取消: {errorMessage}", ex);
                case TimeoutException:
                    throw new InvalidOperationException($"{errorMessage}，操作超时", ex);
                default:
                    throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }
}