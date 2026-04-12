/*
 * 文件名称: RoleCommandHandlers.cs
 * 功能描述: 角色命令处理器，处理角色相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Commands;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Units;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 角色创建命令处理器
/// <para>处理角色的创建操作，包含角色编码唯一性验证</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RoleCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleCreateCommandHandler> logger)
    : DomainCreateCommandHandler<Role, IRoleRepository, RoleCreateCommand, RoleCreateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 在事务内执行角色创建业务逻辑（包含角色编码唯一性验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(RoleCreateCommand request, CancellationToken cancellationToken) {
        var duplicateCodes = new List<string>();

        foreach (var dto in request.Data) {
            var codeExists = await Repository.IsCodeExistsAsync(dto.Code, cancellationToken);
            if (codeExists) {
                duplicateCodes.Add(dto.Code);
            }
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"角色创建失败，以下角色编码已存在: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的角色编码。");
        }

        var entities = Mapper.Map<List<Role>>(request.Data);
        return await Repository.InsertAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 角色更新命令处理器
/// <para>处理角色的更新操作，包含角色编码唯一性验证和循环继承检测</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RoleUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<Role, IRoleRepository, RoleUpdateCommand, RoleUpdateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 在事务内执行角色更新业务逻辑（包含角色编码唯一性验证和循环继承检测）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(RoleUpdateCommand request, CancellationToken cancellationToken) {
        var entities = new List<Role>();
        var notFoundIds = new List<Guid>();
        var duplicateCodes = new List<string>();

        foreach (var dto in request.Data) {
            var entity = await Repository.GetAsync(dto.Id, cancellationToken);

            if (entity is null) {
                notFoundIds.Add(dto.Id);
                continue;
            }

            if (!string.Equals(entity.Code, dto.Code, StringComparison.OrdinalIgnoreCase)) {
                var codeExists = await Repository.IsCodeExistsAsync(dto.Code, cancellationToken);
                if (codeExists) {
                    duplicateCodes.Add(dto.Code);
                    continue;
                }
            }

            if (dto.ParentId.HasValue && dto.ParentId.Value != Guid.Empty) {
                if (dto.ParentId.Value == dto.Id) {
                    throw new InvalidOperationException(
                        $"角色更新失败：角色不能将自己设为父角色。角色ID: {dto.Id}。" +
                        "请选择其他角色作为父角色。");
                }

                var parentExists = await Repository.ExistsAsync(r => r.Id == dto.ParentId.Value, cancellationToken);
                if (!parentExists) {
                    throw new KeyNotFoundException(
                        $"角色更新失败：父角色不存在。父角色ID: {dto.ParentId.Value}。" +
                        "请确认父角色ID是否正确。");
                }

                var hasCycle = await Repository.HasInheritanceCycleAsync(dto.Id, dto.ParentId.Value, cancellationToken);

                if (hasCycle) {
                    throw new InvalidOperationException(
                        $"角色更新失败：检测到循环继承。角色ID: {dto.Id}，父角色ID: {dto.ParentId.Value}。" +
                        "请检查角色继承链，确保不会形成循环引用。");
                }
            }

            Mapper.Map(dto, entity);
            entities.Add(entity);
        }

        if (notFoundIds.Count > 0) {
            throw new KeyNotFoundException(
                $"角色更新失败，以下角色不存在: {string.Join(", ", notFoundIds)}。" +
                "请确认角色ID是否正确。");
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"角色更新失败，以下角色编码已被使用: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的角色编码。");
        }

        return await Repository.UpdateAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 角色删除命令处理器
/// <para>处理角色的删除操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RoleDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<Role, IRoleRepository, RoleDeleteCommand, RoleActionDto>(unitOfWork, mapper, logger);

/// <summary>
/// 角色恢复命令处理器
/// <para>处理角色的恢复操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class RoleRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<Role, IRoleRepository, RoleRestoreCommand, RoleActionDto>(unitOfWork, mapper, logger);
