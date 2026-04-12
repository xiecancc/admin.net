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
using Infrastructure.Shared.Units;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 按钮权限创建命令处理器
/// <para>处理按钮权限的创建操作，包含权限编码唯一性验证和关联菜单权限存在性验证</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class ButtonPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionCreateCommandHandler> logger)
    : DomainCreateCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionCreateCommand, ButtonPermissionCreateDto>(unitOfWork, mapper, logger) {

    private readonly IMenuPermissionRepository _menuPermissionRepository = unitOfWork.GetRepository<IMenuPermissionRepository, MenuPermission>();

    /// <summary>
    /// 处理按钮权限创建命令
    /// </summary>
    /// <param name="request">创建命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="KeyNotFoundException">当关联菜单权限不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">当权限编码已存在或创建操作失败时抛出</exception>
    public override async Task<bool> Handle(ButtonPermissionCreateCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateCreateData(request.Data);

        try {
            Logger.LogInformation("开始创建按钮权限，数量: {Count}", request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
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
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("创建按钮权限成功，数量: {Count}", request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("创建按钮权限操作被取消");
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
/// 按钮权限更新命令处理器
/// <para>处理按钮权限的更新操作，包含权限编码唯一性验证和关联菜单权限存在性验证</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class ButtonPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionUpdateCommand, ButtonPermissionUpdateDto>(unitOfWork, mapper, logger) {

    private readonly IMenuPermissionRepository _menuPermissionRepository = unitOfWork.GetRepository<IMenuPermissionRepository, MenuPermission>();

    /// <summary>
    /// 处理按钮权限更新命令
    /// </summary>
    /// <param name="request">更新命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="KeyNotFoundException">当权限或关联菜单权限不存在时抛出</exception>
    /// <exception cref="InvalidOperationException">当权限编码冲突或更新操作失败时抛出</exception>
    public override async Task<bool> Handle(ButtonPermissionUpdateCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateUpdateData(request.Data);

        try {
            Logger.LogInformation("开始更新按钮权限，数量: {Count}", request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
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

                    UpdateEntity(entity, dto);
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
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("更新按钮权限成功，数量: {Count}", request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("更新按钮权限操作被取消");
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
/// 按钮权限删除命令处理器
/// <para>处理按钮权限的删除操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class ButtonPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionDeleteCommand, ButtonPermissionActionDto>(unitOfWork, mapper, logger);

/// <summary>
/// 按钮权限恢复命令处理器
/// <para>处理按钮权限的恢复操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class ButtonPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ButtonPermissionRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionRestoreCommand, ButtonPermissionActionDto>(unitOfWork, mapper, logger);
