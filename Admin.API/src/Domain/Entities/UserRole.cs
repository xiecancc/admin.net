/*
 * 文件名称: UserRole.cs
 * 功能描述: 用户-角色关联实体类，用于存储用户和角色之间的多对多关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Shared.Entities;

namespace Domain.Entities;

/// <summary>
/// 用户 - 角色关联实体
/// <para>用于存储用户和角色之间的多对多关系</para>
/// </summary>
/// <remarks>
/// <para>继承关系：</para>
/// <list type="bullet">
///   <item>继承自 DomainBase，拥有领域事件功能</item>
///   <item>使用联合主键（UserId, RoleId）</item>
/// </list>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>UserId：用户 ID（联合主键）</item>
///   <item>RoleId：角色 ID（联合主键）</item>
/// </list>
/// </remarks>
[SugarTable("UserRoles", "用户角色关联表")]
[SugarIndex("IX_UserRoles_UserId", nameof(UserId), OrderByType.Asc)]
[SugarIndex("IX_UserRoles_RoleId", nameof(RoleId), OrderByType.Asc)]
public class UserRole : DomainBase {
    /// <summary>
    /// 用户 ID（联合主键）
    /// </summary>
    /// <value>用户的唯一标识符，不能为空</value>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "用户 ID", IsNullable = false)]
    public Guid UserId {
        get; set;
    }

    /// <summary>
    /// 角色 ID（联合主键）
    /// </summary>
    /// <value>角色的唯一标识符，不能为空</value>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "角色 ID", IsNullable = false)]
    public Guid RoleId {
        get; set;
    }
}
