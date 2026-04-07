/*
 * 文件名称: IRepository.cs
 * 功能描述: 泛型仓储接口，所有仓储接口的基接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-01
 */

using Domain.Shared.Dtos;
using Domain.Shared.Entities;
using SqlSugar;
using System.Linq.Expressions;

namespace Domain.Shared.Repositories;

/// <summary>
/// 泛型仓储接口，所有仓储接口的基接口
/// </summary>
/// <remarks>
/// <para>此接口作为所有仓储接口的基接口，提供统一的类型标识</para>
/// <para>主要用于依赖注入和类型识别，包含基础方法</para>
/// </remarks>
/// <typeparam name="TDomain">领域模型类型，必须继承自DomainBase</typeparam>
public interface IRepository<TDomain> where TDomain : DomainBase, new() {
    /// <summary>
    /// 根据条件表达式获取单个实体
    /// </summary>
    /// <param name="predicates">查询条件表达式列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体，不存在则返回 null</returns>
    Task<TDomain?> GetAsync(List<Expression<Func<TDomain, bool>>> predicates, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式获取单个实体（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体，不存在则返回 null</returns>
    Task<TDomain?> GetAsync(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式获取实体列表
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体列表</returns>
    Task<List<TDomain>> GetListAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式获取实体列表（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体列表</returns>
    Task<List<TDomain>> GetListAsync(Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据分页参数获取分页实体列表
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，默认值为 1</param>
    /// <param name="size">每页大小，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含数据列表和分页信息</returns>
    Task<PagedResponse<TDomain>> GetPagedAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据分页参数获取分页实体列表（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，默认值为 1</param>
    /// <param name="size">每页大小，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含数据列表和分页信息</returns>
    Task<PagedResponse<TDomain>> GetPagedAsync(Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式和投影获取单个 DTO
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构</param>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果，不存在则返回 null</returns>
    Task<TResult?> GetAsync<TResult>(Expression<Func<TDomain, TResult>> selector, List<Expression<Func<TDomain, bool>>>? predicates = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式和投影获取单个 DTO（非集合版本）
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构</param>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果，不存在则返回 null</returns>
    Task<TResult?> GetAsync<TResult>(Expression<Func<TDomain, TResult>> selector, Expression<Func<TDomain, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式和投影获取 DTO 列表
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构</param>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果列表</returns>
    Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TDomain, TResult>> selector, List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件表达式和投影获取 DTO 列表（非集合版本）
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构</param>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的投影结果列表</returns>
    Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TDomain, TResult>> selector, Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据分页参数和投影获取分页 DTO 列表
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构</param>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，默认值为 1</param>
    /// <param name="size">每页大小，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含投影结果列表和分页信息</returns>
    Task<PagedResponse<TResult>> GetPagedAsync<TResult>(Expression<Func<TDomain, TResult>> selector, List<Expression<Func<TDomain, bool>>>? predicates = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据分页参数和投影获取分页 DTO 列表（非集合版本）
    /// </summary>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    /// <param name="selector">投影表达式，用于指定返回的DTO结构</param>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="orders">排序表达式字典，可为 null</param>
    /// <param name="page">页码，默认值为 1</param>
    /// <param name="size">每页大小，默认值为 10</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页响应对象，包含投影结果列表和分页信息</returns>
    Task<PagedResponse<TResult>> GetPagedAsync<TResult>(Expression<Func<TDomain, TResult>> selector, Expression<Func<TDomain, bool>>? predicate = null, IDictionary<Expression<Func<TDomain, object>>, OrderByType>? orders = null, int page = 1, int size = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    Task<bool> ExistsAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查实体是否存在（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    Task<bool> ExistsAsync(Expression<Func<TDomain, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="predicates">查询条件表达式列表，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体数量</returns>
    Task<int> CountAsync(List<Expression<Func<TDomain, bool>>>? predicates = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体数量（非集合版本）
    /// </summary>
    /// <param name="predicate">查询条件表达式，可为 null</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>符合条件的实体数量</returns>
    Task<int> CountAsync(Expression<Func<TDomain, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
