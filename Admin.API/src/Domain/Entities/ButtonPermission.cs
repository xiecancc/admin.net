/*
 * 文件名称: ButtonPermission.cs
 * 功能描述: 按钮权限实体类，定义按钮权限的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Domain.Shared.Entities;
using Domain.Shared.Enums;
using SqlSugar;

namespace Domain.Entities;

/// <summary>
/// 按钮权限实体
/// <para>定义按钮权限的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>MenuId：关联的菜单权限 ID</item>
///   <item>Menu：关联的菜单权限</item>
///   <item>ActionType：操作类型</item>
///   <item>Parent：父按钮权限</item>
///   <item>Children：子按钮权限列表</item>
/// </list>
/// </remarks>
[SugarTable("Permissions", "按钮权限", IsDisabledDelete = true)]
[SugarIndex("IX_ButtonPermissions_MenuId", nameof(MenuId), OrderByType.Asc)]
[SugarIndex("IX_ButtonPermissions_ActionType", nameof(ActionType), OrderByType.Asc)]
[SugarIndex("IX_ButtonPermissions_MenuId_ActionType", nameof(MenuId), OrderByType.Asc, nameof(ActionType), OrderByType.Asc, true)]
public class ButtonPermission : Permission, IAggregateTree<ButtonPermission> {
    /// <summary>
    /// 构造函数
    /// <para>初始化按钮权限实体，设置权限类型为按钮</para>
    /// </summary>
    public ButtonPermission() {
        Type = PermissionType.Button;
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

    /// <summary>
    /// 操作类型
    /// </summary>
    /// <value>按钮的操作类型，长度不超过50个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "操作类型", Length = 50, IsNullable = false, DefaultValue = "")]
    public string ActionType { get; set; } = string.Empty;

    /// <inheritdoc/>
    /// <summary>
    /// 父按钮权限
    /// </summary>
    /// <value>父按钮权限对象，可以为空</value>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public new ButtonPermission? Parent {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 子按钮权限列表
    /// </summary>
    /// <value>当前按钮权限的子权限列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public new List<ButtonPermission> Children { get; set; } = [];
}
