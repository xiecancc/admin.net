/*
 * 文件名称: ButtonPermissionCommands.cs
 * 功能描述: 按钮权限相关命令类，包含按钮权限的所有命令操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;

namespace Application.Contracts.Commands;

/// <summary>
/// 按钮权限创建命令
/// <para>用于创建新按钮权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="buttonPermissionCreateDtos">按钮权限创建DTO列表</param>
public class ButtonPermissionCreateCommand(List<ButtonPermissionCreateDto> buttonPermissionCreateDtos) : AggregateCreateCommand<ButtonPermissionCreateDto>(buttonPermissionCreateDtos) {
}

/// <summary>
/// 按钮权限更新命令
/// <para>用于更新按钮权限信息</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="buttonPermissionUpdateDtos">按钮权限更新DTO列表</param>
public class ButtonPermissionUpdateCommand(List<ButtonPermissionUpdateDto> buttonPermissionUpdateDtos) : AggregateUpdateCommand<ButtonPermissionUpdateDto>(buttonPermissionUpdateDtos) {
}

/// <summary>
/// 按钮权限删除命令
/// <para>用于删除按钮权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="buttonPermissionIds">按钮权限ID列表</param>
public class ButtonPermissionDeleteCommand(List<Guid> buttonPermissionIds) : AggregateDeleteCommand<ButtonPermissionActionDto>(buttonPermissionIds.Select(id => new ButtonPermissionActionDto { Id = id }).ToList()) {
}

/// <summary>
/// 按钮权限恢复命令
/// <para>用于恢复已删除的按钮权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="buttonPermissionIds">按钮权限ID列表</param>
public class ButtonPermissionRestoreCommand(List<Guid> buttonPermissionIds) : AggregateRestoreCommand<ButtonPermissionActionDto>(buttonPermissionIds.Select(id => new ButtonPermissionActionDto { Id = id }).ToList()) {
}
