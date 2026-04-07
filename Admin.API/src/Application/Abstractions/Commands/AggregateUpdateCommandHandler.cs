/*
 * 文件名称: AggregateUpdateCommandHandler.cs
 * 功能描述: 通用更新命令处理器，用于处理所有聚合根实体的更新操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Units;
using AutoMapper;

namespace Application.Abstractions.Commands;

/// <summary>
/// 通用更新命令处理器
/// 用于处理所有聚合根实体的更新操作
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TUpdateDto">更新DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
public abstract class AggregateUpdateCommandHandler<TCommand, TDomain, TRepository, TUpdateDto>(IUnitOfWork unitOfWork, IMapper mapper) : DomainCommandHandler<TCommand, TDomain, TRepository, bool>(unitOfWork, mapper)
    where TCommand : AggregateUpdateCommand<TDomain, TRepository, TUpdateDto>
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TUpdateDto : AggregateUpdateDto {

    /// <summary>
    /// 处理更新命令
    /// </summary>
    /// <param name="request">更新命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        return await UnitOfWork.ExecuteInTransactionAsync(async () => {
            var entities = new List<TDomain>();
            foreach (var dto in request.Data) {
                var entity = await Repository.GetAsync(dto.Id, cancellationToken);
                if (entity != null) {
                    UpdateEntity(entity, dto);
                    entities.Add(entity);
                }
            }
            return await Repository.UpdateAsync(entities, cancellationToken);
        }, cancellationToken);
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="dto">更新DTO</param>
    protected virtual void UpdateEntity(TDomain entity, TUpdateDto dto) {
        _ = Mapper.Map(dto, entity);
    }
}
