/*
 * 文件名称: AggregateRestoreCommandHandler.cs
 * 功能描述: 通用恢复命令处理器，用于处理所有聚合根实体的恢复操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Shared.Entities;
using Infrastructure.Shared.Units;
using AutoMapper;
using Domain.Shared.Repositories;
using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Commands;

/// <summary>
/// 通用恢复命令处理器
/// 用于处理所有聚合根实体的恢复操作
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TActionDto">操作DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public abstract class AggregateRestoreCommandHandler<TDomain, TRepository, TCommand, TActionDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<AggregateRestoreCommandHandler<TDomain, TRepository, TCommand, TActionDto>> logger)
    : CommandHandler<TDomain, TRepository, TCommand, bool>(unitOfWork, mapper, logger)
    where TDomain : AggregateBase, new()
    where TRepository : IAggregateRepository<TDomain>
    where TCommand : AggregateRestoreCommand<TActionDto>
    where TActionDto : AggregateActionDto {

    /// <summary>
    /// 处理恢复命令
    /// </summary>
    /// <param name="request">恢复命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当恢复操作失败时抛出</exception>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateRestoreData(request.Data);

        try {
            var ids = request.Data.Select(dto => dto.Id).ToList();
            Logger.LogInformation("开始恢复 {EntityTypeName}，数量: {Count}，ID: {Ids}",
                EntityTypeName, ids.Count, string.Join(", ", ids));

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
                return await Repository.RestoreAsync(ids, cancellationToken);
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("恢复 {EntityTypeName} 成功，数量: {Count}",
                    EntityTypeName, ids.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("恢复 {EntityTypeName} 操作被取消", EntityTypeName);
            throw;
        }
        catch (Exception ex) {
            var ids = request.Data.Select(dto => dto.Id).ToList();
            LogException(ex, "恢复", $"ID: {string.Join(", ", ids)}");
            throw HandleException(ex, "恢复", $"ID: {string.Join(", ", ids)}");
        }
    }

    /// <summary>
    /// 验证恢复数据
    /// </summary>
    /// <param name="data">恢复数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    protected virtual void ValidateRestoreData(List<TActionDto> data) {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        if (data.Count == 0) {
            throw new ArgumentException("恢复数据不能为空集合", nameof(data));
        }

        var emptyIds = data.Where(d => d.Id == Guid.Empty).ToList();
        if (emptyIds.Count > 0) {
            throw new ArgumentException("恢复数据中存在空的 ID 值", nameof(data));
        }
    }
}
