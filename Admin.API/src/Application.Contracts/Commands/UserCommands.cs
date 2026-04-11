/*
 * 文件名称: UserCommands.cs
 * 功能描述: 用户相关命令类，包含用户的所有命令操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;

namespace Application.Contracts.Commands;

/// <summary>
/// 用户创建命令
/// <para>用于创建新用户</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="userCreateDtos">用户创建DTO列表</param>
public class UserCreateCommand(List<UserCreateDto> userCreateDtos) : AggregateCreateCommand<UserCreateDto>(userCreateDtos) {
}

/// <summary>
/// 用户更新命令
/// <para>用于更新用户信息</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="userUpdateDtos">用户更新DTO列表</param>
public class UserUpdateCommand(List<UserUpdateDto> userUpdateDtos) : AggregateUpdateCommand<UserUpdateDto>(userUpdateDtos) {
}

/// <summary>
/// 用户删除命令
/// <para>用于删除用户</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="userIds">用户ID列表</param>
public class UserDeleteCommand(List<Guid> userIds) : AggregateDeleteCommand<UserActionDto>(userIds.Select(id => new UserActionDto { Id = id }).ToList()) {
}

/// <summary>
/// 用户恢复命令
/// <para>用于恢复已删除的用户</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="userIds">用户ID列表</param>
public class UserRestoreCommand(List<Guid> userIds) : AggregateRestoreCommand<UserActionDto>(userIds.Select(id => new UserActionDto { Id = id }).ToList()) {
}


