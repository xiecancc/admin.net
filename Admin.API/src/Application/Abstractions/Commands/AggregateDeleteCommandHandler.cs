/*
 * 文件名称: AggregateDeleteCommandHandler.cs
 * 功能描述: 通用删除命令处理器（Template Method 模式）
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Units;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Commands;

/// <summary>
/// 通用删除命令处理器（Template Method 模式）
/// 用于处理所有聚合根实体的删除操作
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">删除命令类型</typeparam>
/// <typeparam name="TDeleteDto">删除DTO类型</typeparam>
public abstract class AggregateDeleteCommandHandler<TDomain, TRepository, TCommand, TDeleteDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : CommandHandler<TDomain, TRepository, TCommand>(unitOfWork, mapper, logger)
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TCommand : AggregateDeleteCommand<TDeleteDto>, IRequest<bool>
    where TDeleteDto : AggregateActionDto {

    /// <summary>
    /// 验证删除数据（可在子类中扩展）
    /// </summary>
    /// <param name="data">删除数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为空时抛出</exception>
    /// <exception cref="ArgumentException">当数据列表为空时抛出</exception>
    protected virtual void ValidateDeleteData(List<TDeleteDto> data) {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Count == 0) throw new ArgumentException("删除数据不能为空", nameof(data));
    }

    /// <summary>
    /// 验证请求参数
    /// </summary>
    protected override void ValidateRequest(TCommand request, CancellationToken cancellationToken) {
        base.ValidateRequest(request, cancellationToken);
        ValidateDeleteData(request.Data);
    }

    /// <inheritdoc/>
    protected override Task<bool> ExecuteInTransactionAsync(TCommand request, CancellationToken cancellationToken) {
        var ids = request.Data.Select(dto => dto.Id).ToList();
        return Repository.DeleteAsync(ids, cancellationToken);
    }
}
