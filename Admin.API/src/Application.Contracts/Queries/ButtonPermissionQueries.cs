/*
 * 文件名称: ButtonPermissionQueries.cs
 * 功能描述: 按钮权限相关查询类，包含按钮权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 按钮权限根据ID查询
/// <para>用于根据ID获取按钮权限详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">按钮权限ID</param>
public class ButtonPermissionByIdQuery(Guid id) : AggregateByIdQuery<ButtonPermissionDetailDto> {
    /// <summary>
    /// 主键 ID
    /// </summary>
    public new Guid Id { get; set; } = id;
}

/// <summary>
/// 按钮权限列表查询
/// <para>用于获取按钮权限列表</para>
/// </summary>
public class ButtonPermissionListQuery : AggregateListQuery<ButtonPermissionListDto> {
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
    /// 操作类型
    /// </summary>
    /// <value>操作类型，用于模糊搜索</value>
    public string? ActionType { get; set; }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限 ID，用于精确匹配</value>
    public Guid? MenuId { get; set; }
}

/// <summary>
/// 按钮权限分页查询
/// <para>用于分页获取按钮权限列表</para>
/// </summary>
public class ButtonPermissionPagedQuery : AggregatePagedQuery<ButtonPermissionPagedDto> {
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
    /// 操作类型
    /// </summary>
    /// <value>操作类型，用于模糊搜索</value>
    public string? ActionType { get; set; }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限 ID，用于精确匹配</value>
    public Guid? MenuId { get; set; }
}
