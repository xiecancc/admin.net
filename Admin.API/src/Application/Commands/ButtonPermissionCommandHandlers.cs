/*
 * 文件名称: ButtonPermissionCommandHandlers.cs
 * 功能描述: 按钮权限命令处理器，处理按钮权限相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Abstractions.Commands;
using Application.Contracts.Commands;
using Infrastructure.Shared.Units;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Application.Contracts.Dtos;

namespace Application.Commands;

/// <summary>
/// 按钮权限创建命令处理器
/// 处理按钮权限的创建操作
/// </summary>
public class ButtonPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : DomainCreateCommandHandler<ButtonPermissionCreateCommand, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionCreateDto>(unitOfWork, mapper);

/// <summary>
/// 按钮权限更新命令处理器
/// 处理按钮权限的更新操作
/// </summary>
public class ButtonPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateUpdateCommandHandler<ButtonPermissionUpdateCommand, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionUpdateDto>(unitOfWork, mapper);

/// <summary>
/// 按钮权限删除命令处理器
/// 处理按钮权限的删除操作
/// </summary>
public class ButtonPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateDeleteCommandHandler<ButtonPermissionDeleteCommand, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionActionDto>(unitOfWork, mapper);

/// <summary>
/// 按钮权限恢复命令处理器
/// 处理按钮权限的恢复操作
/// </summary>
public class ButtonPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateRestoreCommandHandler<ButtonPermissionRestoreCommand, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionActionDto>(unitOfWork, mapper);
