/*
 * 文件名称: DomainListQueryHandler.cs
 * 功能描述: 通用列表查询处理器，用于处理所有领域实体的列表查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Caches;
using Domain.Shared.Units;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Abstractions.Queries;

/// <summary>
/// 通用列表查询处理器
/// <para>用于处理所有领域实体的列表查询操作，包括聚合根和关系表，支持缓存</para>
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TQuery">查询命令类型</typeparam>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TListDto">列表DTO类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器、缓存提供者或日志记录器为 null 时抛出</exception>
public abstract class ListQueryHandler<TDomain, TRepository, TQuery, TQueryDto, TListDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<ListQueryHandler<TDomain, TRepository, TQuery, TQueryDto, TListDto>> logger) : QueryHandler<TDomain, TRepository, TQuery, TQueryDto, List<TListDto>>(unitOfWork, mapper, cacheProvider)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TQuery : ListQuery<TQueryDto, TListDto>
    where TQueryDto : QueryDto
    where TListDto : ListDto {
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理列表查询命令
    /// </summary>
    /// <param name="request">列表查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>列表数据</returns>
    /// <exception cref="ArgumentNullException">当请求为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当发生未知异常时抛出</exception>
    public override async Task<List<TListDto>> Handle(TQuery request, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var predicates = BuildPredicates(request);
        var orders = BuildOrders(request);
        var cacheKey = $"{CacheKeyPrefix}:list:{BuildCacheParams(request)}";

        try {
            var result = await GetOrSetCacheAsync(cacheKey, async () => {
                var entities = await Repository.GetListAsync(predicates, orders, cancellationToken);
                return Mapper.Map<List<TListDto>>(entities);
            });

            return result ?? [];
        }
        catch (ArgumentNullException) {
            throw;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("查询 {DomainType} 列表被取消", typeof(TDomain).Name);
            throw;
        }
        catch (Exception ex) {
            Logger.LogError(ex, "查询 {DomainType} 列表时发生异常", typeof(TDomain).Name);
            throw new InvalidOperationException($"查询 {typeof(TDomain).Name} 列表时发生异常", ex);
        }
    }
}
