/*
 * 文件名称: DomainCreateCommandHandler.cs
 * 功能描述: 通用创建命令处理器，用于处理所有领域实体的创建操作
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
/// 通用创建命令处理器
/// 用于处理所有领域实体的创建操作，包括聚合根和关系表
/// </summary>
/// <typeparam name="TDomain">实体类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TCreateDto">请求DTO类型</typeparam>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public abstract class DomainCreateCommandHandler<TDomain, TRepository, TCommand, TCreateDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<DomainCreateCommandHandler<TDomain, TRepository, TCommand, TCreateDto>> logger)
    : CommandHandler<TDomain, TRepository, TCommand, bool>(unitOfWork, mapper, logger)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TCommand : CreateCommand<TCreateDto>
    where TCreateDto : CreateDto {

    /// <summary>
    /// 处理创建命令
    /// </summary>
    /// <param name="request">创建命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当创建操作失败时抛出</exception>
    public override async Task<bool> Handle(TCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateCreateData(request.Data);

        try {
            Logger.LogInformation("开始创建 {EntityTypeName}，数量: {Count}",
                EntityTypeName, request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
                var entities = Mapper.Map<List<TDomain>>(request.Data);
                return await Repository.InsertAsync(entities, cancellationToken);
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("创建 {EntityTypeName} 成功，数量: {Count}",
                    EntityTypeName, request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("创建 {EntityTypeName} 操作被取消", EntityTypeName);
            throw;
        }
        catch (Exception ex) {
            LogException(ex, "创建", $"数量: {request.Data.Count}");
            throw HandleException(ex, "创建", $"数量: {request.Data.Count}");
        }
    }

    /// <summary>
    /// 验证创建数据
    /// </summary>
    /// <param name="data">创建数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    protected virtual void ValidateCreateData(List<TCreateDto> data) {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        if (data.Count == 0) {
            throw new ArgumentException("创建数据不能为空集合", nameof(data));
        }
    }
}
