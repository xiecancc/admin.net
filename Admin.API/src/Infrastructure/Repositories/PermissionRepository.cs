/*
 * 文件名称: PermissionRepository.cs
 * 功能描述: 权限仓储实现类，实现权限相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Entities;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IPermissionRepository{TPermission}"/>
/// <remarks>
/// <para>权限仓储负责：</para>
/// <list type="bullet">
///   <item> 提供权限实体的持久化操作 </item>
///   <item> 支持通过编码查询权限 </item>
///   <item> 支持检查编码是否存在 </item>
/// </list>
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentException（编码为空或空白）</item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
/// </list>
/// </remarks>
/// <typeparam name="TPermission">权限类型，必须继承自 Permission 并实现 IAggregateTree 接口</typeparam>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="entityName">实体名称，用于日志记录</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class PermissionRepository<TPermission>(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    string entityName,
    ILogger<PermissionRepository<TPermission>> logger)
    : AggregateTreeRepository<TPermission>(client, eventBus, userContextProvider, entityName, logger), IPermissionRepository<TPermission>
    where TPermission : Permission, IAggregateTree<TPermission>, new() {
    /// <summary>
    /// 根据编码查找权限
    /// </summary>
    /// <param name="code">权限编码，不能为 null、空字符串或空白字符串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>找到的权限实体，不存在则返回 null</returns>
    /// <exception cref="ArgumentException">当 code 为 null、空字符串或空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<TPermission?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        try {
            _logger.LogDebug("开始根据编码查询权限: {Code}", code);
            var result = await GetAsync(t => t.Code == code, cancellationToken);

            if (result != null) {
                _logger.LogDebug("根据编码查询权限成功: {Code}, 权限ID: {PermissionId}", code, result.Id);
            }
            else {
                _logger.LogDebug("根据编码未找到权限: {Code}", code);
            }

            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "根据编码查询权限失败: {Code}", code);
            throw RepositoryExceptionHelper.HandleException(ex, "查询权限", _entityName);
        }
    }

    /// <summary>
    /// 检查编码是否已存在
    /// </summary>
    /// <param name="code">要检查的权限编码，不能为 null、空字符串或空白字符串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>编码已存在返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 code 为 null、空字符串或空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        try {
            _logger.LogDebug("开始检查权限编码是否存在: {Code}", code);
            var exists = await ExistsAsync(t => t.Code == code, cancellationToken);
            _logger.LogDebug("检查权限编码是否存在完成: {Code}, 结果: {Exists}", code, exists);
            return exists;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "检查权限编码是否存在失败: {Code}", code);
            throw RepositoryExceptionHelper.HandleException(ex, "检查权限编码", _entityName);
        }
    }
}

/// <inheritdoc cref="IMenuPermissionRepository"/>
/// <remarks>
/// <para>菜单权限仓储负责：</para>
/// <list type="bullet">
///   <item> 提供菜单权限实体的持久化操作 </item>
///   <item> 继承权限仓储的所有功能 </item>
/// </list>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class MenuPermissionRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<MenuPermissionRepository> logger)
    : PermissionRepository<MenuPermission>(client, eventBus, userContextProvider, "菜单权限", logger), IMenuPermissionRepository {
}

/// <inheritdoc cref="IApiPermissionRepository"/>
/// <remarks>
/// <para>API 权限仓储负责：</para>
/// <list type="bullet">
///   <item> 提供 API 权限实体的持久化操作 </item>
///   <item> 继承权限仓储的所有功能 </item>
/// </list>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class ApiPermissionRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<ApiPermissionRepository> logger)
    : PermissionRepository<ApiPermission>(client, eventBus, userContextProvider, "API 权限", logger), IApiPermissionRepository {
}

/// <inheritdoc cref="IButtonPermissionRepository"/>
/// <remarks>
/// <para>按钮权限仓储负责：</para>
/// <list type="bullet">
///   <item> 提供按钮权限实体的持久化操作 </item>
///   <item> 继承权限仓储的所有功能 </item>
/// </list>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class ButtonPermissionRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<ButtonPermissionRepository> logger)
    : PermissionRepository<ButtonPermission>(client, eventBus, userContextProvider, "按钮权限", logger), IButtonPermissionRepository {
}
