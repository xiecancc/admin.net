/*
 * 文件名称: PermissionRepositoryTests.cs
 * 功能描述: 权限仓储测试类
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
/// 权限仓储测试类
/// <para>测试权限仓储的数据访问功能，包括权限查询、验证等</para>
/// </summary>
public class PermissionRepositoryTests {
    private readonly Mock<ISqlSugarClient> _mockClient;
    private readonly Mock<IDomainEventBus> _mockEventBus;
    private readonly Mock<IHttpContextProvider> _mockHttpContextProvider;
    private readonly Mock<ILogger<PermissionRepository<MenuPermission>>> _mockLogger;
    private readonly PermissionRepository<MenuPermission> _permissionRepository;

    public PermissionRepositoryTests() {
        _mockClient = new Mock<ISqlSugarClient>();
        _mockEventBus = new Mock<IDomainEventBus>();
        _mockHttpContextProvider = new Mock<IHttpContextProvider>();
        _mockLogger = new Mock<ILogger<PermissionRepository<MenuPermission>>>();

        _permissionRepository = new PermissionRepository<MenuPermission>(_mockClient.Object, _mockEventBus.Object, _mockHttpContextProvider.Object, "菜单权限", _mockLogger.Object);
    }

}
