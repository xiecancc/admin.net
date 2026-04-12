/*
 * 文件名称: ButtonPermissionCommandHandlers.cs
 * 功能描述: 按钮权限命令处理器，处理按钮权限相关的命令
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
/// 按钮权限创建命令处理器（Template Method 模式）
/// </summary>
public class ButtonPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionCreateCommandHandler> logger)
    : DomainCreateCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionCreateCommand, ButtonPermissionCreateDto>(unitOfWork, mapper, logger) {

    private readonly IMenuPermissionRepository _menuPermissionRepository = unitOfWork.GetRepository<IMenuPermissionRepository, MenuPermission>();

    /// <summary>
    /// 在事务内执行按钮权限创建业务逻辑（包含编码唯一性和关联菜单验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(ButtonPermissionCreateCommand request, CancellationToken cancellationToken) {
        var duplicateCodes = new List<string>();
        var notFoundMenuIds = new List<Guid>();

        foreach (var dto in request.Data) {
            var codeExists = await Repository.IsCodeExistsAsync(dto.Code, cancellationToken);
            if (codeExists) {
                duplicateCodes.Add(dto.Code);
                continue;
            }

            if (dto.MenuId.HasValue && dto.MenuId.Value != Guid.Empty) {
                var menuExists = await _menuPermissionRepository.ExistsAsync(m => m.Id == dto.MenuId.Value, cancellationToken);
                if (!menuExists) {
                    notFoundMenuIds.Add(dto.MenuId.Value);
                }
            }
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"按钮权限创建失败，以下权限编码已存在: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的权限编码。");
        }

        if (notFoundMenuIds.Count > 0) {
            throw new KeyNotFoundException(
                $"按钮权限创建失败，以下关联菜单权限不存在: {string.Join(", ", notFoundMenuIds)}。" +
                "请确认菜单权限ID是否正确。");
        }

        var entities = Mapper.Map<List<ButtonPermission>>(request.Data);
        return await Repository.InsertAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 按钮权限更新命令处理器（Template Method 模式）
/// </summary>
public class ButtonPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionUpdateCommand, ButtonPermissionUpdateDto>(unitOfWork, mapper, logger) {

    private readonly IMenuPermissionRepository _menuPermissionRepository = unitOfWork.GetRepository<IMenuPermissionRepository, MenuPermission>();

    /// <summary>
    /// 在事务内执行按钮权限更新业务逻辑（包含编码唯一性、实体存在性和关联菜单验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(ButtonPermissionUpdateCommand request, CancellationToken cancellationToken) {
        var entities = new List<ButtonPermission>();
        var notFoundIds = new List<Guid>();
        var duplicateCodes = new List<string>();
        var notFoundMenuIds = new List<Guid>();

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

            if (dto.MenuId.HasValue && dto.MenuId.Value != Guid.Empty) {
                var menuExists = await _menuPermissionRepository.ExistsAsync(m => m.Id == dto.MenuId.Value, cancellationToken);
                if (!menuExists) {
                    notFoundMenuIds.Add(dto.MenuId.Value);
                    continue;
                }
            }

            Mapper.Map(dto, entity);
            entities.Add(entity);
        }

        if (notFoundIds.Count > 0) {
            throw new KeyNotFoundException(
                $"按钮权限更新失败，以下权限不存在: {string.Join(", ", notFoundIds)}。" +
                "请确认权限ID是否正确。");
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"按钮权限更新失败，以下权限编码已被使用: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的权限编码。");
        }

        if (notFoundMenuIds.Count > 0) {
            throw new KeyNotFoundException(
                $"按钮权限更新失败，以下关联菜单权限不存在: {string.Join(", ", notFoundMenuIds)}。" +
                "请确认菜单权限ID是否正确。");
        }

        return await Repository.UpdateAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 按钮权限删除命令处理器
/// </summary>
public class ButtonPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionDeleteCommand, ButtonPermissionActionDto>(unitOfWork, mapper, logger);

/// <summary>
/// 按钮权限恢复命令处理器
/// </summary>
public class ButtonPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionRestoreCommand, ButtonPermissionActionDto>(unitOfWork, mapper, logger);
