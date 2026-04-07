/*
 * 文件名称: DomainListQueryHandler.cs
 * 功能描述: 通用列表查询处理器，用于处理所有领域实体的列表查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;
using SqlSugar;
using System.Linq.Expressions;
using Application.Contracts.Abstractions.Queries;

namespace Application.Abstractions.Queries;

/// <summary>
/// 通用列表查询处理器
/// <para>用于处理所有领域实体的列表查询操作，包括聚合根和关系表，支持缓存</para>
/// </summary>
/// <typeparam name="TQuery">查询命令类型</typeparam>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TListDto">列表DTO类型</typeparam>
/// <typeparam name="TQueryParameters">查询参数类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class DomainListQueryHandler<TQuery, TDomain, TRepository, TListDto, TQueryParameters>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainQueryHandler<TQuery, TDomain, TRepository, List<TListDto>>(unitOfWork, mapper, cacheProvider)
    where TQuery : DomainListQuery<TDomain, TRepository, TListDto, TQueryParameters>
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TListDto : DomainListDto
    where TQueryParameters : DomainQueryParameters<TDomain>, new() {
    /// <summary>
    /// 处理列表查询命令
    /// </summary>
    /// <param name="request">列表查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>列表数据</returns>
    public override async Task<List<TListDto>> Handle(TQuery request, CancellationToken cancellationToken) {
        var (predicates, orders) = BuildParameters(request);
        var queryCacheKey = request.QueryParameters.CacheKey();
        var cacheKey = $"{CacheKeyPrefix}:list:{queryCacheKey}";

        var result = await GetOrSetCacheAsync(cacheKey, async () => {
            var entities = await Repository.GetListAsync(predicates, orders, cancellationToken);
            return Mapper.Map<List<TListDto>>(entities);
        });

        return result ?? [];
    }

    /// <summary>
    /// 构建查询参数
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>(条件, 排序)</returns>
    protected virtual (List<Expression<Func<TDomain, bool>>>, IDictionary<Expression<Func<TDomain, object>>, OrderByType>) BuildParameters(TQuery request) {
        var parameters = request.QueryParameters;
        return (parameters.Predicates(), parameters.Orders());
    }
}
