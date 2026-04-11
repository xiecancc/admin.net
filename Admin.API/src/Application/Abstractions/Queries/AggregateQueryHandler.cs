/*
 * 文件名称: AggregateQueryHandler.cs
 * 功能描述: 聚合根查询处理器基类，包含通用查询条件构建逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using SqlSugar;
using System.Linq.Expressions;
using AutoMapper;

namespace Application.Abstractions.Queries;

/// <summary>
/// 聚合根查询处理器基类
/// <para>用于处理聚合根实体的查询操作，包含通用查询条件构建逻辑</para>
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class AggregateQueryHandler<TQuery, TAggregate, TRepository, TResponse>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainQueryHandler<TQuery, TAggregate, TRepository, TResponse>(unitOfWork, mapper, cacheProvider)
    where TQuery : AggregateQuery<TResponse>
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate> {
    /// <summary>
    /// 构建聚合根通用查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected List<Expression<Func<TAggregate, bool>>> BuildAggregatePredicates(TQuery query) {
        var predicates = new List<Expression<Func<TAggregate, bool>>>();

        if (query.Id.HasValue) {
            predicates.Add(t => t.Id == query.Id.Value);
        }
        if (!string.IsNullOrWhiteSpace(query.Description)) {
            predicates.Add(t => t.Description != null && t.Description.Contains(query.Description));
        }
        if (query.IsDeleted.HasValue) {
            predicates.Add(t => t.IsDeleted == query.IsDeleted.Value);
        }
        if (query.CreatedAtStart.HasValue) {
            predicates.Add(t => t.CreatedAt >= query.CreatedAtStart.Value);
        }
        if (query.CreatedAtEnd.HasValue) {
            predicates.Add(t => t.CreatedAt <= query.CreatedAtEnd.Value);
        }
        if (query.UpdatedAtStart.HasValue) {
            predicates.Add(t => t.UpdatedAt != null && t.UpdatedAt >= query.UpdatedAtStart.Value);
        }
        if (query.UpdatedAtEnd.HasValue) {
            predicates.Add(t => t.UpdatedAt != null && t.UpdatedAt <= query.UpdatedAtEnd.Value);
        }
        if (query.DeletedAtStart.HasValue) {
            predicates.Add(t => t.DeletedAt != null && t.DeletedAt >= query.DeletedAtStart.Value);
        }
        if (query.DeletedAtEnd.HasValue) {
            predicates.Add(t => t.DeletedAt != null && t.DeletedAt <= query.DeletedAtEnd.Value);
        }
        if (!string.IsNullOrWhiteSpace(query.CreatedBy)) {
            predicates.Add(t => t.CreatedBy != null && t.CreatedBy.Value.ToString().Contains(query.CreatedBy));
        }
        if (!string.IsNullOrWhiteSpace(query.UpdatedBy)) {
            predicates.Add(t => t.UpdatedBy != null && t.UpdatedBy.Value.ToString().Contains(query.UpdatedBy));
        }
        if (!string.IsNullOrWhiteSpace(query.DeletedBy)) {
            predicates.Add(t => t.DeletedBy != null && t.DeletedBy.Value.ToString().Contains(query.DeletedBy));
        }

        return predicates;
    }

    /// <summary>
    /// 构建聚合根通用排序条件
    /// </summary>
    /// <returns>排序条件字典</returns>
    protected IDictionary<Expression<Func<TAggregate, object>>, OrderByType> BuildAggregateOrders() {
        var orders = new Dictionary<Expression<Func<TAggregate, object>>, OrderByType>();
        orders.Add(t => t.CreatedAt, OrderByType.Desc);
        return orders;
    }

    /// <summary>
    /// 构建聚合根通用缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected string BuildAggregateCacheKey(TQuery query) {
        var parts = new List<string>();

        if (query.Id.HasValue) {
            parts.Add($"Id={query.Id.Value}");
        }
        if (!string.IsNullOrWhiteSpace(query.Description)) {
            parts.Add($"Description={query.Description}");
        }
        if (query.IsDeleted.HasValue) {
            parts.Add($"IsDeleted={query.IsDeleted.Value}");
        }
        if (query.CreatedAtStart.HasValue) {
            parts.Add($"CreatedAtStart={query.CreatedAtStart.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (query.CreatedAtEnd.HasValue) {
            parts.Add($"CreatedAtEnd={query.CreatedAtEnd.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (query.UpdatedAtStart.HasValue) {
            parts.Add($"UpdatedAtStart={query.UpdatedAtStart.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (query.UpdatedAtEnd.HasValue) {
            parts.Add($"UpdatedAtEnd={query.UpdatedAtEnd.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (query.DeletedAtStart.HasValue) {
            parts.Add($"DeletedAtStart={query.DeletedAtStart.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (query.DeletedAtEnd.HasValue) {
            parts.Add($"DeletedAtEnd={query.DeletedAtEnd.Value:yyyy-MM-dd HH:mm:ss}");
        }
        if (!string.IsNullOrWhiteSpace(query.CreatedBy)) {
            parts.Add($"CreatedBy={query.CreatedBy}");
        }
        if (!string.IsNullOrWhiteSpace(query.UpdatedBy)) {
            parts.Add($"UpdatedBy={query.UpdatedBy}");
        }
        if (!string.IsNullOrWhiteSpace(query.DeletedBy)) {
            parts.Add($"DeletedBy={query.DeletedBy}");
        }

        return string.Join("|", parts);
    }
}
