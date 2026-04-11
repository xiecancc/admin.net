/*
 * 文件名称: RoleCommands.cs
 * 功能描述: 角色相关命令类，包含角色的所有命令操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;

namespace Application.Contracts.Commands;

/// <summary>
/// 角色创建命令
/// <para>用于创建新角色</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="roleCreateDtos">角色创建DTO列表</param>
public class RoleCreateCommand(List<RoleCreateDto> roleCreateDtos) : AggregateCreateCommand<RoleCreateDto>(roleCreateDtos) {
}

/// <summary>
/// 角色更新命令
/// <para>用于更新角色信息</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="roleUpdateDtos">角色更新DTO列表</param>
public class RoleUpdateCommand(List<RoleUpdateDto> roleUpdateDtos) : AggregateUpdateCommand<RoleUpdateDto>(roleUpdateDtos) {
}

/// <summary>
/// 角色删除命令
/// <para>用于删除角色</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="roleIds">角色ID列表</param>
public class RoleDeleteCommand(List<Guid> roleIds) : AggregateDeleteCommand<RoleActionDto>(roleIds.Select(id => new RoleActionDto { Id = id }).ToList()) {
}

/// <summary>
/// 角色恢复命令
/// <para>用于恢复已删除的角色</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="roleIds">角色ID列表</param>
public class RoleRestoreCommand(List<Guid> roleIds) : AggregateRestoreCommand<RoleActionDto>(roleIds.Select(id => new RoleActionDto { Id = id }).ToList()) {
}
