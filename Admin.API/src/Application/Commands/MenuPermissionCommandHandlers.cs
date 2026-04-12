/*
 * 文件名称: MenuPermissionCommandHandlers.cs
 * 功能描述: 菜单权限命令处理器，处理菜单权限相关的命令
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
/// 菜单权限创建命令处理器（Template Method 模式）
/// </summary>
public class MenuPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionCreateCommandHandler> logger)
    : DomainCreateCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionCreateCommand, MenuPermissionCreateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 在事务内执行菜单权限创建业务逻辑（包含编码唯一性和父级权限验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(MenuPermissionCreateCommand request, CancellationToken cancellationToken) {
        var duplicateCodes = new List<string>();
        var notFoundParentIds = new List<Guid>();

        foreach (var dto in request.Data) {
            var codeExists = await Repository.IsCodeExistsAsync(dto.Code, cancellationToken);
            if (codeExists) {
                duplicateCodes.Add(dto.Code);
                continue;
            }

            if (dto.ParentId.HasValue && dto.ParentId.Value != Guid.Empty) {
                var parentExists = await Repository.ExistsAsync(p => p.Id == dto.ParentId.Value, cancellationToken);
                if (!parentExists) {
                    notFoundParentIds.Add(dto.ParentId.Value);
                }
            }
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"菜单权限创建失败，以下权限编码已存在: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的权限编码。");
        }

        if (notFoundParentIds.Count > 0) {
            throw new KeyNotFoundException(
                $"菜单权限创建失败，以下父级权限不存在: {string.Join(", ", notFoundParentIds)}。" +
                "请确认父级权限ID是否正确。");
        }

        var entities = Mapper.Map<List<MenuPermission>>(request.Data);
        return await Repository.InsertAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 菜单权限更新命令处理器（Template Method 模式）
/// </summary>
public class MenuPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionUpdateCommand, MenuPermissionUpdateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 在事务内执行菜单权限更新业务逻辑（包含编码唯一性、实体存在性和父级权限验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(MenuPermissionUpdateCommand request, CancellationToken cancellationToken) {
        var entities = new List<MenuPermission>();
        var notFoundIds = new List<Guid>();
        var duplicateCodes = new List<string>();
        var notFoundParentIds = new List<Guid>();

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
                        $"菜单权限更新失败：权限不能将自己设为父级权限。权限ID: {dto.Id}。" +
                        "请选择其他权限作为父级权限。");
                }

                var parentExists = await Repository.ExistsAsync(p => p.Id == dto.ParentId.Value, cancellationToken);
                if (!parentExists) {
                    notFoundParentIds.Add(dto.ParentId.Value);
                    continue;
                }
            }

            Mapper.Map(dto, entity);
            entities.Add(entity);
        }

        if (notFoundIds.Count > 0) {
            throw new KeyNotFoundException(
                $"菜单权限更新失败，以下权限不存在: {string.Join(", ", notFoundIds)}。" +
                "请确认权限ID是否正确。");
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"菜单权限更新失败，以下权限编码已被使用: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的权限编码。");
        }

        if (notFoundParentIds.Count > 0) {
            throw new KeyNotFoundException(
                $"菜单权限更新失败，以下父级权限不存在: {string.Join(", ", notFoundParentIds)}。" +
                "请确认父级权限ID是否正确。");
        }

        return await Repository.UpdateAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 菜单权限删除命令处理器
/// </summary>
public class MenuPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionDeleteCommand, MenuPermissionActionDto>(unitOfWork, mapper, logger);

/// <summary>
/// 菜单权限恢复命令处理器
/// </summary>
public class MenuPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionRestoreCommand, MenuPermissionActionDto>(unitOfWork, mapper, logger);
