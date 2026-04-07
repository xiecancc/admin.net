/*
 * 文件名称: MenuPermissionCommandHandlers.cs
 * 功能描述: 菜单权限命令处理器，处理菜单权限相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Abstractions.Commands;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Units;
using AutoMapper;

namespace Application.Commands;

/// <summary>
/// 菜单权限创建命令处理器
/// 处理菜单权限的创建操作
/// </summary>
public class MenuPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : DomainCreateCommandHandler<MenuPermissionCreateCommand, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionCreateDto>(unitOfWork, mapper);

/// <summary>
/// 菜单权限更新命令处理器
/// 处理菜单权限的更新操作
/// </summary>
public class MenuPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateUpdateCommandHandler<MenuPermissionUpdateCommand, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionUpdateDto>(unitOfWork, mapper);

/// <summary>
/// 菜单权限删除命令处理器
/// 处理菜单权限的删除操作
/// </summary>
public class MenuPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateDeleteCommandHandler<MenuPermissionDeleteCommand, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionActionDto>(unitOfWork, mapper);

/// <summary>
/// 菜单权限恢复命令处理器
/// 处理菜单权限的恢复操作
/// </summary>
public class MenuPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateRestoreCommandHandler<MenuPermissionRestoreCommand, MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionActionDto>(unitOfWork, mapper);
