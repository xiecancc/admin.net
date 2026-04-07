/*
 * 文件名称: DomainDeleteCommandHandler.cs
 * 功能描述: 通用删除命令处理器，用于处理所有领域实体的删除操作
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
/// 聚合根删除命令处理器
/// 用于处理所有聚合根实体的删除操作
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TActionDto">操作DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
public abstract class AggregateDeleteCommandHandler<TCommand, TAggregate, TRepository, TActionDto>(IUnitOfWork unitOfWork, IMapper mapper) : DomainCommandHandler<TCommand, TAggregate, TRepository, bool>(unitOfWork, mapper)
    where TCommand : AggregateDeleteCommand<TAggregate, TRepository, TActionDto>
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TActionDto : AggregateActionDto {

    /// <summary>
    /// 处理删除命令
    /// </summary>
    /// <param name="request">删除命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        return await UnitOfWork.ExecuteInTransactionAsync(async () => {
            var ids = request.Data.Select(dto => dto.Id).ToList();
            return await Repository.DeleteAsync(ids, cancellationToken);
        }, cancellationToken);
    }
}
