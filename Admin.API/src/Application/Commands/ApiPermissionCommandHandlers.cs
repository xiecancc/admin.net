/*
 * 文件名称: ApiPermissionCommandHandlers.cs
 * 功能描述: API权限命令处理器，处理API权限相关的命令
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
/// API权限创建命令处理器
/// 处理API权限的创建操作
/// </summary>
public class ApiPermissionCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : DomainCreateCommandHandler<ApiPermissionCreateCommand, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionCreateDto>(unitOfWork, mapper);

/// <summary>
/// API权限更新命令处理器
/// 处理API权限的更新操作
/// </summary>
public class ApiPermissionUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateUpdateCommandHandler<ApiPermissionUpdateCommand, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionUpdateDto>(unitOfWork, mapper);

/// <summary>
/// API权限删除命令处理器
/// 处理API权限的删除操作
/// </summary>
public class ApiPermissionDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateDeleteCommandHandler<ApiPermissionDeleteCommand, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionActionDto>(unitOfWork, mapper);

/// <summary>
/// API权限恢复命令处理器
/// 处理API权限的恢复操作
/// </summary>
public class ApiPermissionRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateRestoreCommandHandler<ApiPermissionRestoreCommand, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionActionDto>(unitOfWork, mapper);
