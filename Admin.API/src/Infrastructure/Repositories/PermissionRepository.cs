/*
 * 文件名称: PermissionRepository.cs
 * 功能描述: 权限仓储实现类，实现权限相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Domain.Shared.Events;
using Domain.Shared.Entities;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IPermissionRepository{TPermission}"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="entityName">实体名称</param>
/// <param name="logger">日志记录器</param>
public class PermissionRepository<TPermission>(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, string entityName, ILogger<PermissionRepository<TPermission>> logger)
    : AggregateTreeRepository<TPermission>(client, eventBus, httpContextProvider, entityName, logger), IPermissionRepository<TPermission>
    where TPermission : Permission, IAggregateTree<TPermission>, new() {
    /// <inheritdoc/>
    public async Task<TPermission?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) {
        return await GetAsync(t => t.Code == code, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default) {
        return await ExistsAsync(t => t.Code == code, cancellationToken);
    }
}

/// <inheritdoc cref="IMenuPermissionRepository"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="logger">日志记录器</param>
public class MenuPermissionRepository(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, ILogger<MenuPermissionRepository> logger)
    : PermissionRepository<MenuPermission>(client, eventBus, httpContextProvider, "菜单权限", logger), IMenuPermissionRepository {
}

/// <inheritdoc cref="IApiPermissionRepository"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="logger">日志记录器</param>
public class ApiPermissionRepository(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, ILogger<ApiPermissionRepository> logger)
    : PermissionRepository<ApiPermission>(client, eventBus, httpContextProvider, "API 权限", logger), IApiPermissionRepository {
}

/// <inheritdoc cref="IButtonPermissionRepository"/>
/// <param name="client">SqlSugar 客户端</param>
/// <param name="eventBus">领域事件总线</param>
/// <param name="httpContextProvider">HTTP 上下文提供者</param>
/// <param name="logger">日志记录器</param>
public class ButtonPermissionRepository(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, ILogger<ButtonPermissionRepository> logger)
    : PermissionRepository<ButtonPermission>(client, eventBus, httpContextProvider, "按钮权限", logger), IButtonPermissionRepository {
}
