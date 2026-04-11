/*
 * 文件名称: DomainPagedQueryHandler.cs
 * 功能描述: 通用分页查询处理器，用于处理所有领域实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Shared.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
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
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class PagedQueryHandler<TQuery, TDomain, TRepository, TResponseDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : QueryHandler<TQuery, TDomain, TRepository, PagedResponse<TResponseDto>>(unitOfWork, mapper, cacheProvider)
    where TQuery : PagedQuery<TResponseDto>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TResponseDto : PagedDto {
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
