/*
 * 文件名称: UserRepository.cs
 * 功能描述: 用户仓储实现类，实现用户相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Domain.Shared.Events;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IUserRepository"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="logger">日志记录器</param>
public class UserRepository(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, ILogger<UserRepository> logger)
    : AggregateRepository<User>(client, eventBus, httpContextProvider, "用户", logger), IUserRepository {
    /// <inheritdoc/>
    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) {
        return await GetAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default) {
        return await ExistsAsync(t => t.Email == email, cancellationToken);
    }
}
