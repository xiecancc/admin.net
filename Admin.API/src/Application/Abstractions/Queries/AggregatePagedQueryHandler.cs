/*
 * 文件名称: AggregatePagedQueryHandler.cs
 * 功能描述: 聚合根分页查询处理器，用于处理聚合根实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Dtos;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;
using SqlSugar;
using System.Linq.Expressions;
using System.Text;

namespace Application.Abstractions.Queries;

/// <summary>
/// 聚合根分页查询处理器
/// <para>用于处理聚合根实体的分页获取操作，包含通用查询条件构建逻辑</para>
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponseDto">响应DTO类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class AggregatePagedQueryHandler<TQuery, TAggregate, TRepository, TResponseDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider)
    : AggregateQueryHandler<TQuery, TAggregate, TRepository, PagedResponse<TResponseDto>>(unitOfWork, mapper, cacheProvider)
    where TQuery : AggregatePagedQuery<TResponseDto>
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TResponseDto : AggregatePagedDto {
    /// <summary>
    /// 构建分页查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<TAggregate, bool>>> BuildPredicates(TQuery query) {
        var predicates = base.BuildPredicates(query);
        if (!string.IsNullOrWhiteSpace(query.Id)) {
            predicates.Add(t => t.Id.ToString().Contains(query.Id));
        }
        return predicates;
    }

    /// <summary>
    /// 构建分页查询缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(TQuery query) {
        var builder = base.BuildCacheParams(query);
        if (!string.IsNullOrWhiteSpace(query.Id)) {
            builder.Append($"|Id={query.Id}");
        }
        return builder;
    }

    /// <summary>
    /// 处理分页查询命令
    /// </summary>
    /// <param name="request">分页查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应</returns>
    public override async Task<PagedResponse<TResponseDto>> Handle(TQuery request, CancellationToken cancellationToken) {
        var predicates = BuildPredicates(request);
        var orders = BuildOrders(request);
        var cacheKey = $"{CacheKeyPrefix}:paged:{request.Page}:{request.Size}:{BuildCacheParams(request)}";

        var result = await GetOrSetCacheAsync(cacheKey, async () => {
            var entities = await Repository.GetPagedAsync(predicates, orders, request.Page, request.Size, cancellationToken);
            return Mapper.Map<PagedResponse<TResponseDto>>(entities);
        });

        return result ?? new PagedResponse<TResponseDto>([], 0, request.Page, request.Size);
    }


}
