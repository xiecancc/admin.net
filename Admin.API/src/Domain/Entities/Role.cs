/*
 * 文件名称: Role.cs
 * 功能描述: 角色实体类，定义角色的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Shared.Entities;
using Domain.Shared.Enums;

namespace Domain.Entities;

/// <summary>
/// 角色实体
/// <para>定义角色的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Code：角色编码，唯一标识</item>
///   <item>Name：角色名称</item>
///   <item>ParentId：父角色ID</item>
///   <item>Parent：父角色</item>
///   <item>Children：子角色列表</item>
/// </list>
/// <para>关联关系：</para>
/// <list type="bullet">
///   <item>与 User 多对多关系，通过 UserRole 中间表</item>
///   <item>与 Permission 多对多关系，通过 RolePermission 中间表</item>
///   <item>与自身一对多关系，形成树形结构</item>
/// </list>
/// </remarks>
[SugarTable("Roles", "角色表")]
[SugarIndex("IX_Roles_Code", nameof(Code), OrderByType.Asc, true)]
public class Role : AggregateBase, IAggregateTree<Role> {
    /// <summary>
    /// 角色编码
    /// </summary>
    /// <value>角色的唯一编码，长度不超过50个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "角色编码", Length = 50, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <value>角色的名称，长度不超过100个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "角色名称", Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 继承类型
    /// </summary>
    /// <value>角色权限的继承方式，默认为不继承</value>
    [SugarColumn(ColumnDescription = "继承类型", IsNullable = false, DefaultValue = "0")]
    public InheritanceType InheritanceType { get; set; } = InheritanceType.None;

    /// <inheritdoc/>
    /// <summary>
    /// 父角色 ID
    /// </summary>
    /// <value>父角色的ID，可以为空</value>
    [SugarColumn(ColumnDescription = "父角色 ID", IsNullable = true)]
    public Guid? ParentId {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 父角色
    /// </summary>
    /// <value>父角色对象，可以为空</value>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public Role? Parent {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 子角色列表
    /// </summary>
    /// <value>当前角色的子角色列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public List<Role> Children { get; set; } = [];

    /// <summary>
    /// 角色关联的用户（多对多）
    /// </summary>
    /// <value>拥有当前角色的用户列表</value>
    [Navigate(typeof(UserRole), nameof(UserRole.RoleId), nameof(UserRole.UserId))]
    public List<User> Users { get; set; } = [];

    /// <summary>
    /// 角色关联的权限（多对多）
    /// </summary>
    /// <value>当前角色拥有的权限列表</value>
    [Navigate(typeof(RolePermission), nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId))]
    public List<PermissionBase> Permissions { get; set; } = [];
}
