/*
 * 文件名称: MenuPermission.cs
 * 功能描述: 菜单权限实体类，定义菜单权限的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Domain.Shared.Enums;
using SqlSugar;

namespace Domain.Entities;

/// <summary>
/// 菜单权限实体
/// <para>定义菜单权限的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Path：菜单路径</item>
///   <item>Icon：菜单图标</item>
///   <item>Component：组件路径</item>
///   <item>IsVisible：是否可见</item>
///   <item>IsExternal：是否外部链接</item>
///   <item>KeepAlive：是否缓存页面</item>
///   <item>Redirect：重定向地址</item>
///   <item>Meta：元信息（JSON 格式）</item>
///   <item>Parent：父菜单权限</item>
///   <item>Children：子菜单权限列表</item>
///   <item>Buttons：关联的按钮权限列表</item>
///   <item>Apis：关联的 API 权限列表</item>
/// </list>
/// </remarks>
[SugarTable("Permissions", "菜单权限", IsDisabledDelete = true)]
[SugarIndex("IX_MenuPermissions_Path", nameof(Path), OrderByType.Asc, true)]
[SugarIndex("IX_MenuPermissions_IsVisible", nameof(IsVisible), OrderByType.Asc)]
public class MenuPermission : Permission<MenuPermission> {
    /// <summary>
    /// 构造函数
    /// <para>初始化菜单权限实体，设置权限类型为菜单</para>
    /// </summary>
    public MenuPermission() {
        Type = PermissionType.Menu;
    }

    /// <summary>
    /// 菜单路径
    /// </summary>
    /// <value>菜单的访问路径，长度不超过300个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "菜单路径", Length = 300, IsNullable = true)]
    public string? Path { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    /// <value>菜单的图标标识，长度不超过100个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "菜单图标", Length = 100, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <value>菜单对应的组件路径，长度不超过500个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "组件路径", Length = 500, IsNullable = true)]
    public string? Component { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <value>菜单是否在前端显示，默认为 true</value>
    [SugarColumn(ColumnDescription = "是否可见", IsNullable = false, DefaultValue = "1")]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 是否外部链接
    /// </summary>
    /// <value>是否为外部链接，默认为 false</value>
    [SugarColumn(ColumnDescription = "是否外部链接", IsNullable = false, DefaultValue = "0")]
    public bool IsExternal { get; set; } = false;

    /// <summary>
    /// 是否缓存页面
    /// </summary>
    /// <value>是否缓存页面，默认为 true</value>
    [SugarColumn(ColumnDescription = "是否缓存页面", IsNullable = false, DefaultValue = "1")]
    public bool KeepAlive { get; set; } = true;

    /// <summary>
    /// 重定向地址
    /// </summary>
    /// <value>菜单的重定向地址，长度不超过500个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "重定向地址", Length = 500, IsNullable = true)]
    public string? Redirect { get; set; }

    /// <summary>
    /// 元信息（JSON 格式，用于存储菜单的额外配置）
    /// </summary>
    /// <value>菜单的元信息，JSON 格式，可以为空</value>
    [SugarColumn(ColumnDescription = "元信息", ColumnDataType = "json", IsNullable = true)]
    public string? Meta { get; set; }

    /// <summary>
    /// 关联的按钮权限
    /// </summary>
    /// <value>当前菜单关联的按钮权限列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ButtonPermission.MenuId))]
    public List<ButtonPermission> Buttons { get; set; } = [];

    /// <summary>
    /// 关联的 API 权限
    /// </summary>
    /// <value>当前菜单关联的 API 权限列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ApiPermission.MenuId))]
    public List<ApiPermission> Apis { get; set; } = [];
}
