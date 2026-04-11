/*
 * 文件名称: ApiPermissionQueries.cs
 * 功能描述: API权限相关查询类，包含API权限的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// API权限根据ID查询
/// <para>用于根据ID获取API权限详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">API权限ID</param>
public class ApiPermissionByIdQuery(Guid id) : AggregateByIdQuery<ApiPermissionDetailDto> {
    /// <summary>
    /// 主键 ID
    /// </summary>
    public new Guid Id { get; set; } = id;
}

/// <summary>
/// API权限列表查询
/// <para>用于获取API权限列表</para>
/// </summary>
public class ApiPermissionListQuery : AggregateListQuery<ApiPermissionListDto> {
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
    /// API 路径
    /// </summary>
    /// <value>API 路径，用于模糊搜索</value>
    public string? ApiPath { get; set; }

    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 方法，用于精确匹配</value>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>模块名称，用于模糊搜索</value>
    public string? ModuleName { get; set; }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限 ID，用于精确匹配</value>
    public Guid? MenuId { get; set; }
}

/// <summary>
/// API权限分页查询
/// <para>用于分页获取API权限列表</para>
/// </summary>
public class ApiPermissionPagedQuery : AggregatePagedQuery<ApiPermissionPagedDto> {
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
    /// API 路径
    /// </summary>
    /// <value>API 路径，用于模糊搜索</value>
    public string? ApiPath { get; set; }

    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 方法，用于精确匹配</value>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>模块名称，用于模糊搜索</value>
    public string? ModuleName { get; set; }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限 ID，用于精确匹配</value>
    public Guid? MenuId { get; set; }
}
