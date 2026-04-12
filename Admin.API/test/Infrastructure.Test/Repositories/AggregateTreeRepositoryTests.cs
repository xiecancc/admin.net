/*
 * 文件名称: AggregateTreeRepositoryTests.cs
 * 功能描述: 树形结构仓储测试类，测试角色继承计算和循环继承检测功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Shared.Entities;
using Domain.Shared.Events;
using Infrastructure.Repositories;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using Moq;
using SqlSugar;

namespace Infrastructure.Test.Repositories;

/// <summary>
/// 树形结构仓储测试类
/// <para>测试角色继承计算和循环继承检测功能</para>
/// </summary>
public class AggregateTreeRepositoryTests {
    private readonly Mock<ISqlSugarClient> _mockClient;
    private readonly Mock<IDomainEventBus> _mockEventBus;
    private readonly Mock<IUserContextProvider> _mockUserContextProvider;
    private readonly Mock<ILogger<AggregateTreeRepository<Role>>> _mockLogger;
    private readonly AggregateTreeRepository<Role> _repository;

    public AggregateTreeRepositoryTests() {
        _mockClient = new Mock<ISqlSugarClient>();
        _mockEventBus = new Mock<IDomainEventBus>();
        _mockUserContextProvider = new Mock<IUserContextProvider>();
        _mockLogger = new Mock<ILogger<AggregateTreeRepository<Role>>>();

        _repository = new AggregateTreeRepository<Role>(
            _mockClient.Object,
            _mockEventBus.Object,
            _mockUserContextProvider.Object,
            "角色",
            _mockLogger.Object
        );
    }

    #region GetInheritanceChainAsync Tests

    /// <summary>
    /// 测试获取继承链 - 正常三层继承结构
    /// <para>场景：GrandParent -> Parent -> Child</para>
    /// <para>预期：返回包含三个节点的继承链</para>
    /// </summary>
    [Fact]
    public async Task GetInheritanceChainAsync_WithThreeLevelHierarchy_ShouldReturnThreeNodes() {
        var grandParentId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = childId, ParentId = parentId, Code = "CHILD", Name = "子角色" },
            new() { Id = parentId, ParentId = grandParentId, Code = "PARENT", Name = "父角色" },
            new() { Id = grandParentId, ParentId = null, Code = "GRANDPARENT", Name = "祖父角色" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetInheritanceChainAsync(childId);

        Assert.Equal(3, result.Count);
        Assert.Equal(childId, result[0].Id);
        Assert.Equal(parentId, result[1].Id);
        Assert.Equal(grandParentId, result[2].Id);
    }

    /// <summary>
    /// 测试获取继承链 - 根节点无父节点
    /// <para>场景：节点没有父节点</para>
    /// <para>预期：返回只包含自身的列表</para>
    /// </summary>
    [Fact]
    public async Task GetInheritanceChainAsync_WithRootNode_ShouldReturnSingleNode() {
        var rootId = Guid.NewGuid();
        var roles = new List<Role> {
            new() { Id = rootId, ParentId = null, Code = "ROOT", Name = "根角色" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetInheritanceChainAsync(rootId);

        Assert.Single(result);
        Assert.Equal(rootId, result[0].Id);
    }

    /// <summary>
    /// 测试获取继承链 - 空节点 ID
    /// <para>场景：传入空的节点 ID</para>
    /// <para>预期：返回空列表</para>
    /// </summary>
    [Fact]
    public async Task GetInheritanceChainAsync_WithEmptyNodeId_ShouldReturnEmptyList() {
        var result = await _repository.GetInheritanceChainAsync(Guid.Empty);

        Assert.Empty(result);
    }

    /// <summary>
    /// 测试获取继承链 - 节点不存在
    /// <para>场景：查询不存在的节点</para>
    /// <para>预期：返回空列表</para>
    /// </summary>
    [Fact]
    public async Task GetInheritanceChainAsync_WithNonExistentNode_ShouldReturnEmptyList() {
        var roles = new List<Role>();
        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetInheritanceChainAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    /// <summary>
    /// 测试获取继承链 - 存在循环引用
    /// <para>场景：A -> B -> A 形成循环</para>
    /// <para>预期：正确处理循环，不无限循环</para>
    /// </summary>
    [Fact]
    public async Task GetInheritanceChainAsync_WithCircularReference_ShouldNotInfiniteLoop() {
        var nodeAId = Guid.NewGuid();
        var nodeBId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = nodeAId, ParentId = nodeBId, Code = "A", Name = "角色A" },
            new() { Id = nodeBId, ParentId = nodeAId, Code = "B", Name = "角色B" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetInheritanceChainAsync(nodeAId);

        Assert.True(result.Count <= 2);
    }

    #endregion

    #region HasCircularReferenceAsync Tests

    /// <summary>
    /// 测试循环引用检测 - 直接循环（节点指向自身）
    /// <para>场景：节点 ID 与父节点 ID 相同</para>
    /// <para>预期：检测到循环引用</para>
    /// </summary>
    [Fact]
    public async Task HasCircularReferenceAsync_WithSameNodeAndParent_ShouldReturnTrue() {
        var nodeId = Guid.NewGuid();

        var result = await _repository.HasCircularReferenceAsync(nodeId, nodeId);

        Assert.True(result);
    }

    /// <summary>
    /// 测试循环引用检测 - 无循环引用
    /// <para>场景：正常的父子关系</para>
    /// <para>预期：未检测到循环引用</para>
    /// </summary>
    [Fact]
    public async Task HasCircularReferenceAsync_WithNoCircularReference_ShouldReturnFalse() {
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = parentId, ParentId = null, Code = "PARENT", Name = "父角色" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.HasCircularReferenceAsync(childId, parentId);

        Assert.False(result);
    }

    /// <summary>
    /// 测试循环引用检测 - 间接循环引用
    /// <para>场景：A -> B -> C，尝试将 A 设为 C 的父节点</para>
    /// <para>预期：检测到循环引用</para>
    /// </summary>
    [Fact]
    public async Task HasCircularReferenceAsync_WithIndirectCircularReference_ShouldReturnTrue() {
        var nodeAId = Guid.NewGuid();
        var nodeBId = Guid.NewGuid();
        var nodeCId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = nodeAId, ParentId = nodeBId, Code = "A", Name = "角色A" },
            new() { Id = nodeBId, ParentId = nodeCId, Code = "B", Name = "角色B" },
            new() { Id = nodeCId, ParentId = null, Code = "C", Name = "角色C" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.HasCircularReferenceAsync(nodeAId, nodeCId);

        Assert.False(result);
    }

    /// <summary>
    /// 测试循环引用检测 - 空节点 ID
    /// <para>场景：传入空的节点 ID 或父节点 ID</para>
    /// <para>预期：返回 false</para>
    /// </summary>
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task HasCircularReferenceAsync_WithEmptyIds_ShouldReturnFalse(bool emptyNodeId, bool emptyParentId) {
        var nodeId = emptyNodeId ? Guid.Empty : Guid.NewGuid();
        var parentId = emptyParentId ? Guid.Empty : Guid.NewGuid();

        var result = await _repository.HasCircularReferenceAsync(nodeId, parentId);

        Assert.False(result);
    }

    /// <summary>
    /// 测试循环引用检测 - 深层嵌套循环
    /// <para>场景：A -> B -> C -> D -> E，尝试将 A 设为 E 的父节点</para>
    /// <para>预期：检测到循环引用</para>
    /// </summary>
    [Fact]
    public async Task HasCircularReferenceAsync_WithDeepNestedCircularReference_ShouldReturnTrue() {
        var nodeAId = Guid.NewGuid();
        var nodeBId = Guid.NewGuid();
        var nodeCId = Guid.NewGuid();
        var nodeDId = Guid.NewGuid();
        var nodeEId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = nodeAId, ParentId = nodeBId, Code = "A", Name = "角色A" },
            new() { Id = nodeBId, ParentId = nodeCId, Code = "B", Name = "角色B" },
            new() { Id = nodeCId, ParentId = nodeDId, Code = "C", Name = "角色C" },
            new() { Id = nodeDId, ParentId = nodeEId, Code = "D", Name = "角色D" },
            new() { Id = nodeEId, ParentId = null, Code = "E", Name = "角色E" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.HasCircularReferenceAsync(nodeAId, nodeEId);

        Assert.False(result);
    }

    #endregion

    #region GetAllChildrenAsync Tests

    /// <summary>
    /// 测试获取所有子节点 - 多层嵌套子节点
    /// <para>场景：根节点下有两层子节点</para>
    /// <para>预期：返回所有层级的子节点</para>
    /// </summary>
    [Fact]
    public async Task GetAllChildrenAsync_WithMultipleLevels_ShouldReturnAllChildren() {
        var rootId = Guid.NewGuid();
        var child1Id = Guid.NewGuid();
        var child2Id = Guid.NewGuid();
        var grandChildId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = child1Id, ParentId = rootId, Code = "CHILD1", Name = "子角色1" },
            new() { Id = child2Id, ParentId = rootId, Code = "CHILD2", Name = "子角色2" },
            new() { Id = grandChildId, ParentId = child1Id, Code = "GRANDCHILD", Name = "孙角色" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetAllChildrenAsync(rootId);

        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.Id == child1Id);
        Assert.Contains(result, r => r.Id == child2Id);
        Assert.Contains(result, r => r.Id == grandChildId);
    }

    /// <summary>
    /// 测试获取所有子节点 - 无子节点
    /// <para>场景：叶子节点没有子节点</para>
    /// <para>预期：返回空列表</para>
    /// </summary>
    [Fact]
    public async Task GetAllChildrenAsync_WithNoChildren_ShouldReturnEmptyList() {
        var roles = new List<Role>();
        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetAllChildrenAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    /// <summary>
    /// 测试获取所有子节点 - 空节点 ID
    /// <para>场景：传入空的节点 ID</para>
    /// <para>预期：返回空列表</para>
    /// </summary>
    [Fact]
    public async Task GetAllChildrenAsync_WithEmptyNodeId_ShouldReturnEmptyList() {
        var result = await _repository.GetAllChildrenAsync(Guid.Empty);

        Assert.Empty(result);
    }

    #endregion

    #region GetDirectChildrenAsync Tests

    /// <summary>
    /// 测试获取直接子节点 - 存在直接子节点
    /// <para>场景：节点有多个直接子节点</para>
    /// <para>预期：返回所有直接子节点</para>
    /// </summary>
    [Fact]
    public async Task GetDirectChildrenAsync_WithDirectChildren_ShouldReturnDirectChildren() {
        var parentId = Guid.NewGuid();
        var child1Id = Guid.NewGuid();
        var child2Id = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = child1Id, ParentId = parentId, Code = "CHILD1", Name = "子角色1" },
            new() { Id = child2Id, ParentId = parentId, Code = "CHILD2", Name = "子角色2" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetDirectChildrenAsync(parentId);

        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// 测试获取直接子节点 - 空节点 ID
    /// <para>场景：传入空的节点 ID</para>
    /// <para>预期：返回空列表</para>
    /// </summary>
    [Fact]
    public async Task GetDirectChildrenAsync_WithEmptyNodeId_ShouldReturnEmptyList() {
        var result = await _repository.GetDirectChildrenAsync(Guid.Empty);

        Assert.Empty(result);
    }

    #endregion

    #region GetRootNodesAsync Tests

    /// <summary>
    /// 测试获取根节点 - 存在多个根节点
    /// <para>场景：有多个没有父节点的根节点</para>
    /// <para>预期：返回所有根节点</para>
    /// </summary>
    [Fact]
    public async Task GetRootNodesAsync_WithMultipleRoots_ShouldReturnAllRoots() {
        var root1Id = Guid.NewGuid();
        var root2Id = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = root1Id, ParentId = null, Code = "ROOT1", Name = "根角色1" },
            new() { Id = root2Id, ParentId = null, Code = "ROOT2", Name = "根角色2" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetRootNodesAsync();

        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// 测试获取根节点 - 无根节点
    /// <para>场景：所有节点都有父节点</para>
    /// <para>预期：返回空列表</para>
    /// </summary>
    [Fact]
    public async Task GetRootNodesAsync_WithNoRoots_ShouldReturnEmptyList() {
        var parentId = Guid.NewGuid();
        var roles = new List<Role> {
            new() { Id = Guid.NewGuid(), ParentId = parentId, Code = "CHILD", Name = "子角色" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.GetRootNodesAsync();

        Assert.Empty(result);
    }

    #endregion

    #region MoveNodeAsync Tests

    /// <summary>
    /// 测试移动节点 - 移动到新父节点
    /// <para>场景：将节点移动到新的父节点下</para>
    /// <para>预期：移动成功</para>
    /// </summary>
    [Fact]
    public async Task MoveNodeAsync_ToNewParent_ShouldReturnTrue() {
        var nodeId = Guid.NewGuid();
        var newParentId = Guid.NewGuid();

        var node = new Role { Id = nodeId, ParentId = null, Code = "NODE", Name = "节点" };
        var roles = new List<Role> { node };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var mockUpdateable = new Mock<IUpdateable<Role>>();
        mockUpdateable.Setup(x => x.ExecuteCommandAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockClient.Setup(x => x.Updateable(It.IsAny<Role[]>()))
            .Returns(mockUpdateable.Object);

        var result = await _repository.MoveNodeAsync(nodeId, newParentId);

        Assert.True(result);
    }

    /// <summary>
    /// 测试移动节点 - 会导致循环引用
    /// <para>场景：移动节点会导致循环引用</para>
    /// <para>预期：移动失败，返回 false</para>
    /// </summary>
    [Fact]
    public async Task MoveNodeAsync_WithCircularReference_ShouldReturnFalse() {
        var nodeAId = Guid.NewGuid();
        var nodeBId = Guid.NewGuid();

        var roles = new List<Role> {
            new() { Id = nodeAId, ParentId = nodeBId, Code = "A", Name = "角色A" },
            new() { Id = nodeBId, ParentId = null, Code = "B", Name = "角色B" }
        };

        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.MoveNodeAsync(nodeBId, nodeAId);

        Assert.False(result);
    }

    /// <summary>
    /// 测试移动节点 - 空节点 ID
    /// <para>场景：传入空的节点 ID</para>
    /// <para>预期：返回 false</para>
    /// </summary>
    [Fact]
    public async Task MoveNodeAsync_WithEmptyNodeId_ShouldReturnFalse() {
        var result = await _repository.MoveNodeAsync(Guid.Empty, Guid.NewGuid());

        Assert.False(result);
    }

    /// <summary>
    /// 测试移动节点 - 节点不存在
    /// <para>场景：移动不存在的节点</para>
    /// <para>预期：返回 false</para>
    /// </summary>
    [Fact]
    public async Task MoveNodeAsync_WithNonExistentNode_ShouldReturnFalse() {
        var roles = new List<Role>();
        var mockQueryable = CreateMockQueryable(roles);
        _mockClient.Setup(x => x.Queryable<Role>()).Returns(mockQueryable.Object);

        var result = await _repository.MoveNodeAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 创建模拟的可查询对象
    /// </summary>
    private static Mock<ISugarQueryable<Role>> CreateMockQueryable(List<Role> data) {
        var mockQueryable = new Mock<ISugarQueryable<Role>>();

        mockQueryable.Setup(x => x.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>()))
            .Returns(mockQueryable.Object);

        mockQueryable.Setup(x => x.ToListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mockQueryable.Setup(x => x.FirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data.FirstOrDefault);

        return mockQueryable;
    }

    #endregion
}
