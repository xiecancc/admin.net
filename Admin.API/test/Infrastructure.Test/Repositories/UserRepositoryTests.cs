/*
 * 文件名称: UserRepositoryTests.cs
 * 功能描述: 用户仓储测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
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
/// 用户仓储测试类
/// <para>测试用户仓储的数据访问功能，包括查询、存在性检查等</para>
/// </summary>
public class UserRepositoryTests {
    private readonly Mock<ISqlSugarClient> _mockClient;
    private readonly Mock<IDomainEventBus> _mockEventBus;
    private readonly Mock<IUserContextProvider> _mockUserContextProvider;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests() {
        _mockClient = new Mock<ISqlSugarClient>();
        _mockEventBus = new Mock<IDomainEventBus>();
        _mockUserContextProvider = new Mock<IUserContextProvider>();
        _mockLogger = new Mock<ILogger<UserRepository>>();

        _userRepository = new UserRepository(
            _mockClient.Object,
            _mockEventBus.Object,
            _mockUserContextProvider.Object,
            _mockLogger.Object
        );
    }

    /// <summary>
    /// 测试根据邮箱查找用户（用户存在）
    /// <para>预期结果：返回对应的用户实体</para>
    /// </summary>
    [Fact]
    public async Task FindByEmailAsync_WithExistingEmail_ShouldReturnUser() {
        var email = "test@example.com";
        var user = new User { Id = Guid.NewGuid(), Email = email, NickName = "Test User" };

        var mockQueryable = new Mock<ISugarQueryable<User>>();
        _mockClient.Setup(c => c.Queryable<User>()).Returns(mockQueryable.Object);
        mockQueryable.Setup(c => c.Where(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>())).Returns(mockQueryable.Object);
        mockQueryable.Setup(c => c.FirstAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);

        _mockEventBus.Setup(c => c.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _userRepository.FindByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(email, result!.Email);
    }
}
