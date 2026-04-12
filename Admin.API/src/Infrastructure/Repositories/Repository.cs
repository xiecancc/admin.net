/*
 * 文件名称: Repository.cs
 * 功能描述: 泛型仓储实现，提供通用的实体操作和依赖注入
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Shared.Dtos;
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
/// 泛型仓储基类
/// <para>实现泛型仓储接口，提供通用的实体操作和依赖注入</para>
/// </summary>
/// <remarks>
/// <para>泛型仓储负责：</para>
/// <list type="bullet">
///   <item> 提供共享的依赖项注入 </item>
///   <item> 封装通用的数据库操作 </item>
///   <item> 提供统一的日志记录 </item>
///   <item> 支持领域事件发布 </item>
///   <item> 实现泛型仓储接口 </item>
///   <item> 提供参数构建功能 </item>
/// </list>
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentNullException、ArgumentException、ArgumentOutOfRangeException </item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
///   <item> 连接异常：保留原始异常并添加上下文信息 </item>
/// </list>
/// </remarks>
/// <typeparam name="TDomain">领域模型类型，必须继承自DomainBase</typeparam>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="entityName">实体名称，用于日志记录</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class Repository<TDomain>(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    string entityName,
    ILogger<Repository<TDomain>> logger)
    : IRepository<TDomain>
    where TDomain : DomainBase, new() {
    /// <summary>
    /// SqlSugar 客户端，用于数据库操作
    /// </summary>
    protected ISqlSugarClient _client { get; init; } = client;

    /// <summary>
    /// 领域事件总线，用于发布领域事件
    /// </summary>
    protected IDomainEventBus _eventBus { get; init; } = eventBus;

    /// <summary>
    /// 用户上下文提供者，用于获取当前用户信息
    /// </summary>
    protected IUserContextProvider _userContextProvider { get; init; } = userContextProvider;

    /// <summary>
    /// 实体名称
    /// </summary>
    protected string _entityName { get; init; } = entityName;

    /// <summary>
    /// 日志记录器，用于记录操作日志
    /// </summary>
    protected ILogger<Repository<TDomain>> _logger { get; init; } = logger;

    /// <summary>
    /// 构建查询参数
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，必须大于 0，默认值为 1</param>
    /// <param name="size">每页大小，必须大于 0，默认值为 10</param>
    /// <returns>构建的参数对象，包含查询对象和分页参数</returns>
    /// <exception cref="ArgumentOutOfRangeException">当页码或每页大小小于等于 0 时抛出</exception>
    protected (ISugarQueryable<TDomain>, (int, int)) BuildQueryable(
        List<Expression<Func<TDomain, bool>>>? predicates = null,
        IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null,
        int page = 1, int size = 10) {
        if (page <= 0) {
            throw new ArgumentOutOfRangeException(nameof(page), page, "页码必须大于 0");
        }

        if (size <= 0) {
            throw new ArgumentOutOfRangeException(nameof(size), size, "每页大小必须大于 0");
        }

        var queryable = _client.Queryable<TDomain>();
        if (predicates != null) {
            foreach (var predicate in predicates) {
                queryable = queryable.Where(predicate);
            }
        }
        if (orders != null) {
            foreach (var order in orders) {
                var sugarOrderType = order.Value ? OrderByType.Desc : OrderByType.Asc;
                queryable = queryable.OrderBy(order.Key, sugarOrderType);
            }
        }
        return (queryable, (page, size));
    }

    /// <summary>
    /// 根据条件表达式获取单个实体
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体，不存在则返回 null</returns>
    /// <exception cref="ArgumentNullException">当 predicates 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<TDomain?> GetAsync(List<Expression<Func<TDomain, bool>>> predicates, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(predicates, nameof(predicates));

        if (predicates.Count == 0) {
            throw new ArgumentException("查询条件列表不能为空", nameof(predicates));
        }

        try {
            var (queryable, _) = BuildQueryable(predicates);
            return await queryable.FirstAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not ArgumentException and not ArgumentNullException) {
            _logger.LogError(ex, "获取实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取", _entityName);
        }
    }

    /// <summary>
    /// 根据条件表达式获取实体列表
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体列表</returns>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<List<TDomain>> GetListAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, CancellationToken cancellationToken = default) {
        try {
            var (queryable, _) = BuildQueryable(predicates, orders);
            return await queryable.ToListAsync(cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取实体列表失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取列表", _entityName);
        }
    }

    /// <summary>
    /// 根据分页参数获取分页实体列表
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，必须大于 0，默认值为 1</param>
    /// <param name="size">每页大小，必须大于 0，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含数据列表和分页信息</returns>
    /// <exception cref="ArgumentOutOfRangeException">当页码或每页大小小于等于 0 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<PagedResponse<TDomain>> GetPagedAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default) {
        try {
            var (queryable, (validPage, validSize)) = BuildQueryable(predicates, orders, page, size);
            RefAsync<int> total = 0;
            var items = await queryable.ToPageListAsync(validPage, validSize, total, cancellationToken);
            return new PagedResponse<TDomain>(items, total, validPage, validSize);
        }
        catch (Exception ex) when (ex is not ArgumentOutOfRangeException) {
            _logger.LogError(ex, "获取分页实体失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取分页", _entityName);
        }
    }

    /// <summary>
    /// 根据条件表达式和投影获取单个 DTO
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构，不能为 null</param>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果，不存在则返回 null</returns>
    /// <exception cref="ArgumentNullException">当 selector 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<TResult?> GetAsync<TResult>(Expression<Func<TDomain, TResult>> selector, List<Expression<Func<TDomain, bool>>>? predicates = null, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        try {
            var (queryable, _) = BuildQueryable(predicates);
            return await queryable.Select(selector).FirstAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not ArgumentNullException) {
            _logger.LogError(ex, "获取实体并投影失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取投影", _entityName);
        }
    }

    /// <summary>
    /// 根据条件表达式和投影获取 DTO 列表
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构，不能为 null</param>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果列表</returns>
    /// <exception cref="ArgumentNullException">当 selector 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TDomain, TResult>> selector, List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        try {
            var (queryable, _) = BuildQueryable(predicates, orders);
            return await queryable.Select(selector).ToListAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not ArgumentNullException) {
            _logger.LogError(ex, "获取实体列表并投影失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取列表投影", _entityName);
        }
    }

    /// <summary>
    /// 根据分页参数和投影获取分页 DTO 列表
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构，不能为 null</param>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，必须大于 0，默认值为 1</param>
    /// <param name="size">每页大小，必须大于 0，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含投影结果列表和分页信息</returns>
    /// <exception cref="ArgumentNullException">当 selector 为 null 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当页码或每页大小小于等于 0 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<PagedResponse<TResult>> GetPagedAsync<TResult>(Expression<Func<TDomain, TResult>> selector, List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        try {
            var (queryable, (validPage, validSize)) = BuildQueryable(predicates, orders, page, size);
            RefAsync<int> total = 0;
            var items = await queryable.Select(selector).ToPageListAsync(validPage, validSize, total, cancellationToken);
            return new PagedResponse<TResult>(items, total, validPage, validSize);
        }
        catch (Exception ex) when (ex is not ArgumentNullException and not ArgumentOutOfRangeException) {
            _logger.LogError(ex, "获取分页实体并投影失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取分页投影", _entityName);
        }
    }

    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<bool> ExistsAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, CancellationToken cancellationToken = default) {
        try {
            var (queryable, _) = BuildQueryable(predicates);
            return await queryable.AnyAsync(cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "检查实体是否存在失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "检查存在", _entityName);
        }
    }

    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体数量</returns>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<int> CountAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, CancellationToken cancellationToken = default) {
        try {
            var (queryable, _) = BuildQueryable(predicates);
            return await queryable.CountAsync(cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "获取实体数量失败: {EntityName}", _entityName);
            throw RepositoryExceptionHelper.HandleException(ex, "获取数量", _entityName);
        }
    }

    /// <summary>
    /// 根据条件表达式获取单个实体（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，不能为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体，不存在则返回 null</returns>
    /// <exception cref="ArgumentNullException">当 predicate 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<TDomain?> GetAsync(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        return await GetAsync([predicate], cancellationToken);
    }

    /// <summary>
    /// 根据条件表达式获取实体列表（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体列表</returns>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<List<TDomain>> GetListAsync(Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, CancellationToken cancellationToken = default) {
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await GetListAsync(predicates, orders, cancellationToken);
    }

    /// <summary>
    /// 根据分页参数获取分页实体列表（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，必须大于 0，默认值为 1</param>
    /// <param name="size">每页大小，必须大于 0，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含数据列表和分页信息</returns>
    /// <exception cref="ArgumentOutOfRangeException">当页码或每页大小小于等于 0 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<PagedResponse<TDomain>> GetPagedAsync(Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default) {
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await GetPagedAsync(predicates, orders, page, size, cancellationToken);
    }

    /// <summary>
    /// 根据条件表达式和投影获取单个 DTO（非集合版本）
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构，不能为 null</param>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果，不存在则返回 null</returns>
    /// <exception cref="ArgumentNullException">当 selector 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<TResult?> GetAsync<TResult>(Expression<Func<TDomain, TResult>> selector, Expression<Func<TDomain, bool>>? predicate = null, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await GetAsync(selector, predicates, cancellationToken);
    }

    /// <summary>
    /// 根据条件表达式和投影获取 DTO 列表（非集合版本）
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构，不能为 null</param>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果列表</returns>
    /// <exception cref="ArgumentNullException">当 selector 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TDomain, TResult>> selector, Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await GetListAsync(selector, predicates, orders, cancellationToken);
    }

    /// <summary>
    /// 根据分页参数和投影获取分页 DTO 列表（非集合版本）
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构，不能为 null</param>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，必须大于 0，默认值为 1</param>
    /// <param name="size">每页大小，必须大于 0，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含投影结果列表和分页信息</returns>
    /// <exception cref="ArgumentNullException">当 selector 为 null 时抛出</exception>
    /// <exception cref="ArgumentOutOfRangeException">当页码或每页大小小于等于 0 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<PagedResponse<TResult>> GetPagedAsync<TResult>(Expression<Func<TDomain, TResult>> selector, Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, bool>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await GetPagedAsync(selector, predicates, orders, page, size, cancellationToken);
    }

    /// <summary>
    /// 检查实体是否存在（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<bool> ExistsAsync(Expression<Func<TDomain, bool>>? predicate = null, CancellationToken cancellationToken = default) {
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await ExistsAsync(predicates, cancellationToken);
    }

    /// <summary>
    /// 获取实体数量（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体数量</returns>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public virtual async Task<int> CountAsync(Expression<Func<TDomain, bool>>? predicate = null, CancellationToken cancellationToken = default) {
        List<Expression<Func<TDomain, bool>>>? predicates = predicate != null ? [predicate] : null;
        return await CountAsync(predicates, cancellationToken);
    }
}
