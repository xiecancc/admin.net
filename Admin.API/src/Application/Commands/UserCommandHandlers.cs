/*
 * 文件名称: UserCommandHandlers.cs
 * 功能描述: 用户命令处理器，处理用户相关的命令
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
/// 用户创建命令处理器
/// 处理用户的创建操作
/// </summary>
public class UserCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : DomainCreateCommandHandler<UserCreateCommand, User, IUserRepository, UserCreateDto>(unitOfWork, mapper);

/// <summary>
/// 用户更新命令处理器
/// 处理用户的更新操作
/// </summary>
public class UserUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateUpdateCommandHandler<UserUpdateCommand, User, IUserRepository, UserUpdateDto>(unitOfWork, mapper);

/// <summary>
/// 用户删除命令处理器
/// 处理用户的删除操作
/// </summary>
public class UserDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateDeleteCommandHandler<UserDeleteCommand, User, IUserRepository, UserActionDto>(unitOfWork, mapper);

/// <summary>
/// 用户恢复命令处理器
/// 处理用户的恢复操作
/// </summary>
public class UserRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : AggregateRestoreCommandHandler<UserRestoreCommand, User, IUserRepository, UserActionDto>(unitOfWork, mapper);
