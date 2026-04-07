/*
 * 文件名称: Request.cs
 * 功能描述: 请求基类，所有命令和查询的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using MediatR;

namespace Application.Contracts.Abstractions;

/// <summary>
/// 领域请求基类
/// 所有领域相关请求的基础类，包含仓储属性
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainRequest<TDomain, TRepository, TResponse> : IRequest<TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>;

/// <summary>
/// 查询基类
/// 所有查询的基础类，用于领域实体（包括关系表）的查询操作
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainQuery<TDomain, TRepository, TResponse> : DomainRequest<TDomain, TRepository, TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>;

/// <summary>
/// 领域命令基类
/// 所有命令的基础类，用于领域实体（包括关系表）的操作
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainCommand<TDomain, TRepository, TResponse> : DomainRequest<TDomain, TRepository, TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>;
