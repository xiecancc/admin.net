/*
 * 文件名称: UserDepartmentRole.cs
 * 功能描述: 用户部门角色关联实体类，定义用户、部门和角色的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using SqlSugar;
using Domain.Shared.Entities;

namespace Domain.Entities;

/// <summary>
/// 用户部门角色关联实体
/// <para>定义用户、部门和角色的多对多关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>UserId：用户ID</item>
///   <item>DepartmentId：部门ID</item>
///   <item>RoleId：角色ID</item>
/// </list>
/// <para>关联关系：</para>
/// <list type="bullet">
///   <item>与 User 一对一关系</item>
///   <item>与 Department 一对一关系</item>
///   <item>与 Role 一对一关系</item>
/// </list>
/// </remarks>
[SugarTable("UserDepartmentRoles", "用户部门角色关联表")]
[SugarIndex("IX_UserDepartmentRoles_UserId", nameof(UserId), OrderByType.Asc)]
[SugarIndex("IX_UserDepartmentRoles_DepartmentId", nameof(DepartmentId), OrderByType.Asc)]
[SugarIndex("IX_UserDepartmentRoles_RoleId", nameof(RoleId), OrderByType.Asc)]
public class UserDepartmentRole : DomainBase {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>用户的ID，不能为空</value>
    [SugarColumn(ColumnDescription = "用户 ID", IsPrimaryKey = true, IsNullable = false)]
    public Guid UserId { get; set; }

    /// <summary>
    /// 部门 ID
    /// </summary>
    /// <value>部门的ID，不能为空</value>
    [SugarColumn(ColumnDescription = "部门 ID", IsPrimaryKey = true, IsNullable = false)]
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// 角色 ID
    /// </summary>
    /// <value>角色的ID，不能为空</value>
    [SugarColumn(ColumnDescription = "角色 ID", IsPrimaryKey = true, IsNullable = false)]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    /// <value>关联的用户对象</value>
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User? User { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    /// <value>关联的部门对象</value>
    [Navigate(NavigateType.OneToOne, nameof(DepartmentId))]
    public Department? Department { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    /// <value>关联的角色对象</value>
    [Navigate(NavigateType.OneToOne, nameof(RoleId))]
    public Role? Role { get; set; }
}