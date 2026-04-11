/*
 * 文件名称: RequestHandler.cs
 * 功能描述: 请求处理器基类，所有命令和查询处理器的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using MediatR;
using AutoMapper;

namespace Application.Abstractions;

/// <summary>
/// 领域请求处理器基类
/// 所有领域相关请求处理器的基础类，包含工作单元、仓储和映射器属性
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public abstract class DomainRequestHandler<TRequest, TDomain, TRepository, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain> {
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
    /// <param name="mapper">对象映射器，不能为空</param>
    /// <exception cref="ArgumentNullException">当工作单元或映射器为 null 时抛出</exception>
    protected DomainRequestHandler(IUnitOfWork unitOfWork, IMapper mapper) {
        ArgumentNullException.ThrowIfNull(unitOfWork, nameof(unitOfWork));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        UnitOfWork = unitOfWork;
        Repository = unitOfWork.GetRepository<TRepository, TDomain>();
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

/// <summary>
/// 领域查询处理器基类
/// 用于处理所有领域实体的查询操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
/// <remarks>
/// <para>职责：提供查询处理和缓存功能</para>
/// <para>依赖：IUnitOfWork, IMapper, ICacheProvider</para>
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public abstract class DomainQueryHandler<TQuery, TDomain, TRepository, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainRequestHandler<TQuery, TDomain, TRepository, TResponse>(unitOfWork, mapper)
    where TQuery : IRequest<TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain> {
    /// <summary>
    /// 缓存提供者
    /// </summary>
    protected readonly ICacheProvider CacheProvider = cacheProvider;

    /// <summary>
    /// 缓存键前缀
    /// <para>子类需要重写此属性以提供正确的缓存键前缀</para>
    /// </summary>
    protected abstract string CacheKeyPrefix {
        get;
    }

    /// <summary>
    /// 获取或设置缓存
    /// <para>过期时间由缓存配置决定</para>
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="factory">数据工厂方法</param>
    /// <returns>缓存数据</returns>
    protected Task<T?> GetOrSetCacheAsync<T>(string cacheKey, Func<Task<T?>> factory) {
        return CacheProvider.GetOrSetAsync(cacheKey, factory);
    }
}

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
public abstract class DomainCommandHandler<TCommand, TDomain, TRepository, TResponse>(IUnitOfWork unitOfWork, IMapper mapper) : DomainRequestHandler<TCommand, TDomain, TRepository, TResponse>(unitOfWork, mapper)
    where TCommand : IRequest<TResponse>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain> {
}
