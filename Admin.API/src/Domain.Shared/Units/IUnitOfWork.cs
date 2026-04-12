using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Domain.Shared.Units;

/// <summary>
/// 工作单元接口
/// </summary>
public interface IUnitOfWork : IDisposable {
    /// <summary>
    /// 获取指定领域仓储实例
    /// </summary>
    /// <typeparam name="TRepository">仓储类型</typeparam>
    /// <typeparam name="TDomain">领域模型类型</typeparam>
    /// <returns>仓储实例</returns>
    TRepository GetRepository<TRepository, TDomain>()
        where TRepository : IDomainRepository<TDomain>
        where TDomain : DomainBase, new();

    /// <summary>
    /// 在事务中执行操作（有返回值）
    /// </summary>
    /// <typeparam name="TResult">操作结果类型</typeparam>
    /// <param name="action">异步操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// 在事务中执行操作（无返回值）
    /// </summary>
    /// <param name="action">异步操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
