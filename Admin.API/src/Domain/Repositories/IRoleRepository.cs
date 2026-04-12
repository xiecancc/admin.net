/*
 * 文件名称: IRoleRepository.cs
 * 功能描述: 角色仓储接口，定义角色相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 角色仓储接口
/// <para>定义角色相关的数据访问操作</para>
/// </summary>
/// <remarks>
/// <para>继承自 IAggregateTreeRepository&lt;Role&gt;，提供聚合根和树形结构的基本操作</para>
/// <para>扩展功能：</para>
/// <list type="bullet">
///   <item>根据编码查找角色</item>
///   <item>检查角色编码是否存在</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 在应用服务中使用
/// public class RoleAppService
/// {
///     private readonly IRoleRepository _roleRepository;
///     
///     public RoleAppService(IRoleRepository roleRepository)
///     {
///         _roleRepository = roleRepository;
///     }
///     
///     public async Task&lt;Role?&gt; GetRoleByCodeAsync(string code)
///     {
///         return await _roleRepository.FindByCodeAsync(code);
///     }
///     
///     public async Task&lt;bool&gt; IsCodeUniqueAsync(string code)
///     {
///         return !await _roleRepository.IsCodeExistsAsync(code);
///     }
/// }
/// </code>
/// </example>
/// <inheritdoc/>
public interface IRoleRepository : IAggregateTreeRepository<Role> {
    /// <summary>
    /// 根据编码查找角色
    /// </summary>
    /// <param name="code">角色编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色实体，不存在则返回 null</returns>
    Task<Role?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查角色编码是否存在
    /// </summary>
    /// <param name="code">角色编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检测角色继承是否存在循环引用
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="parentId">要设置的父角色ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在循环引用</returns>
    /// <exception cref="ArgumentException">当 roleId 或 parentId 为空时抛出</exception>
    /// <remarks>
    /// <para>循环引用检测算法：</para>
    /// <list type="number">
    ///   <item>检查角色ID是否与父角色ID相同</item>
    ///   <item>遍历父角色的继承链，检查是否包含当前角色</item>
    ///   <item>如果找到当前角色，则存在循环引用</item>
    /// </list>
    /// </remarks>
    Task<bool> HasInheritanceCycleAsync(Guid roleId, Guid parentId, CancellationToken cancellationToken = default);
}
