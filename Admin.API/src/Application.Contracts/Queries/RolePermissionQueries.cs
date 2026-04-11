/*
 * 文件名称: RolePermissionQueries.cs
 * 功能描述: 角色权限关联查询类，包含角色权限查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Contracts.Queries;

/// <summary>
/// 角色权限分页查询
/// <para>用于角色权限关联关系的分页查询</para>
/// </summary>
public class RolePermissionPagedQuery : DomainPagedQuery<RolePermissionPagedDto> {
    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// 权限 ID
    /// </summary>
    public Guid? PermissionId { get; set; }
}

/// <summary>
/// 角色权限列表查询
/// <para>用于角色权限关联关系的列表查询</para>
/// </summary>
public class RolePermissionListQuery : DomainListQuery<RolePermissionListDto> {
    /// <summary>
    /// 角色 ID
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// 权限 ID
    /// </summary>
    public Guid? PermissionId { get; set; }
}

/// <summary>
/// 角色权限列表查询
/// <para>获取指定角色的所有权限</para>
/// </summary>
public class RolePermissionsQuery : IRequest<List<Permission>> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleId">角色ID</param>
    public RolePermissionsQuery(Guid roleId) {
        RoleId = roleId;
    }
}

/// <summary>
/// 角色权限ID列表查询
/// <para>获取指定角色的权限ID列表</para>
/// </summary>
public class RolePermissionIdsQuery : IRequest<List<Guid>> {
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleId">角色ID</param>
    public RolePermissionIdsQuery(Guid roleId) {
        RoleId = roleId;
    }
}
