/*
 * 文件名称: ApiPermissionCommandHandlers.cs
 * 功能描述: API权限命令处理器，处理API权限相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
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
/// API权限创建命令处理器（Template Method 模式）
/// </summary>
public class ApiPermissionCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IPermissionRepository<ApiPermission> repository,
    IMenuPermissionRepository menuPermissionRepository,
    IMapper mapper,
    ILogger<ApiPermissionCreateCommandHandler> logger)
    : DomainCreateCommandHandler<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionCreateCommand, ApiPermissionCreateDto>(unitOfWork, repository, mapper, logger) {

    /// <summary>
    /// 在事务内执行API权限创建业务逻辑（包含编码唯一性和关联菜单验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(ApiPermissionCreateCommand request, CancellationToken cancellationToken) {
        var duplicateCodes = new List<string>();
        var notFoundMenuIds = new List<Guid>();

        foreach (var dto in request.Data) {
            var codeExists = await Repository.IsCodeExistsAsync(dto.Code, cancellationToken);
            if (codeExists) {
                duplicateCodes.Add(dto.Code);
                continue;
            }

            if (dto.MenuId.HasValue && dto.MenuId.Value != Guid.Empty) {
                var menuExists = await menuPermissionRepository.ExistsAsync([m => m.Id == dto.MenuId.Value], cancellationToken);
                if (!menuExists) {
                    notFoundMenuIds.Add(dto.MenuId.Value);
                }
            }
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"API权限创建失败，以下权限编码已存在: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的权限编码。");
        }

        if (notFoundMenuIds.Count > 0) {
            throw new KeyNotFoundException(
                $"API权限创建失败，以下关联菜单权限不存在: {string.Join(", ", notFoundMenuIds)}。" +
                "请确认菜单权限ID是否正确。");
        }

        var entities = Mapper.Map<List<ApiPermission>>(request.Data);
        return await Repository.InsertAsync(entities, cancellationToken);
    }
}

/// <summary>
/// API权限更新命令处理器（Template Method 模式）
/// </summary>
public class ApiPermissionUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IPermissionRepository<ApiPermission> repository,
    IMenuPermissionRepository menuPermissionRepository,
    IMapper mapper,
    ILogger<ApiPermissionUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionUpdateCommand, ApiPermissionUpdateDto>(unitOfWork, repository, mapper, logger) {

    /// <summary>
    /// 在事务内执行API权限更新业务逻辑（包含编码唯一性、实体存在性和关联菜单验证）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(ApiPermissionUpdateCommand request, CancellationToken cancellationToken) {
        var entities = new List<ApiPermission>();
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
                var menuExists = await menuPermissionRepository.ExistsAsync([m => m.Id == dto.MenuId.Value], cancellationToken);
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
                $"API权限更新失败，以下权限不存在: {string.Join(", ", notFoundIds)}。" +
                "请确认权限ID是否正确。");
        }

        if (duplicateCodes.Count > 0) {
            throw new InvalidOperationException(
                $"API权限更新失败，以下权限编码已被使用: {string.Join(", ", duplicateCodes)}。" +
                "请使用不同的权限编码。");
        }

        if (notFoundMenuIds.Count > 0) {
            throw new KeyNotFoundException(
                $"API权限更新失败，以下关联菜单权限不存在: {string.Join(", ", notFoundMenuIds)}。" +
                "请确认菜单权限ID是否正确。");
        }

        return await Repository.UpdateAsync(entities, cancellationToken);
    }
}

/// <summary>
/// API权限删除命令处理器
/// </summary>
public class ApiPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IPermissionRepository<ApiPermission> repository, IMapper mapper, ILogger<ApiPermissionDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionDeleteCommand, ApiPermissionActionDto>(unitOfWork, repository, mapper, logger);

/// <summary>
/// API权限恢复命令处理器
/// </summary>
public class ApiPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IPermissionRepository<ApiPermission> repository, IMapper mapper, ILogger<ApiPermissionRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionRestoreCommand, ApiPermissionActionDto>(unitOfWork, repository, mapper, logger);
