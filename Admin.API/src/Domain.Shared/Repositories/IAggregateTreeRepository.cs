/*
 * 文件名称: IAggregateTreeRepository.cs
 * 功能描述: 树形结构仓储接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-31
 */

using Domain.Shared.Entities;

namespace Domain.Shared.Repositories;

/// <summary>
/// 树形结构仓储接口
/// <para>继承自 IAggregateRepository，添加树形结构特有的操作</para>
/// <para>适用于具有父子关系的聚合根实体</para>
/// </summary>
/// <remarks>
/// <para>树形结构仓储负责：</para>
/// <list type="bullet">
///   <item> 提供树形结构的查询和操作 </item>
///   <item> 处理节点之间的父子关系 </item>
///   <item> 检测和避免循环引用 </item>
///   <item> 支持节点的移动和重组 </item>
/// </list>
/// </remarks>
/// <typeparam name="TAggregate">聚合根类型，必须继承自 AggregateBase 并实现 IAggregateTree 接口</typeparam>
public interface IAggregateTreeRepository<TAggregate> : IAggregateRepository<TAggregate>
    where TAggregate : AggregateBase, IAggregateTree<TAggregate>, new() {
    /// <summary>
    /// 获取节点及其完整的继承链
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点列表（包含当前节点及其所有父节点）</returns>
    /// <exception cref="ArgumentException">当 nodeId 为空时抛出</exception>
    Task<List<TAggregate>> GetInheritanceChainAsync(Guid nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检测是否存在循环引用
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="parentId">要设置的父节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在循环引用</returns>
    /// <exception cref="ArgumentException">当 nodeId 或 parentId 为空时抛出</exception>
    Task<bool> HasCircularReferenceAsync(Guid nodeId, Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取节点的所有子节点（递归）
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>所有子节点列表</returns>
    /// <exception cref="ArgumentException">当 nodeId 为空时抛出</exception>
    Task<List<TAggregate>> GetAllChildrenAsync(Guid nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取节点的直接子节点
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>直接子节点列表</returns>
    /// <exception cref="ArgumentException">当 nodeId 为空时抛出</exception>
    Task<List<TAggregate>> GetDirectChildrenAsync(Guid nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取节点的直接父节点
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>父节点，不存在则返回 null</returns>
    /// <exception cref="ArgumentException">当 nodeId 为空时抛出</exception>
    Task<TAggregate?> GetParentAsync(Guid nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取树的根节点列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>根节点列表</returns>
    Task<List<TAggregate>> GetRootNodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取完整的树形结构
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>树形结构的根节点列表</returns>
    Task<List<TAggregate>> GetTreeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取以指定节点为根的子树结构
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>子树的根节点（包含子节点）</returns>
    /// <exception cref="ArgumentException">当 nodeId 为空时抛出</exception>
    Task<TAggregate?> GetSubTreeAsync(Guid nodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移动节点到新的父节点下
    /// </summary>
    /// <param name="nodeId">要移动的节点ID</param>
    /// <param name="newParentId">新的父节点ID（null表示移动到根级别）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentException">当 nodeId 为空时抛出</exception>
    Task<bool> MoveNodeAsync(Guid nodeId, Guid? newParentId, CancellationToken cancellationToken = default);
}
