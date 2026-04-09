using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Shared.Contexts;

/// <summary>
/// 用户上下文提供者接口，用于获取当前用户的信息和验证权限
/// </summary>
public interface IUserContextProvider {
    /// <summary>
    /// 当前用户 ID（已认证用户，未认证返回 null）
    /// </summary>
    Guid? UserId {
        get;
    }

    /// <summary>
    /// 是否已认证
    /// </summary>
    bool IsAuthenticated {
        get;
    }

    /// <summary>
    /// 检查当前用户是否拥有指定权限
    /// </summary>
    /// <param name="permissionCode">权限码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有权限</returns>
    Task<bool> HasPermissionAsync(string permissionCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查当前用户是否拥有多个权限（OR 逻辑）
    /// </summary>
    /// <param name="permissionCodes">权限码数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有任意一个权限</returns>
    Task<bool> HasAnyPermissionAsync(string[] permissionCodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查当前用户是否拥有多个权限（AND 逻辑）
    /// </summary>
    /// <param name="permissionCodes">权限码数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有所有权限</returns>
    Task<bool> HasAllPermissionsAsync(string[] permissionCodes, CancellationToken cancellationToken = default);
}