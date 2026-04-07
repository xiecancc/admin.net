/*
 * 文件名称: AggregateTreeRepository.cs
 * 功能描述: 树形结构仓储实现，支持具有父子关系的聚合根实体
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-31
 */

using Domain.Shared.Entities;
using Domain.Shared.Events;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IAggregateTreeRepository{TAggregate}"/>
/// <remarks>
/// <para>树形结构仓储负责：</para>
/// <list type="bullet">
///   <item> 提供树形结构的查询和操作 </item>
///   <item> 处理节点之间的父子关系 </item>
///   <item> 检测和避免循环引用 </item>
///   <item> 支持节点的移动和重组 </item>
/// </list>
/// <para>实现策略：使用内存构建树形结构，支持递归查询</para>
/// <para>异常处理：所有操作失败时抛出异常，由调用方处理</para>
/// </remarks>
/// <typeparam name="TAggregate">聚合根类型，必须继承自 AggregateBase 并实现 IAggregateTree 接口</typeparam>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="httpContextProvider">HTTP 上下文提供者，用于获取当前用户信息</param>
/// <param name="entityName">实体名称，用于日志记录</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class AggregateTreeRepository<TAggregate>(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, string entityName, ILogger<AggregateTreeRepository<TAggregate>> logger)
    : AggregateRepository<TAggregate>(client, eventBus, httpContextProvider, entityName, logger), IAggregateTreeRepository<TAggregate>
    where TAggregate : AggregateBase, IAggregateTree<TAggregate>, new() {

    /// <inheritdoc/>
    public virtual async Task<List<TAggregate>> GetInheritanceChainAsync(Guid nodeId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty) {
            _logger.LogWarning("获取继承链失败：节点ID为空");
            return [];
        }

        var nodes = new List<TAggregate>();
        var visitedIds = new HashSet<Guid>();

        try {
            var allNodes = await _client.Queryable<TAggregate>()
                .Where(n => n.Id == nodeId || n.ParentId != null)
                .ToListAsync(cancellationToken);

            var nodeDict = allNodes.ToDictionary(n => n.Id);
            var currentNodeId = nodeId;

            while (currentNodeId != Guid.Empty && !visitedIds.Contains(currentNodeId)) {
                if (!nodeDict.TryGetValue(currentNodeId, out var node))
                    break;

                nodes.Add(node);
                _ = visitedIds.Add(currentNodeId);
                currentNodeId = node.ParentId ?? Guid.Empty;
            }

            _logger.LogInformation("获取继承链成功：节点ID: {NodeId}, 链长度: {Length}", nodeId, nodes.Count);
            return nodes;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取继承链失败：节点ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> HasCircularReferenceAsync(Guid nodeId, Guid parentId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty || parentId == Guid.Empty) {
            _logger.LogWarning("检测循环引用失败：节点ID或父节点ID为空");
            return false;
        }

        if (nodeId == parentId) {
            _logger.LogInformation("检测到循环引用：节点ID与父节点ID相同");
            return true;
        }

        try {
            var allNodes = await _client.Queryable<TAggregate>()
                .Where(n => n.Id == nodeId || n.Id == parentId || n.ParentId != null)
                .ToListAsync(cancellationToken);

            var nodeDict = allNodes.ToDictionary(n => n.Id);
            var visitedIds = new HashSet<Guid> { nodeId };
            var currentParentId = parentId;

            while (currentParentId != Guid.Empty) {
                if (visitedIds.Contains(currentParentId)) {
                    _logger.LogInformation("检测到循环引用：节点ID: {NodeId}, 父节点ID: {ParentId}", nodeId, parentId);
                    return true;
                }

                _ = visitedIds.Add(currentParentId);

                if (!nodeDict.TryGetValue(currentParentId, out var parentNode))
                    break;

                currentParentId = parentNode.ParentId ?? Guid.Empty;
            }

            _logger.LogInformation("未检测到循环引用：节点ID: {NodeId}, 父节点ID: {ParentId}", nodeId, parentId);
            return false;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "检测循环引用失败：节点ID: {NodeId}, 父节点ID: {ParentId}", nodeId, parentId);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<List<TAggregate>> GetAllChildrenAsync(Guid nodeId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty) {
            _logger.LogWarning("获取所有子节点失败：节点ID为空");
            return [];
        }

        try {
            var allNodes = await _client.Queryable<TAggregate>()
                .Where(n => n.ParentId != null)
                .ToListAsync(cancellationToken);

            var childrenDict = allNodes
                .GroupBy(n => n.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var allChildren = new List<TAggregate>();
            var queue = new Queue<Guid>();
            queue.Enqueue(nodeId);

            while (queue.Count > 0) {
                var currentId = queue.Dequeue();

                if (childrenDict.TryGetValue(currentId, out var children)) {
                    allChildren.AddRange(children);
                    foreach (var child in children) {
                        queue.Enqueue(child.Id);
                    }
                }
            }

            _logger.LogInformation("获取所有子节点成功：节点ID: {NodeId}, 子节点数量: {Count}", nodeId, allChildren.Count);
            return allChildren;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取所有子节点失败：节点ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<List<TAggregate>> GetDirectChildrenAsync(Guid nodeId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty) {
            _logger.LogWarning("获取直接子节点失败：节点ID为空");
            return [];
        }

        try {
            var children = await _client.Queryable<TAggregate>()
                .Where(n => n.ParentId == nodeId)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("获取直接子节点成功：节点ID: {NodeId}, 子节点数量: {Count}", nodeId, children.Count);
            return children;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取直接子节点失败：节点ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate?> GetParentAsync(Guid nodeId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty) {
            _logger.LogWarning("获取父节点失败：节点ID为空");
            return null;
        }

        try {
            var result = await _client.Queryable<TAggregate, TAggregate>(
                (child, parent) => new JoinQueryInfos(
                    JoinType.Inner,
                    child.ParentId == parent.Id
                ))
                .Where((child, parent) => child.Id == nodeId)
                .Select((child, parent) => parent)
                .FirstAsync(cancellationToken);

            _logger.LogInformation("获取父节点成功：节点ID: {NodeId}, 父节点ID: {ParentId}", nodeId, result?.Id ?? Guid.Empty);
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取父节点失败：节点ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<List<TAggregate>> GetRootNodesAsync(CancellationToken cancellationToken = default) {
        try {
            var rootNodes = await _client.Queryable<TAggregate>()
                .Where(n => n.ParentId == null || n.ParentId == Guid.Empty)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("获取根节点成功：数量: {Count}", rootNodes.Count);
            return rootNodes;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取根节点失败");
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<List<TAggregate>> GetTreeAsync(CancellationToken cancellationToken = default) {
        try {
            var allNodes = await _client.Queryable<TAggregate>()
                .ToListAsync(cancellationToken);

            var tree = BuildTree(allNodes);
            _logger.LogInformation("获取树形结构成功：根节点数量: {Count}", tree.Count);
            return tree;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取树形结构失败");
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate?> GetSubTreeAsync(Guid nodeId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty) {
            _logger.LogWarning("获取子树结构失败：节点ID为空");
            return null;
        }

        try {
            var allNodes = await _client.Queryable<TAggregate>()
                .Where(n => n.Id == nodeId || n.ParentId != null)
                .ToListAsync(cancellationToken);

            var nodeDict = allNodes.ToDictionary(n => n.Id);

            if (!nodeDict.TryGetValue(nodeId, out var rootNode)) {
                _logger.LogWarning("获取子树结构失败：节点不存在，ID: {NodeId}", nodeId);
                return null;
            }

            var childrenDict = allNodes
                .Where(n => n.ParentId.HasValue)
                .GroupBy(n => n.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            BuildChildrenTree(rootNode, childrenDict);

            _logger.LogInformation("获取子树结构成功：节点ID: {NodeId}", nodeId);
            return rootNode;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取子树结构失败：节点ID: {NodeId}", nodeId);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> MoveNodeAsync(Guid nodeId, Guid? newParentId, CancellationToken cancellationToken = default) {
        if (nodeId == Guid.Empty) {
            _logger.LogWarning("移动节点失败：节点ID为空");
            return false;
        }

        try {
            if (newParentId.HasValue && newParentId.Value != Guid.Empty) {
                if (await HasCircularReferenceAsync(nodeId, newParentId.Value, cancellationToken)) {
                    _logger.LogWarning("移动节点失败：会导致循环引用。节点ID: {NodeId}, 新父节点ID: {ParentId}", nodeId, newParentId);
                    return false;
                }
            }

            var node = await _client.Queryable<TAggregate>()
                .FirstAsync(n => n.Id == nodeId, cancellationToken);

            if (node == null) {
                _logger.LogWarning("移动节点失败：节点不存在。节点ID: {NodeId}", nodeId);
                return false;
            }

            node.ParentId = newParentId;
            node.UpdatedAt = DateTime.UtcNow;

            var result = await _client.Updateable(node)
                .ExecuteCommandAsync(cancellationToken);

            if (result > 0) {
                _logger.LogInformation("移动节点成功：节点ID: {NodeId}, 新父节点ID: {ParentId}", nodeId, newParentId);
                return true;
            }
            else {
                _logger.LogWarning("移动节点失败：更新操作未影响任何记录。节点ID: {NodeId}", nodeId);
                return false;
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "移动节点失败：节点ID: {NodeId}, 新父节点ID: {ParentId}", nodeId, newParentId);
            throw;
        }
    }

    /// <summary>
    /// 构建完整的树形结构
    /// <para>从所有节点中构建完整的树形结构，返回根节点列表</para>
    /// </summary>
    /// <param name="allNodes">所有节点列表</param>
    /// <returns>根节点列表</returns>
    private static List<TAggregate> BuildTree(List<TAggregate> allNodes) {
        var nodeDict = allNodes.ToDictionary(n => n.Id);
        var rootNodes = new List<TAggregate>();

        foreach (var node in allNodes) {
            if (node.ParentId == null || node.ParentId == Guid.Empty) {
                rootNodes.Add(node);
            }
            else if (nodeDict.TryGetValue(node.ParentId.Value, out var parent)) {
                parent.Children.Add(node);
            }
        }

        return rootNodes;
    }

    /// <summary>
    /// 构建子树结构
    /// <para>递归构建指定节点的子树结构</para>
    /// </summary>
    /// <param name="node">根节点</param>
    /// <param name="childrenDict">子节点字典</param>
    private static void BuildChildrenTree(TAggregate node, Dictionary<Guid, List<TAggregate>> childrenDict) {
        if (childrenDict.TryGetValue(node.Id, out var children)) {
            node.Children = children;
            foreach (var child in children) {
                BuildChildrenTree(child, childrenDict);
            }
        }
    }
}
