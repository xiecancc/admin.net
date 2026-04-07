/*
 * 文件名称: RoleRepositoryTests.cs
 * 功能描述: 角色仓储测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Entities;
using Domain.Shared.Constants;
using Domain.Shared.Events;
using Infrastructure.Repositories;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using Moq;
using SqlSugar;

namespace Infrastructure.Test.Repositories;

/// <summary>
/// 角色仓储测试类
/// <para>测试角色仓储的数据访问功能，包括查询、存在性检查等</para>
/// </summary>
public class RoleRepositoryTests {
    private readonly Mock<ISqlSugarClient> _mockClient;
    private readonly Mock<IDomainEventBus> _mockEventBus;
    private readonly Mock<IHttpContextProvider> _mockHttpContextProvider;
    private readonly Mock<ILogger<RoleRepository>> _mockLogger;
    private readonly RoleRepository _roleRepository;

    public RoleRepositoryTests() {
        _mockClient = new Mock<ISqlSugarClient>();
        _mockEventBus = new Mock<IDomainEventBus>();
        _mockHttpContextProvider = new Mock<IHttpContextProvider>();
        _mockLogger = new Mock<ILogger<RoleRepository>>();

        _roleRepository = new RoleRepository(
            _mockClient.Object,
            _mockEventBus.Object,
            _mockHttpContextProvider.Object,
            _mockLogger.Object
        );
    }

    /// <summary>
    /// 测试根据编码查找角色（角色存在）
    /// <para>预期结果：返回对应的角色实体</para>
    /// </summary>
    [Fact]
    public async Task FindByCodeAsync_WithExistingCode_ShouldReturnRole() {
        var code = "ADMIN";
        var role = new Role { Id = Guid.NewGuid(), Code = code, Name = RoleConstants.Administrator.NAME };

        var mockQueryable = new Mock<ISugarQueryable<Role>>();
        _ = mockQueryable.Setup(x => x.FirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)role);

        _ = _mockClient.Setup(x => x.Queryable<Role>())
            .Returns(mockQueryable.Object);

        var result = await _roleRepository.FindByCodeAsync(code);

        Assert.NotNull(result);
        Assert.Equal(code, result.Code);
        Assert.Equal(role.Id, result.Id);
    }

    /// <summary>
    /// 测试根据编码查找角色（角色不存在）
    /// <para>预期结果：返回 null</para>
    /// </summary>
    [Fact]
    public async Task FindByCodeAsync_WithNonExistingCode_ShouldReturnNull() {
        var code = "NONEXISTENT";

        var mockQueryable = new Mock<ISugarQueryable<Role>>();
        _ = mockQueryable.Setup(x => x.FirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(Role));

        _ = _mockClient.Setup(x => x.Queryable<Role>())
            .Returns(mockQueryable.Object);

        var result = await _roleRepository.FindByCodeAsync(code);

        Assert.Null(result);
    }

    /// <summary>
    /// 测试检查编码是否存在（编码存在）
    /// <para>预期结果：返回 true</para>
    /// </summary>
    [Fact]
    public async Task IsCodeExistsAsync_WithExistingCode_ShouldReturnTrue() {
        var code = "ADMIN";

        var mockQueryable = new Mock<ISugarQueryable<Role>>();
        _ = mockQueryable.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _ = _mockClient.Setup(x => x.Queryable<Role>())
            .Returns(mockQueryable.Object);

        var result = await _roleRepository.IsCodeExistsAsync(code);

        Assert.True(result);
    }

    /// <summary>
    /// 测试检查编码是否存在（编码不存在）
    /// <para>预期结果：返回 false</para>
    /// </summary>
    [Fact]
    public async Task IsCodeExistsAsync_WithNonExistingCode_ShouldReturnFalse() {
        var code = "NONEXISTENT";

        var mockQueryable = new Mock<ISugarQueryable<Role>>();
        _ = mockQueryable.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _ = _mockClient.Setup(x => x.Queryable<Role>())
            .Returns(mockQueryable.Object);

        var result = await _roleRepository.IsCodeExistsAsync(code);

        Assert.False(result);
    }


}
