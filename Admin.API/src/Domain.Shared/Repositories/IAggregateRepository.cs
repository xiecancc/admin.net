/*
 * 文件名称: IAggregateRepository.cs
 * 功能描述: 聚合根仓储接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-31
 */

using Domain.Shared.Entities;
using System.Linq.Expressions;

namespace Domain.Shared.Repositories;

/// <summary>
/// 聚合根仓储接口
/// <para>仅适用于聚合根实体，关系表使用 IDomainRepository</para>
/// <para>继承 IDomainRepository，添加聚合根特有的软删除和恢复操作</para>
/// <para>所有写操作返回成功数量，失败时抛出异常</para>
/// </summary>
/// <remarks>
/// <para>聚合根仓储负责：</para>
/// <list type="bullet">
///   <item> 提供对聚合根实体的持久化操作 </item>
///   <item> 实现聚合根的软删除和恢复功能 </item>
///   <item> 支持基于查询参数的复杂查询 </item>
///   <item> 维护聚合的完整性和一致性 </item>
/// </list>
/// </remarks>
/// <typeparam name="TAggregate">聚合根类型，必须继承自 AggregateBase</typeparam>
public interface IAggregateRepository<TAggregate> : IDomainRepository<TAggregate> where TAggregate : AggregateBase, new() {
    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">要更新的实体列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当 entities 为 null 时抛出</exception>
    Task<bool> UpdateAsync(List<TAggregate> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量恢复已软删除的实体
    /// </summary>
    /// <param name="predicate">恢复条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当 predicate 为 null 时抛出</exception>
    Task<bool> RestoreAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID获取单个实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体，不存在则返回 null</returns>
    Task<TAggregate?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID列表批量软删除实体
    /// </summary>
    /// <param name="ids">要删除的实体ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当 ids 为 null 时抛出</exception>
    Task<bool> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据ID列表批量恢复已软删除的实体
    /// </summary>
    /// <param name="ids">要恢复的实体ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当 ids 为 null 时抛出</exception>
    Task<bool> RestoreAsync(List<Guid> ids, CancellationToken cancellationToken = default);
}
