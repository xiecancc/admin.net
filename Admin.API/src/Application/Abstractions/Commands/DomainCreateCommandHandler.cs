/*
 * 文件名称: DomainCreateCommandHandler.cs
 * 功能描述: 通用创建命令处理器，用于处理所有领域实体的创建操作
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
/// 通用创建命令处理器
/// 用于处理所有领域实体的创建操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCreateDto">请求DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
public abstract class DomainCreateCommandHandler<TCommand, TDomain, TRepository, TCreateDto>(IUnitOfWork unitOfWork, IMapper mapper) : DomainCommandHandler<TCommand, TDomain, TRepository, bool>(unitOfWork, mapper)
    where TCommand : DomainCreateCommands<TDomain, TRepository, TCreateDto>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TCreateDto : DomainCreateDto {

    /// <summary>
    /// 处理创建命令
    /// </summary>
    /// <param name="request">创建命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        return await UnitOfWork.ExecuteInTransactionAsync(async () => {
            var entities = Mapper.Map<List<TDomain>>(request.Data);
            return await Repository.InsertAsync(entities, cancellationToken);
        }, cancellationToken);
    }
}
