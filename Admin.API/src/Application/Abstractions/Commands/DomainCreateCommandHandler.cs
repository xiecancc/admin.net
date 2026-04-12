/*
 * 文件名称: DomainCreateCommandHandler.cs
 * 功能描述: 通用创建命令处理器（Template Method 模式）
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Units;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Commands;

/// <summary>
/// 通用创建命令处理器（Template Method 模式）
/// 用于处理所有领域实体（非聚合根）的创建操作
/// </summary>
/// <typeparam name="TDomain">领域模型类型</typeparam>
/// <typeparam name="TRepository">仓储接口类型</typeparam>
/// <typeparam name="TCommand">创建命令类型</typeparam>
/// <typeparam name="TCreateDto">创建DTO类型</typeparam>
public abstract class DomainCreateCommandHandler<TDomain, TRepository, TCommand, TCreateDto>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : CommandHandler<TDomain, TRepository, TCommand>(unitOfWork, mapper, logger)
    where TDomain : DomainBase, new()
    where TRepository : IDomainRepository<TDomain>
    where TCommand : CreateCommand<TCreateDto>, IRequest<bool>
    where TCreateDto : CreateDto {

    /// <summary>
    /// 验证创建数据（可在子类中扩展）
    /// </summary>
    /// <param name="data">创建数据列表</param>
    /// <exception cref="ArgumentNullException">当数据为空时抛出</exception>
    /// <exception cref="ArgumentException">当数据列表为空时抛出</exception>
    protected virtual void ValidateCreateData(List<TCreateDto> data) {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Count == 0) throw new ArgumentException("创建数据不能为空", nameof(data));
    }

    /// <summary>
    /// 验证请求参数
    /// </summary>
    protected override void ValidateRequest(TCommand request, CancellationToken cancellationToken) {
        base.ValidateRequest(request, cancellationToken);
        ValidateCreateData(request.Data);
    }

    /// <inheritdoc/>
    protected override Task<bool> ExecuteInTransactionAsync(TCommand request, CancellationToken cancellationToken) {
        var entities = Mapper.Map<List<TDomain>>(request.Data);
        return Repository.InsertAsync(entities, cancellationToken);
    }
}
