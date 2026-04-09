/*
 * 文件名称: DomainRepository.cs
 * 功能描述: 领域仓储实现，支持所有实体（包括聚合根和关系表）的基本操作
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

/// <summary>
/// 领域仓储实现，支持所有实体（包括聚合根和关系表）的基本操作
/// <para>注意：此类的删除操作为物理删除</para>
/// <para>所有写操作返回成功数量，失败时抛出异常</para>
/// </summary>
/// <remarks>
/// <para>仓储实现负责：</para>
/// <list type="bullet">
///   <item> 提供对领域实体的持久化操作 </item>
///   <item> 实现领域对象的检索 </item>
///   <item> 支持基本的CRUD操作 </item>
///   <item> 提供分页和投影功能 </item>
/// </list>
/// <para>持久化策略：使用 SqlSugar ORM 框架进行数据库操作</para>
/// <para>异常处理：所有操作失败时抛出异常，由调用方处理</para>
/// </remarks>
/// <typeparam name="TDomain">领域模型类型，必须继承自 DomainBase</typeparam>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
    /// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
    /// <param name="entityName">实体名称，用于日志记录</param>
    /// <param name="logger">日志记录器，用于记录操作日志</param>
public class DomainRepository<TDomain>(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    string entityName,
    ILogger<DomainRepository<TDomain>> logger)
    : Repository<TDomain>(client, eventBus, userContextProvider, entityName, logger as ILogger<Repository<TDomain>>), IDomainRepository<TDomain>
    where TDomain : DomainBase, new() {

    /// <inheritdoc/>
    public virtual async Task<bool> InsertAsync(List<TDomain> entities, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities.Count == 0) {
            return false;
        }

        try {
            _logger.LogInformation("开始插入实体: {EntityName}, 数量: {Count}", _entityName, entities.Count);
            var count = await _client.Insertable(entities.ToArray()).ExecuteCommandAsync(cancellationToken);
            _logger.LogInformation("插入实体成功: {EntityName}, 数量: {Count}", _entityName, count);

            if (count > 0) {
                var createdEvent = new DomainCreatedEvent<TDomain>(entities, _entityName);
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
    public virtual async Task<bool> DeleteAsync(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        try {
            _logger.LogInformation("开始删除实体: {EntityName}", _entityName);

            var entities = await _client.Queryable<TDomain>()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            if (entities.Count == 0) {
                _logger.LogInformation("未找到符合条件的实体: {EntityName}", _entityName);
                return false;
            }

            var count = await _client.Deleteable<TDomain>().Where(predicate).ExecuteCommandAsync(cancellationToken);
            _logger.LogInformation("删除实体成功: {EntityName}, 数量: {Count}", _entityName, count);

            if (count > 0) {
                var deleteEvent = new DomainDeletedEvent<TDomain>(entities, _entityName);
                await _eventBus.PublishAsync(deleteEvent, cancellationToken);
            }

            return count > 0;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "删除实体失败: {EntityName}", _entityName);
            throw;
        }
    }
}
