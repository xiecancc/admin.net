/*
 * 文件名称: PermissionBase.cs
 * 功能描述: 权限基类实体，使用鉴别器模式定义权限的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using SqlSugar;
using Domain.Shared.Entities;
using Domain.Shared.Enums;

namespace Domain.Entities;

/// <summary>
/// 权限基类实体（使用鉴别器模式）
/// <para>定义权限的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Code：权限编码，唯一标识</item>
///   <item>Name：权限名称</item>
///   <item>Type：权限类型（鉴别器列）</item>
///   <item>ParentId：父权限ID</item>
///   <item>Sort：排序值</item>
/// </list>
/// <para>关联关系：</para>
/// <list type="bullet">
///   <item>与 Role 多对多关系，通过 RolePermission 中间表</item>
/// </list>
/// </remarks>
[SugarTable("Permissions", "权限表", IsDisabledDelete = true)]
[SugarIndex("IX_Permissions_Code", nameof(Code), OrderByType.Asc, true)]
[SugarIndex("IX_Permissions_Type", nameof(Type), OrderByType.Asc)]
[SugarIndex("IX_Permissions_ParentId", nameof(ParentId), OrderByType.Asc)]
public abstract class PermissionBase : AggregateBase {
    /// <summary>
    /// 权限编码
    /// </summary>
    /// <value>权限的唯一编码，长度不超过100个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "权限编码", Length = 100, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限名称
    /// </summary>
    /// <value>权限的名称，长度不超过100个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "权限名称", Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型（鉴别器列）
    /// </summary>
    /// <value>权限的类型，不能为空</value>
    [SugarColumn(ColumnDescription = "权限类型", IsNullable = false)]
    public PermissionType Type { get; set; }

    /// <summary>
    /// 父权限 ID
    /// </summary>
    /// <value>父权限的ID，可以为空</value>
    [SugarColumn(ColumnDescription = "父权限 ID", IsNullable = true)]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>权限的排序值，默认为0，不能为空</value>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false, DefaultValue = "0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 权限关联的角色（多对多）
    /// </summary>
    /// <value>拥有当前权限的角色列表</value>
    [Navigate(typeof(RolePermission), nameof(RolePermission.PermissionId), nameof(RolePermission.RoleId))]
    public List<Role> Roles { get; set; } = [];
}
