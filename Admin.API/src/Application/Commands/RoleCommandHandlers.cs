/*
 * 文件名称: RoleCommandHandlers.cs
 * 功能描述: 角色命令处理器，处理角色相关的命令
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
/// 角色创建命令处理器
/// 处理角色的创建操作
/// </summary>
public class RoleCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : DomainCreateCommandHandler<RoleCreateCommand, Role, IRoleRepository, RoleCreateDto>(unitOfWork, mapper);

/// <summary>
/// 角色更新命令处理器
/// 处理角色的更新操作
/// </summary>
public class RoleUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateUpdateCommandHandler<RoleUpdateCommand, Role, IRoleRepository, RoleUpdateDto>(unitOfWork, mapper);

/// <summary>
/// 角色删除命令处理器
/// 处理角色的删除操作
/// </summary>
public class RoleDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateDeleteCommandHandler<RoleDeleteCommand, Role, IRoleRepository, RoleActionDto>(unitOfWork, mapper);

/// <summary>
/// 角色恢复命令处理器
/// 处理角色的恢复操作
/// </summary>
public class RoleRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateRestoreCommandHandler<RoleRestoreCommand, Role, IRoleRepository, RoleActionDto>(unitOfWork, mapper);
