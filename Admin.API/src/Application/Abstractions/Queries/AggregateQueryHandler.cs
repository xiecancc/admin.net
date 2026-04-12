/*
 * 文件名称: AggregateQueryHandler.cs
 * 功能描述: 聚合根查询处理器基类，包含通用查询条件构建逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using System.Linq.Expressions;
using AutoMapper;
using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using System.Text;

namespace Application.Abstractions.Queries;

/// <summary>
/// 聚合根查询处理器基类
/// <para>用于处理聚合根实体的查询操作，包含通用查询条件构建逻辑</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TResponseDto">响应类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class AggregateQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TResponseDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : QueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TResponseDto>(unitOfWork, mapper, cacheProvider)
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TQuery : AggregateQuery<TQueryDto, TResponseDto>
    where TQueryDto : AggregateQueryDto {
    /// <summary>
    /// 构建聚合根通用查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<TAggregate, bool>>> BuildPredicates(TQuery query) {
        var predicates = new List<Expression<Func<TAggregate, bool>>>();
        return predicates;
    }

    /// <summary>
    /// 构建聚合根通用排序条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>排序条件字典，bool值为true代表倒序，false代表正序</returns>
    protected override IDictionary<Expression<Func<TAggregate, object>>, bool> BuildOrders(TQuery query) {
        var orders = new Dictionary<Expression<Func<TAggregate, object>>, bool>
        {
            { t => t.CreatedAt, true }
        };
        return orders;
    }

    /// <summary>
    /// 构建聚合根通用缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(TQuery query) {
        var builder = base.BuildCacheParams(query);
        return builder;
    }
}
