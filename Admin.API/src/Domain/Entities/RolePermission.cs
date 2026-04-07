/*
 * 文件名称: RolePermission.cs
 * 功能描述: 角色-权限关联实体类，用于存储角色和权限之间的多对多关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Shared.Entities;

namespace Domain.Entities;

/// <summary>
/// 角色 - 权限关联实体
/// <para>用于存储角色和权限之间的多对多关系</para>
/// </summary>
/// <remarks>
/// <para>继承关系：</para>
/// <list type="bullet">
///   <item>继承自 DomainBase，拥有领域事件功能</item>
///   <item>使用联合主键（RoleId, PermissionId）</item>
/// </list>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>RoleId：角色 ID（联合主键）</item>
///   <item>PermissionId：权限 ID（联合主键）</item>
/// </list>
/// </remarks>
[SugarTable("RolePermissions", "角色权限关联表")]
[SugarIndex("IX_RolePermissions_RoleId", nameof(RoleId), OrderByType.Asc)]
[SugarIndex("IX_RolePermissions_PermissionId", nameof(PermissionId), OrderByType.Asc)]
public class RolePermission : DomainBase {
    /// <summary>
    /// 角色 ID（联合主键）
    /// </summary>
    /// <value>角色的唯一标识符，不能为空</value>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "角色 ID", IsNullable = false)]
    public Guid RoleId {
        get; set;
    }

    /// <summary>
    /// 权限 ID（联合主键）
    /// </summary>
    /// <value>权限的唯一标识符，不能为空</value>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "权限 ID", IsNullable = false)]
    public Guid PermissionId {
        get; set;
    }
}
