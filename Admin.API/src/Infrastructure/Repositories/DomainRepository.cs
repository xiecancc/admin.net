/*
 * 文件名称: DomainRepository.cs
 * 功能描述: 领域仓储实现，支持所有实体（包括聚合根和关系表）的基本操作
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
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentNullException、ArgumentException </item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
///   <item> 唯一键冲突：InvalidOperationException 并说明重复字段 </item>
///   <item> 外键约束：InvalidOperationException 并说明关联关系 </item>
/// </list>
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

    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <param name="entities">要插入的实体列表，不能为 null 或空集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>插入成功返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentNullException">当 entities 为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当 entities 为空集合时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出，包括唯一键冲突、外键约束等</exception>
    public virtual async Task<bool> InsertAsync(List<TDomain> entities, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        if (entities.Count == 0) {
            throw new ArgumentException("实体列表不能为空", nameof(entities));
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
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "插入实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "插入", _entityName);
        }
    }

    /// <summary>
    /// 根据条件物理删除实体
    /// </summary>
    /// <param name="predicate">删除条件表达式，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除成功返回 true，未找到匹配实体返回 false</returns>
    /// <exception cref="ArgumentNullException">当 predicate 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出，包括外键约束冲突等</exception>
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
        catch (Exception ex) when (ex is not ArgumentNullException) {
            _logger.LogError(ex, "删除实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "删除", _entityName);
        }
    }
}
