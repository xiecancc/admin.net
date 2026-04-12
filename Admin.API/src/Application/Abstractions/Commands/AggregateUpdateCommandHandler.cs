/*
 * 文件名称: AggregateUpdateCommandHandler.cs
 * 功能描述: 通用更新命令处理器，用于处理所有聚合根实体的更新操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Units;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Commands;

/// <summary>
/// 通用更新命令处理器
/// 用于处理所有聚合根实体的更新操作
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TUpdateDto">更新DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public abstract class AggregateUpdateCommandHandler<TDomain, TRepository, TCommand, TUpdateDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<AggregateUpdateCommandHandler<TDomain, TRepository, TCommand, TUpdateDto>> logger)
    : CommandHandler<TDomain, TRepository, TCommand, bool>(unitOfWork, mapper, logger)
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TCommand : AggregateUpdateCommand<TUpdateDto>
    where TUpdateDto : AggregateUpdateDto {

    /// <summary>
    /// 处理更新命令
    /// </summary>
    /// <param name="request">更新命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="KeyNotFoundException">当实体不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">当更新操作失败时抛出</exception>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateUpdateData(request.Data);

        try {
            Logger.LogInformation("开始更新 {EntityTypeName}，数量: {Count}",
                EntityTypeName, request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
                var entities = new List<TDomain>();
                var notFoundIds = new List<Guid>();

                foreach (var dto in request.Data) {
                    var entity = await Repository.GetAsync(dto.Id, cancellationToken);

                    if (entity is null) {
                        notFoundIds.Add(dto.Id);
                        continue;
                    }

                    UpdateEntity(entity, dto);
                    entities.Add(entity);
                }

                if (notFoundIds.Count > 0) {
                    throw new KeyNotFoundException(
                        $"{EntityTypeName} 更新失败，以下实体不存在: {string.Join(", ", notFoundIds)}");
                }

                return await Repository.UpdateAsync(entities, cancellationToken);
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("更新 {EntityTypeName} 成功，数量: {Count}",
                    EntityTypeName, request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("更新 {EntityTypeName} 操作被取消", EntityTypeName);
            throw;
        }
        catch (KeyNotFoundException) {
            throw;
        }
        catch (Exception ex) {
            LogException(ex, "更新", $"数量: {request.Data.Count}");
            throw HandleException(ex, "更新", $"数量: {request.Data.Count}");
        }
    }

    /// <summary>
    /// 验证更新数据
    /// </summary>
    /// <param name="data">更新数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    protected virtual void ValidateUpdateData(List<TUpdateDto> data) {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        if (data.Count == 0) {
            throw new ArgumentException("更新数据不能为空集合", nameof(data));
        }

        var emptyIds = data.Where(d => d.Id == Guid.Empty).ToList();
        if (emptyIds.Count > 0) {
            throw new ArgumentException("更新数据中存在空的 ID 值", nameof(data));
        }
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="dto">更新DTO</param>
    protected virtual void UpdateEntity(TDomain entity, TUpdateDto dto) {
        Mapper.Map(dto, entity);
    }
}
