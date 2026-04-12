/*
 * 文件名称: QueryHandler.cs
 * 功能描述: 请求处理器基类，所有命令和查询处理器的基础类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;
using System.Linq.Expressions;
using System.Text;

namespace Application.Abstractions.Queries;

/// <summary>
/// 领域查询处理器基类
/// 用于处理所有领域实体的查询操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TResponseDto">响应类型</typeparam>
/// <remarks>
/// <para>职责：提供查询处理和缓存功能</para>
/// <para>依赖：IUnitOfWork, IMapper, ICacheProvider</para>
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class QueryHandler<TDomain, TRepository, TQuery, TQueryDto, TResponseDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : RequestHandler<TDomain, TRepository, TQuery, TResponseDto>(unitOfWork, mapper)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TQuery : Query<TQueryDto, TResponseDto>
    where TQueryDto : QueryDto {
    /// <summary>
    /// 缓存提供者
    /// </summary>
    protected readonly ICacheProvider CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));

    /// <summary>
    /// 缓存键前缀
    /// <para>默认返回 TDomain 类型名称，子类可重写以提供自定义缓存键前缀</para>
    /// </summary>
    protected virtual string CacheKeyPrefix {
        get => typeof(TDomain).Name.ToLower();
    }

    /// <summary>
    /// 获取或设置缓存
    /// <para>过期时间由缓存配置决定</para>
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="factory">数据工厂方法</param>
    /// <returns>缓存数据</returns>
    protected Task<T?> GetOrSetCacheAsync<T>(string cacheKey, Func<Task<T?>> factory) => CacheProvider.GetOrSetAsync(cacheKey, factory);

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected virtual List<Expression<Func<TDomain, bool>>> BuildPredicates(TQuery query) {
        return [];
    }

    /// <summary>
    /// 构建排序条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>排序条件字典，bool值为true代表倒序，false代表正序</returns>
    protected virtual IDictionary<Expression<Func<TDomain, object>>, bool> BuildOrders(TQuery query) {
        return new Dictionary<Expression<Func<TDomain, object>>, bool>();
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected virtual StringBuilder BuildCacheParams(TQuery query) => new();


}
