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
}
