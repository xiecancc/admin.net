using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Domain.Shared.Units;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Infrastructure.Units;

/// <summary>
/// 工作单元实现类
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="serviceProvider">服务提供者</param>
public class UnitOfWork(ISqlSugarClient client, IServiceProvider serviceProvider) : IUnitOfWork {
    private bool _disposed;

    /// <summary>
    /// 获取指定领域仓储实例
    /// </summary>
    public TRepository GetRepository<TRepository, TDomain>()
        where TRepository : IDomainRepository<TDomain>
        where TDomain : DomainBase, new() {
        return serviceProvider.GetRequiredService<TRepository>();
    }

    /// <summary>
    /// 在事务中执行操作（有返回值）
    /// </summary>
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();

        var result = await client.Ado.UseTranAsync(action);

        if (!result.IsSuccess) {
            throw result.ErrorException is not null
                ? new InvalidOperationException($"事务执行失败: {result.ErrorException.Message}", result.ErrorException)
                : new InvalidOperationException("事务执行失败，未知错误");
        }

        return result.Data;
    }

    /// <summary>
    /// 在事务中执行操作（无返回值）
    /// </summary>
    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();

        var result = await client.Ado.UseTranAsync(action);

        if (!result.IsSuccess) {
            throw result.ErrorException is not null
                ? new InvalidOperationException($"事务执行失败: {result.ErrorException.Message}", result.ErrorException)
                : new InvalidOperationException("事务执行失败，未知错误");
        }
    }

    /// <summary>
    /// 释放资源
    /// 注意：不释放 SqlSugarClient，由 DI 容器管理生命周期
    /// </summary>
    public void Dispose() {
        if (!_disposed) {
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
