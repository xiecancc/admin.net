/*
 * 文件名称: RequestHandler.cs
 * 功能描述: 请求处理器基类，所有命令和查询处理器的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Units;
using MediatR;
using AutoMapper;

namespace Application.Abstractions;

/// <summary>
/// 领域请求处理器基类
/// 所有领域相关请求处理器的基础类，包含工作单元、仓储和映射器属性
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
/// <remarks>
/// <para>依赖注入说明：</para>
/// <para>- 仓储通过构造函数直接注入，遵循显式依赖原则</para>
/// </remarks>
public abstract class RequestHandler<TDomain, TRepository, TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TRequest : IRequest<TResponse> {
    /// <summary>
    /// 工作单元
    /// </summary>
    protected readonly IUnitOfWork UnitOfWork;

    /// <summary>
    /// 仓储接口
    /// </summary>
    protected readonly TRepository Repository;

    /// <summary>
    /// 对象映射器
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元，不能为空</param>
    /// <param name="repository">仓储接口，不能为空</param>
    /// <param name="mapper">对象映射器，不能为空</param>
    /// <exception cref="ArgumentNullException">当参数为 null 时抛出</exception>
    protected RequestHandler(IUnitOfWork unitOfWork, TRepository repository, IMapper mapper) {
        ArgumentNullException.ThrowIfNull(unitOfWork, nameof(unitOfWork));
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        UnitOfWork = unitOfWork;
        Repository = repository;
        Mapper = mapper;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应数据</returns>
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
