/*
 * 文件名称: ApiPermission.cs
 * 功能描述: API 权限实体类，定义 API 权限的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Domain.Shared.Entities;
using Domain.Shared.Enums;
using SqlSugar;

namespace Domain.Entities;

/// <summary>
/// API 权限实体
/// <para>定义 API 权限的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>HttpMethod：HTTP 方法</item>
///   <item>ApiPath：API 路径</item>
///   <item>ModuleName：模块名称</item>
///   <item>MenuId：关联的菜单权限 ID</item>
///   <item>Menu：关联的菜单权限</item>
///   <item>Parent：父 API 权限</item>
///   <item>Children：子 API 权限列表</item>
/// </list>
/// </remarks>
[SugarTable("Permissions", "API 权限", IsDisabledDelete = true)]
[SugarIndex("IX_ApiPermissions_ApiPath", nameof(ApiPath), OrderByType.Asc)]
[SugarIndex("IX_ApiPermissions_MenuId", nameof(MenuId), OrderByType.Asc)]
[SugarIndex("IX_ApiPermissions_HttpMethod", nameof(HttpMethod), OrderByType.Asc)]
[SugarIndex("IX_ApiPermissions_ApiPath_HttpMethod", nameof(ApiPath), OrderByType.Asc, nameof(HttpMethod), OrderByType.Asc, true)]
public class ApiPermission : Permission, IAggregateTree<ApiPermission> {
    /// <summary>
    /// 构造函数
    /// <para>初始化 API 权限实体，设置权限类型为 API</para>
    /// </summary>
    public ApiPermission() {
        Type = PermissionType.Api;
    }

    /// <summary>
    /// HTTP 方法
    /// </summary>
    /// <value>HTTP 请求方法，如 GET、POST、PUT、DELETE 等，长度不超过10个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "HTTP 方法", Length = 10, IsNullable = true)]
    public string? HttpMethod {
        get; set;
    }

    /// <summary>
    /// API 路径
    /// </summary>
    /// <value>API 的访问路径，长度不超过300个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "API 路径", Length = 300, IsNullable = false, DefaultValue = "")]
    public string ApiPath { get; set; } = string.Empty;

    /// <summary>
    /// 模块名称
    /// </summary>
    /// <value>API 所属的模块名称，长度不超过100个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "模块名称", Length = 100, IsNullable = true)]
    public string? ModuleName {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限 ID
    /// </summary>
    /// <value>关联的菜单权限的 ID，可以为空</value>
    [SugarColumn(ColumnDescription = "关联的菜单权限 ID", IsNullable = true)]
    public Guid? MenuId {
        get; set;
    }

    /// <summary>
    /// 关联的菜单权限
    /// </summary>
    /// <value>关联的菜单权限对象，可以为空</value>
    [Navigate(NavigateType.OneToOne, nameof(MenuId))]
    public MenuPermission? Menu {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 父 API 权限
    /// </summary>
    /// <value>父 API 权限对象，可以为空</value>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public new ApiPermission? Parent {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 子 API 权限列表
    /// </summary>
    /// <value>当前 API 权限的子权限列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public new List<ApiPermission> Children { get; set; } = [];
}
