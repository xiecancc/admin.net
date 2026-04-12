/*
 * 文件名称: CacheInvalidationIntegrationTests.cs
 * 功能描述: 缓存失效集成测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application.Contracts.Dtos;
using Domain.Shared.Constants;
using Domain.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace API.Test.Integration;

/// <summary>
/// 缓存失效集成测试类
/// <para>测试缓存失效机制在实际 HTTP 请求中的行为</para>
/// </summary>
public class CacheInvalidationIntegrationTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>, IAsyncDisposable {
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
    /// 测试用户角色变更后缓存失效
    /// <para>预期结果：用户角色变更后权限缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_UserRoleChanged_ClearsUserPermissionCache() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("cache_user_role@test.com", "Test123!");
        var roleId = await CreateRoleWithPermissionAsync("cache_user_role", "user:view");

        await WarmupUserPermissionCacheAsync(userId);
        Assert.True(await IsUserPermissionCachedAsync(userId));

        await AssignRoleToUserAsync(userId, roleId);

        Assert.False(await IsUserPermissionCachedAsync(userId));

        var permissions = await GetUserPermissionsAsync(userId);
        Assert.Contains("user:view", permissions);
    }

    /// <summary>
    /// 测试角色权限变更后缓存失效
    /// <para>预期结果：角色权限变更后相关用户缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_RolePermissionChanged_ClearsRelatedUserCache() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("cache_role_perm@test.com", "Test123!");
        var roleId = await CreateRoleAsync("cache_role_perm");
        await AssignRoleToUserAsync(userId, roleId);

        await WarmupUserPermissionCacheAsync(userId);
        Assert.True(await IsUserPermissionCachedAsync(userId));

        var permissionId = await GetPermissionIdByCodeAsync("user:view");
        await AssignPermissionToRoleAsync(roleId, permissionId);

        Assert.False(await IsUserPermissionCachedAsync(userId));

        var permissions = await GetUserPermissionsAsync(userId);
        Assert.Contains("user:view", permissions);
    }

    /// <summary>
    /// 测试角色继承关系变更后缓存失效
    /// <para>预期结果：角色继承变更后相关缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_RoleInheritanceChanged_ClearsInheritedCache() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleWithPermissionAsync("cache_parent_inherit", "user:view");
        var childRoleId = await CreateRoleAsync("cache_child_inherit");

        var userId = await CreateUserAsync("cache_inherit@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        await WarmupUserPermissionCacheAsync(userId);
        await WarmupRoleInheritedPermissionCacheAsync(childRoleId);

        Assert.True(await IsUserPermissionCachedAsync(userId));

        await SetRoleInheritanceAsync(childRoleId, parentRoleId, InheritanceType.Upward);

        Assert.False(await IsRoleInheritedPermissionCachedAsync(childRoleId));
        Assert.False(await IsUserPermissionCachedAsync(userId));

        var permissions = await GetUserPermissionsAsync(userId);
        Assert.Contains("user:view", permissions);
    }

    /// <summary>
    /// 测试角色继承类型变更后缓存失效
    /// <para>预期结果：继承类型变更后相关缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_InheritanceTypeChanged_ClearsRelatedCache() {
        await InitializeAdminTokenAsync();

        var parentRoleId = await CreateRoleWithPermissionAsync("cache_parent_type", "user:view");
        var childRoleId = await CreateRoleWithInheritanceAsync("cache_child_type", parentRoleId, InheritanceType.Upward);

        var userId = await CreateUserAsync("cache_type@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        await WarmupUserPermissionCacheAsync(userId);
        Assert.True(await IsUserPermissionCachedAsync(userId));

        await SetRoleInheritanceAsync(childRoleId, parentRoleId, InheritanceType.None);

        Assert.False(await IsUserPermissionCachedAsync(userId));

        var permissions = await GetUserPermissionsAsync(userId);
        Assert.DoesNotContain("user:view", permissions);
    }

    /// <summary>
    /// 测试批量角色分配后缓存失效
    /// <para>预期结果：批量分配后所有用户缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_BatchRoleAssign_ClearsAllUserCaches() {
        await InitializeAdminTokenAsync();

        var user1Id = await CreateUserAsync("cache_batch_1@test.com", "Test123!");
        var user2Id = await CreateUserAsync("cache_batch_2@test.com", "Test123!");
        var roleId = await CreateRoleWithPermissionAsync("cache_batch_role", "user:view");

        await WarmupUserPermissionCacheAsync(user1Id);
        await WarmupUserPermissionCacheAsync(user2Id);

        Assert.True(await IsUserPermissionCachedAsync(user1Id));
        Assert.True(await IsUserPermissionCachedAsync(user2Id));

        await AssignRoleToUserAsync(user1Id, roleId);
        await AssignRoleToUserAsync(user2Id, roleId);

        Assert.False(await IsUserPermissionCachedAsync(user1Id));
        Assert.False(await IsUserPermissionCachedAsync(user2Id));
    }

    /// <summary>
    /// 测试权限删除后缓存失效
    /// <para>预期结果：权限删除后所有相关缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_PermissionDeleted_ClearsAllRelatedCaches() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("cache_perm_del@test.com", "Test123!");
        var roleId = await CreateRoleAsync("cache_perm_del_role");
        await AssignRoleToUserAsync(userId, roleId);

        var permissionId = await GetPermissionIdByCodeAsync("user:view");
        await AssignPermissionToRoleAsync(roleId, permissionId);

        await WarmupUserPermissionCacheAsync(userId);
        Assert.True(await IsUserPermissionCachedAsync(userId));

        await RemovePermissionFromRoleAsync(roleId, permissionId);

        Assert.False(await IsUserPermissionCachedAsync(userId));
    }

    /// <summary>
    /// 测试用户角色撤销后缓存失效
    /// <para>预期结果：撤销角色后用户缓存被清除</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_RoleRevoked_ClearsUserCache() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("cache_revoke@test.com", "Test123!");
        var roleId = await CreateRoleWithPermissionAsync("cache_revoke_role", "user:view");
        await AssignRoleToUserAsync(userId, roleId);

        await WarmupUserPermissionCacheAsync(userId);
        Assert.True(await IsUserPermissionCachedAsync(userId));

        var permissionsBefore = await GetUserPermissionsAsync(userId);
        Assert.Contains("user:view", permissionsBefore);

        await RemoveRoleFromUserAsync(userId, roleId);

        Assert.False(await IsUserPermissionCachedAsync(userId));

        var permissionsAfter = await GetUserPermissionsAsync(userId);
        Assert.DoesNotContain("user:view", permissionsAfter);
    }

    /// <summary>
    /// 测试缓存键格式正确性
    /// <para>预期结果：缓存键符合规范格式</para>
    /// </summary>
    [Fact]
    public void CacheInvalidation_CacheKeyFormat_IsCorrect() {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var userPermissionKey = CacheKeyConstants.User.Permissions(userId);
        Assert.Equal($"user:permissions:{userId}", userPermissionKey);

        var roleInheritedKey = CacheKeyConstants.Role.InheritedPermissions(roleId);
        Assert.Equal($"role:inherited_permissions:{roleId}", roleInheritedKey);

        var userDetailKey = CacheKeyConstants.User.Detail(userId);
        Assert.Equal($"user:detail:{userId}", userDetailKey);

        var roleDetailKey = CacheKeyConstants.Role.Detail(roleId);
        Assert.Equal($"role:detail:{roleId}", roleDetailKey);
    }

    /// <summary>
    /// 测试缓存预热功能
    /// <para>预期结果：预热后缓存存在</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_Warmup_CreatesCache() {
        await InitializeAdminTokenAsync();

        var userId = await CreateUserAsync("cache_warmup@test.com", "Test123!");
        var roleId = await CreateRoleWithPermissionAsync("cache_warmup_role", "user:view");
        await AssignRoleToUserAsync(userId, roleId);

        Assert.False(await IsUserPermissionCachedAsync(userId));

        await WarmupUserPermissionCacheAsync(userId);

        Assert.True(await IsUserPermissionCachedAsync(userId));

        var cachedPermissions = await GetCachedUserPermissionsAsync(userId);
        Assert.NotNull(cachedPermissions);
        Assert.Contains("user:view", cachedPermissions);
    }

    /// <summary>
    /// 测试多级继承缓存失效
    /// <para>预期结果：父角色变更影响所有子角色缓存</para>
    /// </summary>
    [Fact]
    public async Task CacheInvalidation_MultiLevelInheritance_ClearsAllChildCaches() {
        await InitializeAdminTokenAsync();

        var grandParentRoleId = await CreateRoleWithPermissionAsync("cache_grandparent", "user:view");
        var parentRoleId = await CreateRoleWithInheritanceAsync("cache_parent_multi", grandParentRoleId, InheritanceType.Upward);
        var childRoleId = await CreateRoleWithInheritanceAsync("cache_child_multi", parentRoleId, InheritanceType.Upward);

        var userId = await CreateUserAsync("cache_multi@test.com", "Test123!");
        await AssignRoleToUserAsync(userId, childRoleId);

        await WarmupUserPermissionCacheAsync(userId);
        await WarmupRoleInheritedPermissionCacheAsync(childRoleId);
        await WarmupRoleInheritedPermissionCacheAsync(parentRoleId);

        Assert.True(await IsUserPermissionCachedAsync(userId));

        var permissionId = await GetPermissionIdByCodeAsync("role:view");
        await AssignPermissionToRoleAsync(grandParentRoleId, permissionId);

        Assert.False(await IsRoleInheritedPermissionCachedAsync(childRoleId));
        Assert.False(await IsRoleInheritedPermissionCachedAsync(parentRoleId));
        Assert.False(await IsUserPermissionCachedAsync(userId));
    }

    /// <summary>
    /// 测试缓存过期时间设置
    /// <para>预期结果：缓存过期时间符合预期</para>
    /// </summary>
    [Fact]
    public void CacheInvalidation_ExpirationTimes_AreCorrect() {
        Assert.Equal(TimeSpan.FromMinutes(30), CacheKeyConstants.Expiration.UserPermissions);
        Assert.Equal(TimeSpan.FromMinutes(30), CacheKeyConstants.Expiration.RolePermissions);
        Assert.Equal(TimeSpan.FromMinutes(30), CacheKeyConstants.Expiration.RoleInheritedPermissions);
        Assert.Equal(TimeSpan.FromMinutes(5), CacheKeyConstants.Expiration.UserStatus);
        Assert.Equal(TimeSpan.FromMinutes(10), CacheKeyConstants.Expiration.UserDetail);
        Assert.Equal(TimeSpan.FromMinutes(10), CacheKeyConstants.Expiration.RoleDetail);
        Assert.Equal(TimeSpan.FromMinutes(5), CacheKeyConstants.Expiration.List);
        Assert.Equal(TimeSpan.FromMinutes(5), CacheKeyConstants.Expiration.Paged);
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

        if (parentId != Guid.Empty || inheritanceType != InheritanceType.None) {
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

    private async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId) {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/role/{roleId}/permissions");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(new List<Guid> { permissionId });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId) {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/role/{roleId}/permissions");
        request.Headers.Authorization = new("Bearer", _adminToken);
        request.Content = JsonContent.Create(new List<Guid> { permissionId });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task WarmupUserPermissionCacheAsync(Guid userId) {
        using var scope = _factory.Services.CreateScope();
        var permissionCacheService = scope.ServiceProvider.GetRequiredService<Infrastructure.Shared.Services.IPermissionCacheService>();
        await permissionCacheService.WarmupUserPermissionsAsync(userId);
    }

    private async Task WarmupRoleInheritedPermissionCacheAsync(Guid roleId) {
        using var scope = _factory.Services.CreateScope();
        var permissionCacheService = scope.ServiceProvider.GetRequiredService<Infrastructure.Shared.Services.IPermissionCacheService>();
        await permissionCacheService.WarmupRoleInheritedPermissionsAsync(roleId);
    }

    private async Task<bool> IsUserPermissionCachedAsync(Guid userId) {
        using var scope = _factory.Services.CreateScope();
        var cacheProvider = scope.ServiceProvider.GetRequiredService<Infrastructure.Shared.Caches.ICacheProvider>();
        var cacheKey = CacheKeyConstants.User.Permissions(userId);
        return await cacheProvider.ExistsAsync(cacheKey);
    }

    private async Task<bool> IsRoleInheritedPermissionCachedAsync(Guid roleId) {
        using var scope = _factory.Services.CreateScope();
        var cacheProvider = scope.ServiceProvider.GetRequiredService<Infrastructure.Shared.Caches.ICacheProvider>();
        var cacheKey = CacheKeyConstants.Role.InheritedPermissions(roleId);
        return await cacheProvider.ExistsAsync(cacheKey);
    }

    private async Task<HashSet<string>?> GetCachedUserPermissionsAsync(Guid userId) {
        using var scope = _factory.Services.CreateScope();
        var cacheProvider = scope.ServiceProvider.GetRequiredService<Infrastructure.Shared.Caches.ICacheProvider>();
        var cacheKey = CacheKeyConstants.User.Permissions(userId);
        return await cacheProvider.GetAsync<HashSet<string>>(cacheKey);
    }

    private async Task<List<string>> GetUserPermissionsAsync(Guid userId) {
        using var scope = _factory.Services.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<Domain.Services.IPermissionDomainService>();
        return await permissionService.GetUserPermissionCodesAsync(userId);
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
