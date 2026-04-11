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

namespace Application.Abstractions.Commands;

/// <summary>
/// 领域命令处理器基类
/// 用于处理所有领域实体的命令操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
public abstract class CommandHandler<TCommand, TDomain, TRepository, TResponse>(IUnitOfWork unitOfWork, IMapper mapper) : RequestHandler<TCommand, TDomain, TRepository, TResponse>(unitOfWork, mapper)
    where TCommand : IRequest<TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain> {
}
