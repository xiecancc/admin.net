/*
 * 文件名称: IDomainRepository.cs
 * 功能描述: 领域仓储接口，支持所有实体（包括关系表）的基本操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-31
 */

using Domain.Shared.Entities;
using System.Linq.Expressions;

namespace Domain.Shared.Repositories;

/// <summary>
/// 领域仓储接口
/// <para>支持所有实体（包括聚合根和关系表）的基本操作</para>
/// <para>注意：此接口的删除操作为物理删除</para>
/// <para>所有写操作返回成功数量，失败时抛出异常</para>
/// </summary>
/// <remarks>
/// <para>仓储负责：</para>
/// <list type="bullet">
///   <item> 提供对领域实体的持久化操作 </item>
///   <item> 实现领域对象的检索 </item>
///   <item> 支持基本的CRUD操作 </item>
///   <item> 提供分页和投影功能 </item>
/// </list>
/// </remarks>
/// <typeparam name="TDomain">领域模型类型，必须继承自 DomainBase</typeparam>
public interface IDomainRepository<TDomain> : IRepository<TDomain> where TDomain : DomainBase, new() {
    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <param name="entities">要插入的实体列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当 entities 为 null 时抛出</exception>
    Task<bool> InsertAsync(List<TDomain> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量物理删除实体
    /// </summary>
    /// <param name="predicate">删除条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当 predicate 为 null 时抛出</exception>
    Task<bool> DeleteAsync(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken = default);
}
