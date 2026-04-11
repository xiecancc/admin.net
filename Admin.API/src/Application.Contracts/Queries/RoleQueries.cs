/*
 * 文件名称: RoleQueries.cs
 * 功能描述: 角色相关查询类，包含角色的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 角色根据ID查询
/// <para>用于根据ID获取角色详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">角色ID</param>
public class RoleByIdQuery(Guid id) : AggregateByIdQuery<RoleDetailDto> {
    /// <summary>
    /// 主键 ID
    /// </summary>
    public new Guid Id { get; set; } = id;
}

/// <summary>
/// 角色列表查询
/// <para>用于获取角色列表</para>
/// </summary>
public class RoleListQuery : AggregateListQuery<RoleListDto> {
    /// <summary>
    /// 角色编码
    /// </summary>
    /// <value>角色编码，用于模糊搜索</value>
    public string? Code { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色名称，用于模糊搜索</value>
    public string? Name { get; set; }

    /// <summary>
    /// 父角色ID
    /// </summary>
    /// <value>父角色ID，用于筛选</value>
    public Guid? ParentId { get; set; }
}

/// <summary>
/// 角色分页查询
/// <para>用于分页获取角色列表</para>
/// </summary>
public class RolePagedQuery : AggregatePagedQuery<RolePagedDto> {
    /// <summary>
    /// 角色编码
    /// </summary>
    /// <value>角色编码，用于模糊搜索</value>
    public string? Code { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色名称，用于模糊搜索</value>
    public string? Name { get; set; }

    /// <summary>
    /// 父角色ID
    /// </summary>
    /// <value>父角色ID，用于筛选</value>
    public Guid? ParentId { get; set; }
}
