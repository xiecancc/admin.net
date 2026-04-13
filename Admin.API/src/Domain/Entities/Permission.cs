/*
 * 文件名称: Permission.cs
 * 功能描述: 权限泛型基类，使用自引用泛型模式实现类型安全的树形结构
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using SqlSugar;
using Domain.Shared.Entities;

namespace Domain.Entities;

/// <summary>
/// 权限泛型基类（自引用泛型模式）
/// <para>实现类型安全的树形结构，避免子类使用 new 关键字隐藏基类属性</para>
/// </summary>
/// <typeparam name="TPermission">权限实体类型，必须继承自 Permission{TPermission}</typeparam>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Parent：父权限（类型安全）</item>
///   <item>Children：子权限列表（类型安全）</item>
/// </list>
/// <para>继承关系：</para>
/// <list type="bullet">
///   <item>ApiPermission : Permission&lt;ApiPermission&gt;</item>
///   <item>MenuPermission : Permission&lt;MenuPermission&gt;</item>
///   <item>ButtonPermission : Permission&lt;ButtonPermission&gt;</item>
/// </list>
/// </remarks>
public abstract class Permission<TPermission> : PermissionBase, IAggregateTree<TPermission>
    where TPermission : Permission<TPermission>, new() {

    /// <inheritdoc/>
    /// <summary>
    /// 父权限
    /// </summary>
    /// <value>父权限对象，可以为空</value>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public TPermission? Parent { get; set; }

    /// <inheritdoc/>
    /// <summary>
    /// 子权限列表
    /// </summary>
    /// <value>当前权限的子权限列表</value>
    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public List<TPermission> Children { get; set; } = [];
}
