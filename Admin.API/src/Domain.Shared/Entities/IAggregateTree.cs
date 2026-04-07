/*
 * 文件名称: IAggregateTree.cs
 * 功能描述: 树形结构聚合根接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

namespace Domain.Shared.Entities;

/// <summary>
/// 树形结构聚合根接口
/// <para>用于定义树形结构的聚合根实体</para>
/// </summary>
/// <typeparam name="TAggregate">聚合根类型，必须继承自 AggregateBase</typeparam>
/// <remarks>
/// <para>适用场景：</para>
/// <list type="bullet">
///   <item>组织架构</item>
///   <item>菜单权限</item>
///   <item>分类目录</item>
/// </list>
/// </remarks>
public interface IAggregateTree<TAggregate> where TAggregate : DomainBase {
    /// <summary>
    /// 父节点 ID
    /// </summary>
    Guid? ParentId {
        get; set;
    }

    /// <summary>
    /// 父节点
    /// </summary>
    TAggregate? Parent {
        get; set;
    }

    /// <summary>
    /// 子节点列表
    /// </summary>
    List<TAggregate> Children {
        get; set;
    }
}
