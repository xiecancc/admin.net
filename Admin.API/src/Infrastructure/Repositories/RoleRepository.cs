/*
 * 文件名称: RoleRepository.cs
 * 功能描述: 角色仓储实现类，实现角色相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IRoleRepository"/>
/// <param name="client">SqlSugar 客户端</param>
    /// <param name="eventBus">领域事件总线</param>
    /// <param name="userContextProvider">用户上下文提供者</param>
    /// <param name="logger">日志记录器</param>
public class RoleRepository(ISqlSugarClient client, IDomainEventBus eventBus, IUserContextProvider userContextProvider, ILogger<RoleRepository> logger)
    : AggregateTreeRepository<Role>(client, eventBus, userContextProvider, "角色", logger), IRoleRepository {
    /// <inheritdoc/>
    public async Task<Role?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) {
        return await GetAsync(t => t.Code == code, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default) {
        return await ExistsAsync(t => t.Code == code, cancellationToken);
    }
}
