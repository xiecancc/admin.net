/*
 * 文件名称: DepartmentRepositoryTests.cs
 * 功能描述: 部门仓储测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Domain.Entities;
using Domain.Shared.Events;
using Infrastructure.Repositories;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using Moq;
using SqlSugar;

namespace Infrastructure.Test.Repositories;

/// <summary>
/// 部门仓储测试类
/// <para>测试部门仓储的数据访问功能，包括查询、存在性检查等</para>
/// </summary>
public class DepartmentRepositoryTests {
    private readonly Mock<ISqlSugarClient> _mockClient;
    private readonly Mock<IDomainEventBus> _mockEventBus;
    private readonly Mock<IHttpContextProvider> _mockHttpContextProvider;
    private readonly Mock<ILogger<DepartmentRepository>> _mockLogger;
    private readonly DepartmentRepository _departmentRepository;

    public DepartmentRepositoryTests() {
        _mockClient = new Mock<ISqlSugarClient>();
        _mockEventBus = new Mock<IDomainEventBus>();
        _mockHttpContextProvider = new Mock<IHttpContextProvider>();
        _mockLogger = new Mock<ILogger<DepartmentRepository>>();

        _departmentRepository = new DepartmentRepository(
            _mockClient.Object,
            _mockEventBus.Object,
            _mockHttpContextProvider.Object,
            _mockLogger.Object
        );
    }

    /// <summary>
    /// 测试根据编码查找部门（部门存在）
    /// <para>预期结果：返回对应的部门实体</para>
    /// </summary>
    [Fact]
    public async Task FindByCodeAsync_WithExistingCode_ShouldReturnDepartment() {
        var code = "DEPT001";
        var department = new Department { Id = Guid.NewGuid(), Code = code, Name = "Test Department" };

        var mockQueryable = new Mock<ISugarQueryable<Department>>();
        _ = _mockClient.Setup(c => c.Queryable<Department>()).Returns(mockQueryable.Object);
        _ = mockQueryable.Setup(c => c.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>())).Returns(mockQueryable.Object);
        _ = mockQueryable.Setup(c => c.FirstAsync(It.IsAny<CancellationToken>())).ReturnsAsync(department);

        _ = _mockEventBus.Setup(c => c.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _departmentRepository.FindByCodeAsync(code);

        Assert.NotNull(result);
        Assert.Equal(code, result!.Code);
    }

    /// <summary>
    /// 测试检查编码是否存在（编码存在）
    /// <para>预期结果：返回 true</para>
    /// </summary>
    [Fact]
    public async Task IsCodeExistsAsync_WithExistingCode_ShouldReturnTrue() {
        var code = "DEPT001";

        var mockQueryable = new Mock<ISugarQueryable<Department>>();
        _ = _mockClient.Setup(c => c.Queryable<Department>()).Returns(mockQueryable.Object);
        _ = mockQueryable.Setup(c => c.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>())).Returns(mockQueryable.Object);
        _ = mockQueryable.Setup(c => c.AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _departmentRepository.IsCodeExistsAsync(code);

        Assert.True(result);
    }

    /// <summary>
    /// 测试检查编码是否存在（编码不存在）
    /// <para>预期结果：返回 false</para>
    /// </summary>
    [Fact]
    public async Task IsCodeExistsAsync_WithNonExistingCode_ShouldReturnFalse() {
        var code = "DEPT001";

        var mockQueryable = new Mock<ISugarQueryable<Department>>();
        _ = _mockClient.Setup(c => c.Queryable<Department>()).Returns(mockQueryable.Object);
        _ = mockQueryable.Setup(c => c.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Department, bool>>>())).Returns(mockQueryable.Object);
        _ = mockQueryable.Setup(c => c.AnyAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _departmentRepository.IsCodeExistsAsync(code);

        Assert.False(result);
    }
}