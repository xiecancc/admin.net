/*
 * 文件名称: AggregateDeleteCommandHandler.cs
 * 功能描述: 通用删除命令处理器，用于处理所有聚合根实体的删除操作
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
/// 聚合根删除命令处理器
/// 用于处理所有聚合根实体的删除操作
/// </summary>
/// <typeparam name="TAggregate">聚合根类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TActionDto">操作DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public abstract class AggregateDeleteCommandHandler<TAggregate, TRepository, TCommand, TActionDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<AggregateDeleteCommandHandler<TAggregate, TRepository, TCommand, TActionDto>> logger)
    : CommandHandler<TAggregate, TRepository, TCommand, bool>(unitOfWork, mapper, logger)
    where TAggregate : AggregateBase, new()
    where TRepository : IAggregateRepository<TAggregate>
    where TCommand : AggregateDeleteCommand<TActionDto>
    where TActionDto : AggregateActionDto {

    /// <summary>
    /// 处理删除命令
    /// </summary>
    /// <param name="request">删除命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当删除操作失败时抛出</exception>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateDeleteData(request.Data);

        try {
            var ids = request.Data.Select(dto => dto.Id).ToList();
            Logger.LogInformation("开始删除 {EntityTypeName}，数量: {Count}，ID: {Ids}",
                EntityTypeName, ids.Count, string.Join(", ", ids));

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
                return await Repository.DeleteAsync(ids, cancellationToken);
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("删除 {EntityTypeName} 成功，数量: {Count}",
                    EntityTypeName, ids.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("删除 {EntityTypeName} 操作被取消", EntityTypeName);
            throw;
        }
        catch (Exception ex) {
            var ids = request.Data.Select(dto => dto.Id).ToList();
            LogException(ex, "删除", $"ID: {string.Join(", ", ids)}");
            throw HandleException(ex, "删除", $"ID: {string.Join(", ", ids)}");
        }
    }

    /// <summary>
    /// 验证删除数据
    /// </summary>
    /// <param name="data">删除数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    protected virtual void ValidateDeleteData(List<TActionDto> data) {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        if (data.Count == 0) {
            throw new ArgumentException("删除数据不能为空集合", nameof(data));
        }

        var emptyIds = data.Where(d => d.Id == Guid.Empty).ToList();
        if (emptyIds.Count > 0) {
            throw new ArgumentException("删除数据中存在空的 ID 值", nameof(data));
        }
    }
}
