/*
 * 文件名称: ApiPermissionCommands.cs
 * 功能描述: API权限相关命令类，包含API权限的所有命令操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;

namespace Application.Contracts.Commands;

/// <summary>
/// API权限创建命令
/// <para>用于创建新API权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="apiPermissionCreateDtos">API权限创建DTO列表</param>
public class ApiPermissionCreateCommand(List<ApiPermissionCreateDto> apiPermissionCreateDtos) : AggregateCreateCommand<ApiPermissionCreateDto>(apiPermissionCreateDtos) {
}

/// <summary>
/// API权限更新命令
/// <para>用于更新API权限信息</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="apiPermissionUpdateDtos">API权限更新DTO列表</param>
public class ApiPermissionUpdateCommand(List<ApiPermissionUpdateDto> apiPermissionUpdateDtos) : AggregateUpdateCommand<ApiPermissionUpdateDto>(apiPermissionUpdateDtos) {
}

/// <summary>
/// API权限删除命令
/// <para>用于删除API权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="apiPermissionIds">API权限ID列表</param>
public class ApiPermissionDeleteCommand(List<Guid> apiPermissionIds) : AggregateDeleteCommand<ApiPermissionActionDto>(apiPermissionIds.Select(id => new ApiPermissionActionDto { Id = id }).ToList()) {
}

/// <summary>
/// API权限恢复命令
/// <para>用于恢复已删除的API权限</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="apiPermissionIds">API权限ID列表</param>
public class ApiPermissionRestoreCommand(List<Guid> apiPermissionIds) : AggregateRestoreCommand<ApiPermissionActionDto>(apiPermissionIds.Select(id => new ApiPermissionActionDto { Id = id }).ToList()) {
}
