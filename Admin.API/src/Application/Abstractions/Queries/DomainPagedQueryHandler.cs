/*
 * 文件名称: DomainPagedQueryHandler.cs
 * 功能描述: 通用分页查询处理器，用于处理所有领域实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Shared.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using SqlSugar;
using System.Linq.Expressions;
using AutoMapper;

namespace Application.Abstractions.Queries;

/// <summary>
/// 通用分页查询处理器
/// <para>用于处理所有领域实体的分页获取操作，包括聚合根和关系表，支持缓存</para>
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponseDto">响应DTO类型</typeparam>
/// <typeparam name="TQueryParameters">查询参数类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class DomainPagedQueryHandler<TQuery, TDomain, TRepository, TResponseDto, TQueryParameters>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainQueryHandler<TQuery, TDomain, TRepository, PagedResponse<TResponseDto>>(unitOfWork, mapper, cacheProvider)
    where TQuery : DomainPagedQuery<TDomain, TRepository, TResponseDto, TQueryParameters>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TResponseDto : DomainPagedDto
    where TQueryParameters : DomainQueryParameters<TDomain>, new() {
    /// <summary>
    /// 处理分页查询命令
    /// </summary>
    /// <param name="request">分页查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应</returns>
    public override async Task<PagedResponse<TResponseDto>> Handle(TQuery request, CancellationToken cancellationToken) {
        var (conditions, sortConditions, (page, size)) = BuildParameters(request);
        var queryCacheKey = request.QueryParameters.CacheKey();
        var cacheKey = $"{CacheKeyPrefix}:paged:{page}:{size}:{queryCacheKey}";

        var result = await GetOrSetCacheAsync(cacheKey, async () => {
            var entities = await Repository.GetPagedAsync(conditions, sortConditions, page, size, cancellationToken);
            var items = Mapper.Map<List<TResponseDto>>(entities.Items);
            return new PagedResponse<TResponseDto>(items, entities.Total, entities.Page, entities.Size);
        });

        return result ?? new PagedResponse<TResponseDto>([], 0, page, size);
    }

    /// <summary>
    /// 构建查询参数
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>(条件, 排序, 页码, 大小)</returns>
    protected virtual (List<Expression<Func<TDomain, bool>>>, IDictionary<Expression<Func<TDomain, object>>, OrderByType>, (int, int)) BuildParameters(TQuery request) {
        var parameters = request.QueryParameters;
        return (parameters.Predicates(), parameters.Orders(), parameters.Pagination(request.Page, request.Size));
    }
}
