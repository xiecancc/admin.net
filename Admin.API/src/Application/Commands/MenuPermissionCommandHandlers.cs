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
using Infrastructure.Shared.Units;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 菜单权限创建命令处理器
/// <para>处理菜单权限的创建操作，包含权限编码唯一性验证和父级权限存在性验证</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionCreateCommandHandler> logger)
    : DomainCreateCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionCreateCommand, MenuPermissionCreateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 处理菜单权限创建命令
    /// </summary>
    /// <param name="request">创建命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="KeyNotFoundException">当父级权限不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">当权限编码已存在或创建操作失败时抛出</exception>
    public override async Task<bool> Handle(MenuPermissionCreateCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateCreateData(request.Data);

        try {
            Logger.LogInformation("开始创建菜单权限，数量: {Count}", request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
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
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("创建菜单权限成功，数量: {Count}", request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("创建菜单权限操作被取消");
            throw;
        }
        catch (InvalidOperationException) {
            throw;
        }
        catch (KeyNotFoundException) {
            throw;
        }
        catch (Exception ex) {
            LogException(ex, "创建", $"数量: {request.Data.Count}");
            throw HandleException(ex, "创建", $"数量: {request.Data.Count}");
        }
    }
}

/// <summary>
/// 菜单权限更新命令处理器
/// <para>处理菜单权限的更新操作，包含权限编码唯一性验证和父级权限存在性验证</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionUpdateCommand, MenuPermissionUpdateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 处理菜单权限更新命令
    /// </summary>
    /// <param name="request">更新命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="KeyNotFoundException">当权限或父级权限不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">当权限编码冲突或更新操作失败时抛出</exception>
    public override async Task<bool> Handle(MenuPermissionUpdateCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateUpdateData(request.Data);

        try {
            Logger.LogInformation("开始更新菜单权限，数量: {Count}", request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
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

                    UpdateEntity(entity, dto);
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
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("更新菜单权限成功，数量: {Count}", request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("更新菜单权限操作被取消");
            throw;
        }
        catch (InvalidOperationException) {
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
}

/// <summary>
/// 菜单权限删除命令处理器
/// <para>处理菜单权限的删除操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionDeleteCommand, MenuPermissionActionDto>(unitOfWork, mapper, logger);

/// <summary>
/// 菜单权限恢复命令处理器
/// <para>处理菜单权限的恢复操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class MenuPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MenuPermissionRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionRestoreCommand, MenuPermissionActionDto>(unitOfWork, mapper, logger);
