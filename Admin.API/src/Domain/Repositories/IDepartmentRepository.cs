/*
 * 文件名称: IDepartmentRepository.cs
 * 功能描述: 部门仓储接口，定义部门相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Domain.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 部门仓储接口
/// <para>定义部门相关的数据访问操作</para>
/// </summary>
/// <remarks>
/// <para>继承自 IAggregateTreeRepository&lt;Department&gt;，提供树形结构的基本操作</para>
/// <para>扩展功能：</para>
/// <list type="bullet">
///   <item>根据编码查找部门</item>
///   <item>检查编码是否存在</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 在应用服务中使用
/// public class DepartmentAppService
/// {
///     private readonly IDepartmentRepository _departmentRepository;
///     
///     public DepartmentAppService(IDepartmentRepository departmentRepository)
///     {
///         _departmentRepository = departmentRepository;
///     }
///     
///     public async Task&lt;Department?&gt; GetDepartmentByCodeAsync(string code)
///     {
///         return await _departmentRepository.FindByCodeAsync(code);
///     }
///     
///     public async Task&lt;bool&gt; IsCodeUniqueAsync(string code)
///     {
///         return !await _departmentRepository.IsCodeExistsAsync(code);
///     }
/// }
/// </code>
/// </example>
public interface IDepartmentRepository : IAggregateTreeRepository<Department> {
    /// <summary>
    /// 根据编码查找部门
    /// </summary>
    /// <param name="code">部门编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门实体，不存在则返回 null</returns>
    Task<Department?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查编码是否存在
    /// </summary>
    /// <param name="code">部门编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default);
}