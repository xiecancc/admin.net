/*
 * 文件名称: IUserRoleRepository.cs
 * 功能描述: 用户角色关联仓储接口，用于处理用户和角色之间的关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Domain.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 用户角色关联仓储接口
/// <para>用于处理用户和角色之间的多对多关联关系</para>
/// </summary>
/// <example>
/// <code>
/// // 在应用服务中使用
/// public class UserRoleAppService
/// {
///     private readonly IUserRoleRepository _userRoleRepository;
///     
///     public UserRoleAppService(IUserRoleRepository userRoleRepository)
///     {
///         _userRoleRepository = userRoleRepository;
///     }
///     
///     public async Task AssignRolesToUserAsync(Guid userId, List&lt;Guid&gt; roleIds)
///     {
///         await _userRoleRepository.AssignRolesToUserAsync(userId, roleIds);
///     }
///     
///     public async Task&lt;List&lt;Guid&gt;&gt; GetUserRoleIdsAsync(Guid userId)
///     {
///         return await _userRoleRepository.GetRoleIdsByUserIdAsync(userId);
///     }
/// }
/// </code>
/// </example>
public interface IUserRoleRepository : IDomainRepository<UserRole> {
    /// <summary>
    /// 根据用户 ID 获取角色 ID 列表
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色 ID 列表</returns>
    Task<List<Guid>> GetRoleIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据角色 ID 获取用户 ID 列表
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户 ID 列表</returns>
    Task<List<Guid>> GetUserIdsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="roleIds">角色 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="userIds">用户 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> AssignUsersToRoleAsync(Guid roleId, List<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除用户的角色
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="roleIds">角色 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveRolesFromUserAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除角色的用户
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <param name="userIds">用户 ID 列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveUsersFromRoleAsync(Guid roleId, List<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查用户是否拥有指定角色
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="roleId">角色 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有</returns>
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户的角色编码列表
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色编码列表</returns>
    /// <remarks>
    /// 关联查询：UserRole → Role，返回角色的 Code 列表
    /// </remarks>
    Task<List<string>> GetUserRoleCodesAsync(Guid userId, CancellationToken cancellationToken = default);
}
