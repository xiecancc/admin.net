/*
 * 文件名称: IntegrationTestBase.cs
 * 功能描述: 集成测试基类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using System.Net.Http.Json;
using System.Text.Json;
using Application.Contracts.Dtos;
using Domain.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace API.Test.Integration;

/// <summary>
/// 集成测试基类
/// <para>提供通用的测试辅助方法，用于创建测试数据、登录用户等操作</para>
/// </summary>
public abstract class IntegrationTestBase : IAsyncDisposable {
    protected readonly HttpClient Client;
    protected readonly TestWebApplicationFactory Factory;
    protected readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly List<Guid> _createdUserIds = [];
    private readonly List<Guid> _createdRoleIds = [];
    private string? _adminToken;
    private Guid _adminUserId;

    protected IntegrationTestBase(TestWebApplicationFactory factory) {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async virtual ValueTask DisposeAsync() {
        await CleanupTestDataAsync();
        Client.Dispose();
        await Task.Delay(100);
    }

    /// <summary>
    /// 获取管理员令牌
    /// </summary>
    protected async Task<string> GetAdminTokenAsync() {
        if (_adminToken != null) {
            return _adminToken;
        }

        var loginDto = new {
            Email = "admin@admin.com",
            Password = "Admin123!"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
            _adminToken = result.GetProperty("token").GetString();

            using var scope = Factory.Services.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<Domain.Repositories.IUserRepository>();
            var user = await userRepository.FindByEmailAsync("admin@admin.com");
            _adminUserId = user?.Id ?? Guid.Empty;
        }

        return _adminToken ?? throw new InvalidOperationException("无法获取管理员令牌");
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    protected async Task<Guid> CreateUserAsync(string email, string password, string? nickName = null) {
        var token = await GetAdminTokenAsync();

        var dto = new UserCreateDto {
            Email = email,
            Password = password,
            NickName = nickName ?? email.Split('@')[0]
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/user");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(dto);

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var userId = await GetUserIdByEmailAsync(email);
        _createdUserIds.Add(userId);
        return userId;
    }

    /// <summary>
    /// 根据邮箱获取用户ID
    /// </summary>
    protected async Task<Guid> GetUserIdByEmailAsync(string email) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/user?email={email}");
        request.Headers.Authorization = new("Bearer", token);

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);

        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            var user = data.EnumerateArray().FirstOrDefault();
            if (user.TryGetProperty("id", out var userIdProp)) {
                return userIdProp.GetGuid();
            }
        }

        throw new InvalidOperationException($"无法找到用户: {email}");
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    protected async Task<Guid> CreateRoleAsync(string code, string? name = null) {
        var token = await GetAdminTokenAsync();

        var dto = new RoleCreateDto {
            Code = code,
            Name = name ?? code
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/role");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(dto);

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var roleId = await GetRoleIdByCodeAsync(code);
        _createdRoleIds.Add(roleId);
        return roleId;
    }

    /// <summary>
    /// 创建角色并设置继承关系
    /// </summary>
    protected async Task<Guid> CreateRoleWithInheritanceAsync(string code, Guid? parentId, InheritanceType inheritanceType) {
        var roleId = await CreateRoleAsync(code);

        if (parentId.HasValue || inheritanceType != InheritanceType.None) {
            await SetRoleInheritanceAsync(roleId, parentId, inheritanceType);
        }

        return roleId;
    }

    /// <summary>
    /// 设置角色继承关系（直接操作数据库）
    /// </summary>
    protected async Task SetRoleInheritanceAsync(Guid roleId, Guid? parentId, InheritanceType inheritanceType) {
        using var scope = Factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

        await client.Updateable<Domain.Entities.Role>()
            .SetColumns(r => new Domain.Entities.Role {
                ParentId = parentId,
                InheritanceType = inheritanceType
            })
            .Where(r => r.Id == roleId)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据编码获取角色ID
    /// </summary>
    protected async Task<Guid> GetRoleIdByCodeAsync(string code) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/role?code={code}");
        request.Headers.Authorization = new("Bearer", token);

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);

        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            var role = data.EnumerateArray().FirstOrDefault();
            if (role.TryGetProperty("id", out var roleIdProp)) {
                return roleIdProp.GetGuid();
            }
        }

        throw new InvalidOperationException($"无法找到角色: {code}");
    }

    /// <summary>
    /// 创建带权限的角色
    /// </summary>
    protected async Task<Guid> CreateRoleWithPermissionAsync(string code, string permissionCode) {
        var roleId = await CreateRoleAsync(code);
        var permissionId = await GetPermissionIdByCodeAsync(permissionCode);
        await AssignPermissionToRoleAsync(roleId, permissionId);
        return roleId;
    }

    /// <summary>
    /// 根据编码获取权限ID
    /// </summary>
    protected async Task<Guid> GetPermissionIdByCodeAsync(string code) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/permission");
        request.Headers.Authorization = new("Bearer", token);

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);

        if (result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array) {
            foreach (var permission in data.EnumerateArray()) {
                if (permission.TryGetProperty("code", out var codeProp) && codeProp.GetString() == code) {
                    return permission.GetProperty("id").GetGuid();
                }
            }
        }

        throw new InvalidOperationException($"无法找到权限: {code}");
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    protected async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/role/{roleId}/permissions");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(new List<Guid> { permissionId });

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 从角色移除权限
    /// </summary>
    protected async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/role/{roleId}/permissions");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(new List<Guid> { permissionId });

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    protected async Task AssignRoleToUserAsync(Guid userId, Guid roleId) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/user/{userId}/roles");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(new List<Guid> { roleId });

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 从用户移除角色
    /// </summary>
    protected async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId) {
        var token = await GetAdminTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/user/{userId}/roles");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(new List<Guid> { roleId });

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    protected async Task<string> LoginUserAsync(string email, string password) {
        var loginDto = new {
            Email = email,
            Password = password
        };

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        return result.GetProperty("token").GetString() ?? throw new InvalidOperationException("登录失败");
    }

    /// <summary>
    /// 创建用户并登录
    /// </summary>
    protected async Task<(Guid UserId, string Token)> CreateUserAndLoginAsync(string email, string password) {
        var userId = await CreateUserAsync(email, password);
        var token = await LoginUserAsync(email, password);
        return (userId, token);
    }

    /// <summary>
    /// 清理测试数据
    /// </summary>
#pragma warning disable CA1031 // 需要捕获所有异常以确保清理不会中断
    protected async Task CleanupTestDataAsync() {
        if (_adminToken == null) {
            return;
        }

        foreach (var userId in _createdUserIds) {
            try {
                using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/user");
                request.Headers.Authorization = new("Bearer", _adminToken);
                request.Content = JsonContent.Create(new List<Guid> { userId });
                await Client.SendAsync(request);
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
                await Client.SendAsync(request);
            }
            catch {
                // Ignore cleanup errors
            }
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// 获取服务
    /// </summary>
    protected T GetService<T>() where T : notnull {
        using var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}
