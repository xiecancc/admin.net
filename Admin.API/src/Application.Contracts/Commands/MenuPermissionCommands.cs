/*
 * 文件名称: MenuPermissionCommands.cs
 * 功能描述: 菜单权限相关命令类，包含菜单权限的所有命令操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Contracts.Commands;

/// <summary>
/// 菜单权限创建命令
/// <para>用于创建新菜单权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="menuPermissionCreateDtos">菜单权限创建DTO列表</param>
public class MenuPermissionCreateCommand(List<MenuPermissionCreateDto> menuPermissionCreateDtos) : DomainCreateCommands<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionCreateDto>(menuPermissionCreateDtos) {
}

/// <summary>
/// 菜单权限更新命令
/// <para>用于更新菜单权限信息</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="menuPermissionUpdateDtos">菜单权限更新DTO列表</param>
public class MenuPermissionUpdateCommand(List<MenuPermissionUpdateDto> menuPermissionUpdateDtos) : AggregateUpdateCommand<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionUpdateDto>(menuPermissionUpdateDtos) {
}

/// <summary>
/// 菜单权限删除命令
/// <para>用于删除菜单权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="menuPermissionIds">菜单权限ID列表</param>
public class MenuPermissionDeleteCommand(List<Guid> menuPermissionIds) : AggregateDeleteCommand<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionActionDto>(menuPermissionIds.Select(id => new MenuPermissionActionDto { Id = id }).ToList()) {
}

/// <summary>
/// 菜单权限恢复命令
/// <para>用于恢复已删除的菜单权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="menuPermissionIds">菜单权限ID列表</param>
public class MenuPermissionRestoreCommand(List<Guid> menuPermissionIds) : AggregateRestoreCommand<MenuPermission, IPermissionRepository<MenuPermission>, MenuPermissionActionDto>(menuPermissionIds.Select(id => new MenuPermissionActionDto { Id = id }).ToList()) {
}
