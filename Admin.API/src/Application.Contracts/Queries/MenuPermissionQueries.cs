/*
 * 文件名称: MenuPermissionQueries.cs
 * 功能描述: 菜单权限相关查询类，包含菜单权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 菜单权限根据ID查询
/// <para>用于根据ID获取菜单权限详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">菜单权限ID</param>
public class MenuPermissionByIdQuery(Guid id) : AggregateByIdQuery<MenuPermissionDetailDto> {
    /// <summary>
    /// 主键 ID
    /// </summary>
    public new Guid Id { get; set; } = id;
}

/// <summary>
/// 菜单权限列表查询
/// <para>用于获取菜单权限列表</para>
/// </summary>
public class MenuPermissionListQuery : AggregateListQuery<MenuPermissionListDto> {
    /// <summary>
    /// 权限编码
    /// </summary>
    /// <value>权限编码，用于模糊搜索</value>
    public string? Code { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限名称，用于模糊搜索</value>
    public string? Name { get; set; }

    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单路径，用于模糊搜索</value>
    public string? Path { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>是否可见，用于筛选</value>
    public bool? IsVisible { get; set; }

    /// <summary>
    /// 父权限ID
    /// </summary>
    /// <value>父权限ID，用于筛选</value>
    public Guid? ParentId { get; set; }
}

/// <summary>
/// 菜单权限分页查询
/// <para>用于分页获取菜单权限列表</para>
/// </summary>
public class MenuPermissionPagedQuery : AggregatePagedQuery<MenuPermissionPagedDto> {
    /// <summary>
    /// 权限编码
    /// </summary>
    /// <value>权限编码，用于模糊搜索</value>
    public string? Code { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限名称，用于模糊搜索</value>
    public string? Name { get; set; }

    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单路径，用于模糊搜索</value>
    public string? Path { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>是否可见，用于筛选</value>
    public bool? IsVisible { get; set; }

    /// <summary>
    /// 父权限ID
    /// </summary>
    /// <value>父权限ID，用于筛选</value>
    public Guid? ParentId { get; set; }
}
