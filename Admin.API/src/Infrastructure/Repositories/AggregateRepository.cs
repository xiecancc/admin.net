/*
 * 文件名称: AggregateRepository.cs
 * 功能描述: 聚合根仓储实现，继承 DomainRepository 并添加软删除支持
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Domain.Shared.Entities;
using Domain.Shared.Events;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Contexts;
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
/// <para>异常处理：所有操作失败时抛出异常，由调用方处理</para>
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

    /// <inheritdoc/>
    public override async Task<bool> InsertAsync(List<TAggregate> entities, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities.Count == 0) {
            return false;
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
        catch (Exception ex) {
            _logger.LogError(ex, "插入实体失败: {EntityName}", _entityName);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> UpdateAsync(List<TAggregate> entities, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        var validEntities = entities.Where(e => e.Id != Guid.Empty).ToList();

        if (validEntities.Count == 0) {
            return false;
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
        catch (Exception ex) {
            _logger.LogError(ex, "更新实体失败: {EntityName}", _entityName);
            throw;
        }
    }

    /// <inheritdoc/>
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
        catch (Exception ex) {
            _logger.LogError(ex, "软删除实体失败: {EntityName}", _entityName);
            throw;
        }
    }

    /// <inheritdoc/>
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
        catch (Exception ex) {
            _logger.LogError(ex, "恢复实体失败: {EntityName}", _entityName);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<TAggregate?> GetAsync(Guid id, CancellationToken cancellationToken = default) {
        try {
            return await GetAsync(t => t.Id == id, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "根据ID获取实体失败: {EntityName}, ID: {Id}", _entityName, id);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        var validIds = ids.Where(id => id != Guid.Empty).ToList();

        if (validIds.Count == 0) {
            return false;
        }

        try {
            _logger.LogInformation("开始根据ID列表软删除实体: {EntityName}, 数量: {Count}", _entityName, validIds.Count);
            var result = await DeleteAsync(t => validIds.Contains(t.Id), cancellationToken);
            _logger.LogInformation("根据ID列表软删除实体成功: {EntityName}, 结果: {Result}", _entityName, result);
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "根据ID列表软删除实体失败: {EntityName}", _entityName);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<bool> RestoreAsync(List<Guid> ids, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        var validIds = ids.Where(id => id != Guid.Empty).ToList();

        if (validIds.Count == 0) {
            return false;
        }

        try {
            _logger.LogInformation("开始根据ID列表恢复实体: {EntityName}, 数量: {Count}", _entityName, validIds.Count);
            var result = await RestoreAsync(t => validIds.Contains(t.Id), cancellationToken);
            _logger.LogInformation("根据ID列表恢复实体成功: {EntityName}, 结果: {Result}", _entityName, result);
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "根据ID列表恢复实体失败: {EntityName}", _entityName);
            throw;
        }
    }
}
