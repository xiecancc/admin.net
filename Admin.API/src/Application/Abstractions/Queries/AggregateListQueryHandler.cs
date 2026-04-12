/*
 * 文件名称: AggregateListQueryHandler.cs
 * 功能描述: 聚合根列表查询处理器，用于处理聚合根实体的列表查询操作
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
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text;

namespace Application.Abstractions.Queries;

/// <summary>
/// 聚合根列表查询处理器
/// <para>用于处理聚合根实体的列表查询操作，包含通用查询条件构建逻辑</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TListDto">列表DTO类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器、缓存提供者或日志记录器为 null 时抛出</exception>
public abstract class AggregateListQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TListDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<AggregateListQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TListDto>> logger) : AggregateQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, List<TListDto>>(unitOfWork, mapper, cacheProvider)
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TQuery : AggregateListQuery<TQueryDto, TListDto>
    where TQueryDto : AggregateQueryDto
    where TListDto : AggregateListDto {
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 构建列表查询条件
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
    /// 构建列表查询缓存键参数部分
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
            Logger.LogWarning("查询 {AggregateType} 列表被取消", typeof(TAggregate).Name);
            throw;
        }
        catch (Exception ex) {
            Logger.LogError(ex, "查询 {AggregateType} 列表时发生异常", typeof(TAggregate).Name);
            throw new InvalidOperationException($"查询 {typeof(TAggregate).Name} 列表时发生异常", ex);
        }
    }
}
