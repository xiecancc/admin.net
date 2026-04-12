/*
 * 文件名称: PermissionAuthorizationIntegrationTests.cs
 * 功能描述: 权限校验集成测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace API.Test.Integration;

/// <summary>
/// 权限校验集成测试类
/// <para>测试权限校验机制在实际 HTTP 请求中的行为</para>
/// </summary>
public class PermissionAuthorizationIntegrationTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>, IAsyncDisposable {
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<Guid> _createdRoleIds = [];
    private readonly List<Guid> _createdUserIds = [];
    private string? _adminToken;

    public async ValueTask DisposeAsync() {
        await CleanupTestDataAsync();
        _client.Dispose();
        await Task.Delay(100);
    }

    /// <summary>
    /// 测试未登录用户访问受保护资源
    /// <para>预期结果：返回 401 Unauthorized</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_UnauthenticatedUser_Returns401() {
        var response = await _client.GetAsync("/api/v1/user");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// 测试无权限用户访问受保护资源
    /// <para>预期结果：返回 403 Forbidden</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_UserWithoutPermission_Returns403() {
        var userToken = await CreateAndLoginUserAsync("no_permission@test.com", "Test123!");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request.Headers.Authorization = new("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    /// <summary>
    /// 测试有权限用户访问受保护资源
    /// <para>预期结果：返回 200 OK</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_UserWithPermission_Returns200() {
        await InitializeAdminTokenAsync();

        var userToken = await CreateAndLoginUserWithPermissionAsync("has_permission@test.com", "Test123!", "user:view");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request.Headers.Authorization = new("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// 测试单个权限校验
    /// <para>预期结果：拥有指定权限的用户可以访问</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_SinglePermission_OnlyUsersWithPermissionCanAccess() {
        await InitializeAdminTokenAsync();

        var userWithPermission = await CreateAndLoginUserWithPermissionAsync("single_perm@test.com", "Test123!", "role:view");
        var userWithoutPermission = await CreateAndLoginUserAsync("single_no_perm@test.com", "Test123!");

        using var request1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/role");
        request1.Headers.Authorization = new("Bearer", userWithPermission);
        var response1 = await _client.SendAsync(request1);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        using var request2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/role");
        request2.Headers.Authorization = new("Bearer", userWithoutPermission);
        var response2 = await _client.SendAsync(request2);
        Assert.Equal(HttpStatusCode.Forbidden, response2.StatusCode);
    }

    /// <summary>
    /// 测试权限变更后立即生效
    /// <para>预期结果：撤销权限后立即无法访问</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_PermissionRevoked_ImmediatelyTakesEffect() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("revoke_test@test.com", "Test123!");
        var roleId = await CreateRoleWithPermissionAsync("revoke_role", "user:view");
        await AssignRoleToUserAsync(userId, roleId);

        var userToken = await LoginUserAsync("revoke_test@test.com", "Test123!");

        using var request1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request1.Headers.Authorization = new("Bearer", userToken);
        var response1 = await _client.SendAsync(request1);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        await RemoveRoleFromUserAsync(userId, roleId);

        using var request2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request2.Headers.Authorization = new("Bearer", userToken);
        var response2 = await _client.SendAsync(request2);
        Assert.Equal(HttpStatusCode.Forbidden, response2.StatusCode);
    }

    /// <summary>
    /// 测试角色权限变更后立即生效
    /// <para>预期结果：角色权限变更后用户权限立即更新</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_RolePermissionChanged_ImmediatelyAffectsUser() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("role_change@test.com", "Test123!");
        var roleId = await CreateRoleAsync("role_change_test");
        await AssignRoleToUserAsync(userId, roleId);

        var userToken = await LoginUserAsync("role_change@test.com", "Test123!");

        using var request1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request1.Headers.Authorization = new("Bearer", userToken);
        var response1 = await _client.SendAsync(request1);
        Assert.Equal(HttpStatusCode.Forbidden, response1.StatusCode);

        var permissionId = await GetPermissionIdByCodeAsync("user:view");
        await AssignPermissionToRoleAsync(roleId, permissionId);

        using var request2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request2.Headers.Authorization = new("Bearer", userToken);
        var response2 = await _client.SendAsync(request2);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
    }

    /// <summary>
    /// 测试通过继承获得的权限校验
    /// <para>预期结果：继承的权限同样有效</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_InheritedPermission_IsEffective() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleWithPermissionAsync("parent_inherit_perm", "user:view");
        var childRoleId = await CreateRoleWithInheritanceAsync("child_inherit_perm", parentRoleId, InheritanceType.Upward);

        var userId = await CreateUserAsync("inherit_perm@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        var userToken = await LoginUserAsync("inherit_perm@test.com", "Test123!");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request.Headers.Authorization = new("Bearer", userToken);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// 测试多个角色的权限合并
    /// <para>预期结果：用户拥有所有角色的权限并集</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_MultipleRoles_PermissionsAreMerged() {
        await InitializeAdminTokenAsync();

        var role1Id = await CreateRoleWithPermissionAsync("multi_role_1", "user:view");
        var role2Id = await CreateRoleWithPermissionAsync("multi_role_2", "role:view");

        var userId = await CreateUserAsync("multi_role@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, role1Id);
        await AssignRoleToUserAsync(userId, role2Id);

        var userToken = await LoginUserAsync("multi_role@test.com", "Test123!");

        using var request1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request1.Headers.Authorization = new("Bearer", userToken);
        var response1 = await _client.SendAsync(request1);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        using var request2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/role");
        request2.Headers.Authorization = new("Bearer", userToken);
        var response2 = await _client.SendAsync(request2);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
    }

    /// <summary>
    /// 测试权限校验错误消息格式
    /// <para>预期结果：返回正确的错误消息格式</para>
    /// </summary>
    [Fact]
    public async Task PermissionCheck_Forbidden_ReturnsCorrectErrorMessage() {
        var userToken = await CreateAndLoginUserAsync("error_msg@test.com", "Test123!");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/user");
        request.Headers.Authorization = new("Bearer", userToken);

        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(result.TryGetProperty("success", out var successProp), "响应应包含 success 字段");
        Assert.False(successProp.GetBoolean(), "success 应为 false");
        Assert.True(result.TryGetProperty("message", out var messageProp), "响应应包含 message 字段");
        Assert.False(string.IsNullOrEmpty(messageProp.GetString()), "message 不应为空");
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

    private async Task<string> CreateAndLoginUserAsync(string email, string password) {
        await InitializeAdminTokenAsync();
        await CreateUserAsync(email, password);
        return await LoginUserAsync(email, password);
    }

    private async Task<string> CreateAndLoginUserWithPermissionAsync(string email, string password, string permissionCode) {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync(email, password);
        var roleId = await CreateRoleWithPermissionAsync($"role_{email.Split('@')[0]}", permissionCode);
        await AssignRoleToUserAsync(userId, roleId);

        return await LoginUserAsync(email, password);
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

    private async Task<string> LoginUserAsync(string email, string password) {
        var loginDto = new {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        return result.GetProperty("token").GetString() ?? throw new InvalidOperationException("登录失败");
    }

    private async Task<Guid> CreateRoleAsync(string code) {
        var dto = new RoleCreateDto {
            Code = code,
            Name = code
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

    private async Task<Guid> CreateRoleWithInheritanceAsync(string code, Guid parentId, InheritanceType inheritanceType) {
        var roleId = await CreateRoleAsync(code);

        using var scope = factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        await client.Updateable<Role>()
            .SetColumns(r => new Role {
                ParentId = parentId,
                InheritanceType = inheritanceType
            })
            .Where(r => r.Id == roleId)
            .ExecuteCommandAsync();

        return roleId;
    }

    private async Task<Guid> CreateRoleWithPermissionAsync(string code, string permissionCode) {
        var roleId = await CreateRoleAsync(code);
        var permissionId = await GetPermissionIdByCodeAsync(permissionCode);
        await AssignPermissionToRoleAsync(roleId, permissionId);
        return roleId;
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

    private async Task<Guid> GetPermissionIdByCodeAsync(string code) {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/permission");
        request.Headers.Authorization = new("Bearer", _adminToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);

        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            foreach (var permission in data.EnumerateArray()) {
                if (permission.TryGetProperty("code", out var codeProp) && codeProp.GetString() == code) {
                    return permission.GetProperty("id").GetGuid();
                }
            }
        }

        throw new InvalidOperationException($"无法找到权限: {code}");
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

    private async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId) {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/user/{userId}/roles");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(new List<Guid> { roleId });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
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
