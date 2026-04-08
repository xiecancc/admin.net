/*
 * 文件名称: Department.cs
 * 功能描述: 部门实体类，定义部门的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using SqlSugar;
using Domain.Shared.Entities;

namespace Domain.Entities;

/// <summary>
/// 部门实体
/// <para>定义部门的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Code：部门编码，唯一标识</item>
///   <item>Name：部门名称</item>
///   <item>ParentId：父部门ID</item>
///   <item>Parent：父部门</item>
///   <item>Children：子部门列表</item>
/// </list>
/// <para>关联关系：</para>
/// <list type="bullet">
///   <item>与 User 多对多关系，通过 UserDepartmentRole 中间表</item>
///   <item>与自身一对多关系，形成树形结构</item>
/// </list>
/// </remarks>
[SugarTable("Departments", "部门表")]
[SugarIndex("IX_Departments_Code", nameof(Code), OrderByType.Asc, true)]
[SugarIndex("IX_Departments_ParentId", nameof(ParentId), OrderByType.Asc)]
public class Department : AggregateBase, IAggregateTree<Department> {
    /// <summary>
    /// 部门编码
    /// </summary>
    /// <value>部门的唯一编码，长度不超过50个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "部门编码", Length = 50, IsNullable = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <value>部门的名称，长度不超过100个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "部门名称", Length = 100, IsNullable = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <value>部门的排序值，默认为0</value>
    [SugarColumn(ColumnDescription = "排序", IsNullable = false, DefaultValue = "0")]
    public int Sort { get; set; } = 0;

    /// <inheritdoc/>
    /// <summary>
    /// 父部门 ID
    /// </summary>
    /// <value>父部门的ID，可以为空</value>
    [SugarColumn(ColumnDescription = "父部门 ID", IsNullable = true)]
    public Guid? ParentId {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 父部门
    /// </summary>
    /// <value>父部门对象，可以为空</value>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public Department? Parent {
        get; set;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 子部门列表
    /// </summary>
    /// <value>当前部门的子部门列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public List<Department> Children { get; set; } = [];

    /// <summary>
    /// 部门关联的用户角色（多对多）
    /// </summary>
    /// <value>部门下的用户角色列表</value>
    [Navigate(typeof(UserDepartmentRole), nameof(UserDepartmentRole.DepartmentId), nameof(UserDepartmentRole.UserId))]
    public List<User> Users { get; set; } = [];
}