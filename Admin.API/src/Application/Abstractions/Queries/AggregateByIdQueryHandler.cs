/*
 * 文件名称: AggregateByIdQueryHandler.cs
 * 功能描述: 聚合根详情查询处理器，用于处理聚合根实体的详情获取操作
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

namespace Application.Abstractions.Queries;

/// <summary>
/// 聚合根根据ID查询处理器
/// <para>用于处理聚合根实体的根据ID获取操作，支持缓存</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TQueryDto">查询参数 DTO 类型</typeparam>
/// <typeparam name="TDetailDto">响应DTO类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器、缓存提供者或日志记录器为 null 时抛出</exception>
public abstract class AggregateByIdQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TDetailDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<AggregateByIdQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TDetailDto>> logger) : AggregateQueryHandler<TAggregate, TRepository, TQuery, TQueryDto, TDetailDto>(unitOfWork, mapper, cacheProvider)
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TQuery : AggregateByIdQuery<TQueryDto, TDetailDto>
    where TQueryDto : AggregateQueryDto
    where TDetailDto : AggregateDetailDto {
    /// <summary>
    /// 日志记录器
    /// </summary>
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 处理根据ID查询命令
    /// </summary>
    /// <param name="request">根据ID查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应DTO</returns>
    /// <exception cref="ArgumentNullException">当请求为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当请求的 ID 为空时抛出</exception>
    /// <exception cref="KeyNotFoundException">当实体不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">当发生未知异常时抛出</exception>
    public override async Task<TDetailDto> Handle(TQuery request, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        if (request.Id == Guid.Empty) {
            throw new ArgumentException("ID 不能为空", nameof(request.Id));
        }

        var cacheKey = $"{CacheKeyPrefix}:detail:{request.Id}";

        try {
            var result = await GetOrSetCacheAsync(cacheKey, async () => {
                var entity = await Repository.GetAsync(request.Id, cancellationToken);
                return entity is null ? default : Mapper.Map<TDetailDto>(entity);
            });

            return result ?? throw new KeyNotFoundException($"{typeof(TAggregate).Name} 不存在，ID: {request.Id}");
        }
        catch (ArgumentNullException) {
            throw;
        }
        catch (ArgumentException) {
            throw;
        }
        catch (KeyNotFoundException) {
            throw;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("查询 {AggregateType} 详情被取消，ID: {Id}", typeof(TAggregate).Name, request.Id);
            throw;
        }
        catch (Exception ex) {
            Logger.LogError(ex, "查询 {AggregateType} 详情时发生异常，ID: {Id}", typeof(TAggregate).Name, request.Id);
            throw new InvalidOperationException($"查询 {typeof(TAggregate).Name} 详情时发生异常，ID: {request.Id}", ex);
        }
    }
}
