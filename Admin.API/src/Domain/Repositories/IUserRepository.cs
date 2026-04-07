/*
 * 文件名称: IUserRepository.cs
 * 功能描述: 用户仓储接口，定义用户相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 用户仓储接口
/// <para>定义用户相关的数据访问操作</para>
/// </summary>
/// <remarks>
/// <para>继承自 IAggregateRepository&lt;User&gt;，提供聚合根的基本操作</para>
/// <para>扩展功能：</para>
/// <list type="bullet">
///   <item>根据邮箱查找用户</item>
///   <item>检查邮箱是否存在</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 在应用服务中使用
/// public class UserAppService
/// {
///     private readonly IUserRepository _userRepository;
///     
///     public UserAppService(IUserRepository userRepository)
///     {
///         _userRepository = userRepository;
///     }
///     
///     public async Task&lt;User?&gt; GetUserByEmailAsync(string email)
///     {
///         return await _userRepository.FindByEmailAsync(email);
///     }
///     
///     public async Task&lt;bool&gt; IsEmailUniqueAsync(string email)
///     {
///         return !await _userRepository.IsEmailExistsAsync(email);
///     }
/// }
/// </code>
/// </example>
public interface IUserRepository : IAggregateRepository<User> {
    /// <summary>
    /// 根据邮箱查找用户
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户实体，不存在则返回 null</returns>
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查邮箱是否存在
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
