/*
 * 文件名称: DomainPagedQueryHandler.cs
 * 功能描述: 通用分页查询处理器，用于处理所有领域实体的分页获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Shared.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Caches;
using Domain.Shared.Units;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using AutoMapper;

namespace Application.Abstractions.Queries;

/// <summary>
/// 通用分页查询处理器
/// <para>用于处理所有领域实体的分页获取操作，包括聚合根和关系表，支持缓存</para>
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TResponseDto">响应DTO类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器、缓存提供者或日志记录器为 null 时抛出</exception>
public abstract class PagedQueryHandler<TDomain, TRepository, TQuery, TQueryDto, TResponseDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<PagedQueryHandler<TDomain, TRepository, TQuery, TQueryDto, TResponseDto>> logger) : QueryHandler<TDomain, TRepository, TQuery, TQueryDto, PagedResponse<TResponseDto>>(unitOfWork, mapper, cacheProvider)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TQuery : PagedQuery<TQueryDto, TResponseDto>
    where TQueryDto : QueryDto
    where TResponseDto : PagedDto {
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理分页查询命令
    /// </summary>
    /// <param name="request">分页查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应</returns>
    /// <exception cref="ArgumentNullException">当请求为 null 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当页码或页大小无效时抛出</exception>
    /// <exception cref="InvalidOperationException">当发生未知异常时抛出</exception>
    public override async Task<PagedResponse<TResponseDto>> Handle(TQuery request, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.Page < 1) {
            throw new ArgumentOutOfRangeException(nameof(request.Page), "页码必须大于等于 1");
        }

        if (request.Size < 1) {
            throw new ArgumentOutOfRangeException(nameof(request.Size), "页大小必须大于等于 1");
        }

        var predicates = BuildPredicates(request);
        var orders = BuildOrders(request);
        var cacheKey = $"{CacheKeyPrefix}:paged:{request.Page}:{request.Size}:{BuildCacheParams(request)}";

        try {
            var result = await GetOrSetCacheAsync(cacheKey, async () => {
                var entities = await Repository.GetPagedAsync(predicates, orders, request.Page, request.Size, cancellationToken);
                return Mapper.Map<PagedResponse<TResponseDto>>(entities);
            });

            return result ?? new PagedResponse<TResponseDto>([], 0, request.Page, request.Size);
        }
        catch (ArgumentNullException) {
            throw;
        }
        catch (ArgumentOutOfRangeException) {
            throw;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("查询 {DomainType} 分页被取消，页码: {Page}, 页大小: {Size}", typeof(TDomain).Name, request.Page, request.Size);
            throw;
        }
        catch (Exception ex) {
            Logger.LogError(ex, "查询 {DomainType} 分页时发生异常，页码: {Page}, 页大小: {Size}", typeof(TDomain).Name, request.Page, request.Size);
            throw new InvalidOperationException($"查询 {typeof(TDomain).Name} 分页时发生异常", ex);
        }
    }
}
