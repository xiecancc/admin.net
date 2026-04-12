/*
 * 文件名称: RoleInheritanceIntegrationTests.cs
 * 功能描述: 角色继承集成测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace API.Test.Integration;

/// <summary>
/// 角色继承集成测试类
/// <para>测试角色继承机制在实际 HTTP 请求中的行为</para>
/// </summary>
public class RoleInheritanceIntegrationTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>, IAsyncDisposable {
    private readonly HttpClient _client = factory.CreateClient();
    private readonly TestWebApplicationFactory _factory = factory;
    private readonly List<Guid> _createdRoleIds = [];
    private readonly List<Guid> _createdUserIds = [];
    private string? _adminToken;

    public async ValueTask DisposeAsync() {
        await CleanupTestDataAsync();
        _client.Dispose();
        await Task.Delay(100);
    }

    /// <summary>
    /// 测试向上继承：子角色继承父角色权限
    /// <para>预期结果：子角色用户拥有父角色的权限</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_UpwardInheritance_ChildRoleInheritsParentPermissions() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleAsync("parent_role_upward", "父角色-向上继承");
        var childRoleId = await CreateRoleWithInheritanceAsync("child_role_upward", "子角色-向上继承", parentRoleId, InheritanceType.Upward);

        var permissionId = await GetFirstPermissionIdAsync();
        await AssignPermissionToRoleAsync(parentRoleId, permissionId);

        var userId = await CreateUserAsync("test_upward@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        var hasPermission = await CheckUserPermissionAsync(userId, permissionId);
        Assert.True(hasPermission, "子角色用户应该继承父角色的权限");
    }

    /// <summary>
    /// 测试向下继承：父角色继承子角色权限
    /// <para>预期结果：父角色用户拥有子角色的权限</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_DownwardInheritance_ParentRoleInheritsChildPermissions() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleAsync("parent_role_downward", "父角色-向下继承");
        var childRoleId = await CreateRoleWithInheritanceAsync("child_role_downward", "子角色-向下继承", parentRoleId, InheritanceType.Downward);

        var permissionId = await GetFirstPermissionIdAsync();
        await AssignPermissionToRoleAsync(childRoleId, permissionId);

        var userId = await CreateUserAsync("test_downward@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, parentRoleId);

        var hasPermission = await CheckUserPermissionAsync(userId, permissionId);
        Assert.True(hasPermission, "父角色用户应该继承子角色的权限");
    }

    /// <summary>
    /// 测试不继承：角色不继承任何权限
    /// <para>预期结果：角色用户只拥有直接分配的权限</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_NoInheritance_RoleOnlyHasDirectPermissions() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleAsync("parent_role_none", "父角色-不继承");
        var childRoleId = await CreateRoleWithInheritanceAsync("child_role_none", "子角色-不继承", parentRoleId, InheritanceType.None);

        var permissionId = await GetFirstPermissionIdAsync();
        await AssignPermissionToRoleAsync(parentRoleId, permissionId);

        var userId = await CreateUserAsync("test_none@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        var hasPermission = await CheckUserPermissionAsync(userId, permissionId);
        Assert.False(hasPermission, "不继承模式下子角色用户不应继承父角色权限");
    }

    /// <summary>
    /// 测试多级继承：三级角色继承链
    /// <para>预期结果：最底层角色继承所有上级角色的权限</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_MultiLevelInheritance_BottomRoleInheritsAllPermissions() {
        await InitializeAdminTokenAsync();

        var grandParentRoleId = await CreateRoleAsync("grandparent_role", "祖父角色");
        var parentRoleId = await CreateRoleWithInheritanceAsync("parent_role_multi", "父角色-多级", grandParentRoleId, InheritanceType.Upward);
        var childRoleId = await CreateRoleWithInheritanceAsync("child_role_multi", "子角色-多级", parentRoleId, InheritanceType.Upward);

        var permissions = await GetPermissionIdsAsync(3);
        Assert.True(permissions.Count >= 3, "需要至少3个权限进行测试");

        await AssignPermissionToRoleAsync(grandParentRoleId, permissions[0]);
        await AssignPermissionToRoleAsync(parentRoleId, permissions[1]);
        await AssignPermissionToRoleAsync(childRoleId, permissions[2]);

        var userId = await CreateUserAsync("test_multi@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        var hasGrandParentPermission = await CheckUserPermissionAsync(userId, permissions[0]);
        var hasParentPermission = await CheckUserPermissionAsync(userId, permissions[1]);
        var hasChildPermission = await CheckUserPermissionAsync(userId, permissions[2]);

        Assert.True(hasGrandParentPermission, "子角色用户应该继承祖父角色的权限");
        Assert.True(hasParentPermission, "子角色用户应该继承父角色的权限");
        Assert.True(hasChildPermission, "子角色用户应该拥有自己的权限");
    }

    /// <summary>
    /// 测试继承类型变更：从向上继承变更为不继承
    /// <para>预期结果：变更后用户权限立即更新</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_ChangeInheritanceType_PermissionsUpdatedImmediately() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleAsync("parent_role_change", "父角色-变更测试");
        var childRoleId = await CreateRoleWithInheritanceAsync("child_role_change", "子角色-变更测试", parentRoleId, InheritanceType.Upward);

        var permissionId = await GetFirstPermissionIdAsync();
        await AssignPermissionToRoleAsync(parentRoleId, permissionId);

        var userId = await CreateUserAsync("test_change@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        var hasPermissionBefore = await CheckUserPermissionAsync(userId, permissionId);
        Assert.True(hasPermissionBefore, "变更前应该有权限");

        await SetRoleInheritanceAsync(childRoleId, parentRoleId, InheritanceType.None);

        var hasPermissionAfter = await CheckUserPermissionAsync(userId, permissionId);
        Assert.False(hasPermissionAfter, "变更后不应该有权限");
    }

    /// <summary>
    /// 测试循环引用检测：角色不能形成循环继承
    /// <para>预期结果：设置循环引用时返回错误</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_CircularReference_ShouldBeRejected() {
        await InitializeAdminTokenAsync();

        var role1Id = await CreateRoleAsync("role_circular_1", "角色1-循环测试");
        var role2Id = await CreateRoleWithInheritanceAsync("role_circular_2", "角色2-循环测试", role1Id, InheritanceType.Upward);

        var result = await TrySetParentRoleAsync(role1Id, role2Id);
        Assert.False(result, "不应该允许设置循环引用");
    }

    /// <summary>
    /// 测试角色继承链查询
    /// <para>预期结果：正确返回完整的继承链</para>
    /// </summary>
    [Fact]
    public async Task RoleInheritance_GetInheritanceChain_ReturnsCorrectChain() {
        await InitializeAdminTokenAsync();

        var role1Id = await CreateRoleAsync("chain_role_1", "链角色1");
        var role2Id = await CreateRoleWithInheritanceAsync("chain_role_2", "链角色2", role1Id, InheritanceType.Upward);
        var role3Id = await CreateRoleWithInheritanceAsync("chain_role_3", "链角色3", role2Id, InheritanceType.Upward);

        var chain = await GetRoleInheritanceChainAsync(role3Id);

        Assert.Contains(role1Id, chain);
        Assert.Contains(role2Id, chain);
        Assert.Contains(role3Id, chain);
    }

    #region Helper Methods

    private async Task InitializeAdminTokenAsync() {
        if (_adminToken != null) {
            return;
        }

        var loginDto = new {
            Email = "admin@admin.com",
            Password = "Admin123!"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            _adminToken = result.GetProperty("token").GetString();
        }
    }

    private async Task<Guid> CreateRoleAsync(string code, string name) {
        var dto = new RoleCreateDto {
            Code = code,
            Name = name
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/role");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(dto);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var roleId = await GetRoleIdByCodeAsync(code);
        _createdRoleIds.Add(roleId);
        return roleId;
    }

    private async Task<Guid> CreateRoleWithInheritanceAsync(string code, string name, Guid? parentId, InheritanceType inheritanceType) {
        var roleId = await CreateRoleAsync(code, name);

        if (parentId.HasValue || inheritanceType != InheritanceType.None) {
            await SetRoleInheritanceAsync(roleId, parentId, inheritanceType);
        }

        return roleId;
    }

    private async Task SetRoleInheritanceAsync(Guid roleId, Guid? parentId, InheritanceType inheritanceType) {
        using var scope = _factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        await client.Updateable<Domain.Entities.Role>()
            .SetColumns(r => new Domain.Entities.Role {
                ParentId = parentId,
                InheritanceType = inheritanceType
            })
            .Where(r => r.Id == roleId)
            .ExecuteCommandAsync();
    }

    private async Task<Guid> GetRoleIdByCodeAsync(string code) {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/role?code={code}");
        request.Headers.Authorization = new("Bearer", _adminToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            var role = data.EnumerateArray().FirstOrDefault();
            if (role.TryGetProperty("id", out var roleIdProp)) {
                return roleIdProp.GetGuid();
            }
        }

        throw new InvalidOperationException("无法获取创建的角色ID");
    }

    private async Task<Guid> CreateUserAsync(string email, string password) {
        var dto = new UserCreateDto {
            Email = email,
            Password = password,
            NickName = email.Split('@')[0]
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/user");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(dto);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var usersResponse = await _client.GetAsync($"/api/v1/user?email={email}");
        if (usersResponse.IsSuccessStatusCode) {
            var content = await usersResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
                var user = data.EnumerateArray().FirstOrDefault();
                if (user.TryGetProperty("id", out var userIdProp)) {
                    var userId = userIdProp.GetGuid();
                    _createdUserIds.Add(userId);
                    return userId;
                }
            }
        }

        throw new InvalidOperationException("无法获取创建的用户ID");
    }

    private async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId) {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/role/{roleId}/permissions");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(new List<Guid> { permissionId });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task AssignRoleToUserAsync(Guid userId, Guid roleId) {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/user/{userId}/roles");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(new List<Guid> { roleId });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task<bool> CheckUserPermissionAsync(Guid userId, Guid permissionId) {
        using var scope = _factory.Services.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<Domain.Services.IPermissionDomainService>();
        return await permissionService.UserHasPermissionAsync(userId, permissionId);
    }

    private async Task<Guid> GetFirstPermissionIdAsync() {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/permission");
        request.Headers.Authorization = new("Bearer", _adminToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            var permission = data.EnumerateArray().FirstOrDefault();
            if (permission.TryGetProperty("id", out var idProp)) {
                return idProp.GetGuid();
            }
        }

        throw new InvalidOperationException("无法获取权限ID");
    }

    private async Task<List<Guid>> GetPermissionIdsAsync(int count) {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/permission");
        request.Headers.Authorization = new("Bearer", _adminToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        var ids = new List<Guid>();
        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            foreach (var permission in data.EnumerateArray().Take(count)) {
                if (permission.TryGetProperty("id", out var idProp)) {
                    ids.Add(idProp.GetGuid());
                }
            }
        }

        return ids;
    }

    private async Task<bool> TrySetParentRoleAsync(Guid roleId, Guid parentId) {
        using var scope = _factory.Services.CreateScope();
        var roleRepository = scope.ServiceProvider.GetRequiredService<Domain.Repositories.IRoleRepository>();

        var hasCycle = await roleRepository.HasInheritanceCycleAsync(roleId, parentId);
        return !hasCycle;
    }

    private async Task<List<Guid>> GetRoleInheritanceChainAsync(Guid roleId) {
        using var scope = _factory.Services.CreateScope();
        var roleRepository = scope.ServiceProvider.GetRequiredService<Domain.Repositories.IRoleRepository>();
        var chain = await roleRepository.GetInheritanceChainAsync(roleId);
        return chain.Select(r => r.Id).ToList();
    }

#pragma warning disable CA1031 // 需要捕获所有异常以确保清理不会中断
    private async Task CleanupTestDataAsync() {
        if (_adminToken == null) {
            return;
        }

        foreach (var userId in _createdUserIds) {
            try {
                using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/user");
                request.Headers.Authorization = new("Bearer", _adminToken);
                request.Content = JsonContent.Create(new List<Guid> { userId });
                await _client.SendAsync(request);
            }
            catch {
                // Ignore cleanup errors
            }
        }

        foreach (var roleId in _createdRoleIds) {
            try {
                using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/role");
                request.Headers.Authorization = new("Bearer", _adminToken);
                request.Content = JsonContent.Create(new List<Guid> { roleId });
                await _client.SendAsync(request);
            }
            catch {
                // Ignore cleanup errors
            }
        }
    }
#pragma warning restore CA1031

    #endregion
}
