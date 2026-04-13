/*
 * 文件名称: AggregateUpdateCommandHandler.cs
 * 功能描述: 通用更新命令处理器（Template Method 模式）
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
/// 通用更新命令处理器（Template Method 模式）
/// 用于处理所有聚合根实体的更新操作
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">更新命令类型</typeparam>
/// <typeparam name="TUpdateDto">更新DTO类型</typeparam>
public abstract class AggregateUpdateCommandHandler<TDomain, TRepository, TCommand, TUpdateDto>(
    IUnitOfWork unitOfWork,
    TRepository repository,
    IMapper mapper,
    ILogger logger)
    : CommandHandler<TDomain, TRepository, TCommand>(unitOfWork, repository, mapper, logger)
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TCommand : AggregateUpdateCommand<TUpdateDto>, IRequest<bool>
    where TUpdateDto : AggregateUpdateDto {

    /// <summary>
    /// 验证更新数据（可在子类中扩展）
    /// </summary>
    /// <param name="data">更新数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为空时抛出</exception>
    /// <exception cref="ArgumentException">当数据列表为空时抛出</exception>
    protected virtual void ValidateUpdateData(List<TUpdateDto> data) {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Count == 0) throw new ArgumentException("更新数据不能为空", nameof(data));
    }

    /// <summary>
    /// 验证请求参数
    /// </summary>
    protected override void ValidateRequest(TCommand request, CancellationToken cancellationToken) {
        base.ValidateRequest(request, cancellationToken);
        ValidateUpdateData(request.Data);
    }

    /// <inheritdoc/>
    protected override async Task<bool> ExecuteInTransactionAsync(TCommand request, CancellationToken cancellationToken) {
        var entities = new List<TDomain>();
        var notFoundIds = new List<Guid>();

        foreach (var dto in request.Data) {
            var entity = await Repository.GetAsync(dto.Id, cancellationToken);

            if (entity is null) {
                notFoundIds.Add(dto.Id);
                continue;
            }

            Mapper.Map(dto, entity);
            entities.Add(entity);
        }

        if (notFoundIds.Count > 0) {
            throw new KeyNotFoundException(
                $"{typeof(TDomain).Name} 更新失败，以下实体不存在: {string.Join(", ", notFoundIds)}");
        }

        return await Repository.UpdateAsync(entities, cancellationToken);
    }
}
