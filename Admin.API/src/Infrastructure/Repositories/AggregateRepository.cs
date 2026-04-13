/*
 * 文件名称: AggregateRepository.cs
 * 功能描述: 聚合根仓储实现，继承 DomainRepository 并添加软删除支持
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Shared.Entities;
using Domain.Shared.Events;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Contexts;
using Domain.Shared.Utils;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IAggregateRepository{TAggregate}"/>
/// <remarks>
/// <para>聚合根仓储负责：</para>
/// <list type="bullet">
///   <item> 提供对聚合根实体的持久化操作 </item>
///   <item> 实现聚合根的软删除和恢复功能 </item>
///   <item> 支持基于查询参数的复杂查询 </item>
///   <item> 维护聚合的完整性和一致性 </item>
/// </list>
/// <para>持久化策略：使用 SqlSugar ORM 框架进行数据库操作</para>
/// <para>软删除策略：通过 IsDeleted 字段标记删除状态，保留数据记录</para>
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentNullException、ArgumentException、ArgumentOutOfRangeException </item>
///   <item> 实体未找到：KeyNotFoundException（当按 ID 查询时）</item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
///   <item> 唯一键冲突：InvalidOperationException 并说明重复字段 </item>
///   <item> 外键约束：InvalidOperationException 并说明关联关系 </item>
///   <item> 连接异常：保留原始异常并添加上下文信息 </item>
/// </list>
/// </remarks>
/// <typeparam name="TAggregate">聚合根类型，必须继承自 AggregateBase</typeparam>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="entityName">实体名称，用于日志记录</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class AggregateRepository<TAggregate>(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    string entityName,
    ILogger<AggregateRepository<TAggregate>> logger)
    : DomainRepository<TAggregate>(client, eventBus, userContextProvider, entityName, logger), IAggregateRepository<TAggregate>
    where TAggregate : AggregateBase, new() {

    /// <summary>
    /// 批量插入聚合根实体，自动填充创建审计信息
    /// </summary>
    /// <param name="entities">要插入的实体列表，不能为 null 或空集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>插入成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentNullException">当 entities 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当 entities 为空集合时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出，包括唯一键冲突、外键约束等</exception>
    public override async Task<bool> InsertAsync(List<TAggregate> entities, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities.Count == 0) {
            throw new ArgumentException("实体列表不能为空", nameof(entities));
        }

        try {
            var now = DateTime.UtcNow;
            var userId = _userContextProvider.UserId;
            entities.ForEach(e => {
                e.CreatedAt = now;
                if (userId.HasValue) {
                    e.CreatedBy = userId.Value;
                }
            });
            var count = await _client.Insertable(entities.ToArray()).ExecuteCommandAsync(cancellationToken);

            if (count > 0) {
                var createdEvent = new DomainCreatedEvent<TAggregate>(entities, _entityName);
                await _eventBus.PublishAsync(createdEvent, cancellationToken);
            }

            return count > 0;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "插入实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "插入", _entityName);
        }
    }

    /// <summary>
    /// 批量更新聚合根实体，自动填充更新审计信息
    /// </summary>
    /// <param name="entities">要更新的实体列表，不能为 null 或空集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentNullException">当 entities 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当 entities 为空集合或所有实体 ID 都为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出，包括唯一键冲突、外键约束等</exception>
    public virtual async Task<bool> UpdateAsync(List<TAggregate> entities, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        var validEntities = entities.Where(e => e.Id != Guid.Empty).ToList();

        if (validEntities.Count == 0) {
            throw new ArgumentException("实体列表中必须包含有效的 ID", nameof(entities));
        }

        try {
            var now = DateTime.UtcNow;
            var userId = _userContextProvider.UserId;
            validEntities.ForEach(e => {
                e.UpdatedAt = now;
                if (userId.HasValue) {
                    e.UpdatedBy = userId.Value;
                }
            });
            var count = await _client.Updateable(validEntities.ToArray()).ExecuteCommandAsync(cancellationToken);

            if (count > 0) {
                var updatedEvent = new DomainUpdatedEvent<TAggregate>(validEntities, _entityName);
                await _eventBus.PublishAsync(updatedEvent, cancellationToken);
            }

            return count > 0;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "更新实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "更新", _entityName);
        }
    }

    /// <summary>
    /// 根据条件软删除聚合根实体，自动填充删除审计信息
    /// </summary>
    /// <param name="predicate">删除条件表达式，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>软删除成功返回 true，未找到匹配实体返回 false</returns>
    /// <exception cref="ArgumentNullException">当 predicate 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public override async Task<bool> DeleteAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        try {
            var entities = await _client.Queryable<TAggregate>()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            if (entities.Count == 0) {
                return false;
            }

            var now = DateTime.UtcNow;
            var userId = _userContextProvider.UserId;
            entities.ForEach(e => {
                e.IsDeleted = true;
                e.DeletedAt = now;
                e.DeletedBy = userId;
            });

            var count = await _client.Updateable(entities.ToArray())
                .UpdateColumns(t => new { t.IsDeleted, t.DeletedAt, t.DeletedBy })
                .ExecuteCommandAsync(cancellationToken);

            if (count > 0) {
                var deleteEvent = new DomainDeletedEvent<TAggregate>(entities, _entityName);
                await _eventBus.PublishAsync(deleteEvent, cancellationToken);
            }

            return count > 0;
        }
        catch (Exception ex) when (ex is not ArgumentNullException) {
            _logger.LogError(ex, "软删除实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "软删除", _entityName);
        }
    }

    /// <summary>
    /// 根据条件恢复已软删除的聚合根实体
    /// </summary>
    /// <param name="predicate">恢复条件表达式，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>恢复成功返回 true，未找到匹配实体返回 false</returns>
    /// <exception cref="ArgumentNullException">当 predicate 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<bool> RestoreAsync(Expression<Func<TAggregate, bool>> predicate, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        try {
            var entities = await _client.Queryable<TAggregate>()
                .Where(predicate)
                .Where(t => t.IsDeleted)
                .ToListAsync(cancellationToken);

            if (entities.Count == 0) {
                return false;
            }

            var now = DateTime.UtcNow;
            var userId = _userContextProvider.UserId;
            entities.ForEach(e => {
                e.IsDeleted = false;
                e.DeletedAt = null;
                e.DeletedBy = null;
                e.UpdatedAt = now;
                e.UpdatedBy = userId;
            });

            var count = await _client.Updateable(entities.ToArray())
                .UpdateColumns(t => new { t.IsDeleted, t.DeletedAt, t.DeletedBy, t.UpdatedAt, t.UpdatedBy })
                .ExecuteCommandAsync(cancellationToken);

            if (count > 0) {
                var restoreEvent = new DomainRestoredEvent<TAggregate>(entities, _entityName);
                await _eventBus.PublishAsync(restoreEvent, cancellationToken);
            }

            return count > 0;
        }
        catch (Exception ex) when (ex is not ArgumentNullException) {
            _logger.LogError(ex, "恢复实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "恢复", _entityName);
        }
    }

    /// <summary>
    /// 根据 ID 获取聚合根实体
    /// </summary>
    /// <param name="id">实体 ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>找到的实体，不存在则返回 null</returns>
    /// <exception cref="ArgumentException">当 id 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<TAggregate?> GetAsync(Guid id, CancellationToken cancellationToken = default) {
        if (id == Guid.Empty) {
            throw new ArgumentException("ID 不能为空", nameof(id));
        }

        try {
            return await GetAsync([t => t.Id == id], cancellationToken);
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "根据ID获取实体失败: {EntityName}, ID: {Id}", _entityName, id);
            throw RepositoryExceptionHelper.HandleException(ex, "获取", _entityName);
        }
    }

    /// <summary>
    /// 根据 ID 列表批量软删除聚合根实体
    /// </summary>
    /// <param name="ids">实体 ID 列表，不能为 null 或空集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>软删除成功返回 true，未找到匹配实体返回 false</returns>
    /// <exception cref="ArgumentNullException">当 ids 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当 ids 为空集合或所有 ID 都为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<bool> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        if (ids.Count == 0) {
            throw new ArgumentException("ID 列表不能为空", nameof(ids));
        }

        var validIds = ids.Where(id => id != Guid.Empty).ToList();

        if (validIds.Count == 0) {
            throw new ArgumentException("ID 列表中必须包含有效的 ID", nameof(ids));
        }

        try {
            _logger.LogInformation("开始根据ID列表软删除实体: {EntityName}, 数量: {Count}", _entityName, validIds.Count);
            var result = await DeleteAsync(t => validIds.Contains(t.Id), cancellationToken);
            _logger.LogInformation("根据ID列表软删除实体成功: {EntityName}, 结果: {Result}", _entityName, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "根据ID列表软删除实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "批量软删除", _entityName);
        }
    }

    /// <summary>
    /// 根据 ID 列表批量恢复已软删除的聚合根实体
    /// </summary>
    /// <param name="ids">实体 ID 列表，不能为 null 或空集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>恢复成功返回 true，未找到匹配实体返回 false</returns>
    /// <exception cref="ArgumentNullException">当 ids 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当 ids 为空集合或所有 ID 都为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<bool> RestoreAsync(List<Guid> ids, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        if (ids.Count == 0) {
            throw new ArgumentException("ID 列表不能为空", nameof(ids));
        }

        var validIds = ids.Where(id => id != Guid.Empty).ToList();

        if (validIds.Count == 0) {
            throw new ArgumentException("ID 列表中必须包含有效的 ID", nameof(ids));
        }

        try {
            _logger.LogInformation("开始根据ID列表恢复实体: {EntityName}, 数量: {Count}", _entityName, validIds.Count);
            var result = await RestoreAsync(t => validIds.Contains(t.Id), cancellationToken);
            _logger.LogInformation("根据ID列表恢复实体成功: {EntityName}, 结果: {Result}", _entityName, result);
            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "根据ID列表恢复实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "批量恢复", _entityName);
        }
    }
}
