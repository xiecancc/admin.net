/*
 * 文件名称: AggregateRestoreCommandHandler.cs
 * 功能描述: 通用恢复命令处理器（Template Method 模式）
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
/// 通用恢复命令处理器（Template Method 模式）
/// 用于处理所有聚合根实体的恢复操作（从软删除状态恢复）
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">恢复命令类型</typeparam>
/// <typeparam name="TRestoreDto">恢复DTO类型</typeparam>
public abstract class AggregateRestoreCommandHandler<TDomain, TRepository, TCommand, TRestoreDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : CommandHandler<TDomain, TRepository, TCommand>(unitOfWork, mapper, logger)
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TCommand : AggregateRestoreCommand<TRestoreDto>, IRequest<bool>
    where TRestoreDto : AggregateActionDto {

    /// <summary>
    /// 验证恢复数据（可在子类中扩展）
    /// </summary>
    /// <param name="data">恢复数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为空时抛出</exception>
    /// <exception cref="ArgumentException">当数据列表为空时抛出</exception>
    protected virtual void ValidateRestoreData(List<TRestoreDto> data) {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Count == 0) throw new ArgumentException("恢复数据不能为空", nameof(data));
    }

    /// <summary>
    /// 验证请求参数
    /// </summary>
    protected override void ValidateRequest(TCommand request, CancellationToken cancellationToken) {
        base.ValidateRequest(request, cancellationToken);
        ValidateRestoreData(request.Data);
    }

    /// <inheritdoc/>
    protected override Task<bool> ExecuteInTransactionAsync(TCommand request, CancellationToken cancellationToken) {
        var ids = request.Data.Select(dto => dto.Id).ToList();
        return Repository.RestoreAsync(ids, cancellationToken);
    }
}
