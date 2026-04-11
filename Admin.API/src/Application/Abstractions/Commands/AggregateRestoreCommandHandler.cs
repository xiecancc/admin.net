/*
 * 文件名称: AggregateRestoreCommandHandler.cs
 * 功能描述: 通用恢复命令处理器，用于处理所有聚合根实体的恢复操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Shared.Entities;
using Infrastructure.Shared.Units;
using AutoMapper;
using Domain.Shared.Repositories;
using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;

namespace Application.Abstractions.Commands;

/// <summary>
/// 通用恢复命令处理器
/// 用于处理所有聚合根实体的恢复操作
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TActionDto">操作DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
public abstract class AggregateRestoreCommandHandler<TCommand, TDomain, TRepository, TActionDto>(IUnitOfWork unitOfWork, IMapper mapper) : CommandHandler<TCommand, TDomain, TRepository, bool>(unitOfWork, mapper)
    where TCommand : AggregateRestoreCommand<TActionDto>
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TActionDto : AggregateActionDto {

    /// <summary>
    /// 处理恢复命令
    /// </summary>
    /// <param name="request">恢复命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        return await UnitOfWork.ExecuteInTransactionAsync(async () => {
            var ids = request.Data.Select(dto => dto.Id).ToList();
            return await Repository.RestoreAsync(ids, cancellationToken);
        }, cancellationToken);
    }
}
