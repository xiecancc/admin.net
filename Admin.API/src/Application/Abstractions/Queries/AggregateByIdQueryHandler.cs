/*
 * 文件名称: AggregateByIdQueryHandler.cs
 * 功能描述: 聚合根详情查询处理器，用于处理聚合根实体的详情获取操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;

namespace Application.Abstractions.Queries;

/// <summary>
/// 聚合根根据ID查询处理器
/// <para>用于处理聚合根实体的根据ID获取操作，支持缓存</para>
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TResponseDto">响应DTO类型</typeparam>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
/// <exception cref="ArgumentNullException">当工作单元、映射器或缓存提供者为 null 时抛出</exception>
public abstract class AggregateByIdQueryHandler<TQuery, TAggregate, TRepository, TResponseDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateQueryHandler<TQuery, TAggregate, TRepository, TResponseDto>(unitOfWork, mapper, cacheProvider)
    where TQuery : AggregateByIdQuery<TResponseDto>
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TResponseDto : AggregateDetailDto {
    /// <summary>
    /// 处理根据ID查询命令
    /// </summary>
    /// <param name="request">根据ID查询命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应DTO</returns>
    public override async Task<TResponseDto> Handle(TQuery request, CancellationToken cancellationToken) {
        var cacheKey = $"{CacheKeyPrefix}:detail:{request.Id}";
        var result = await GetOrSetCacheAsync(cacheKey, async () => {
            var entity = await Repository.GetAsync(request.Id, cancellationToken);
            return Mapper.Map<TResponseDto>(entity);
        });

        return result == null ? throw new ArgumentException($"实体不存在，ID: {request.Id}") : result;
    }
}
