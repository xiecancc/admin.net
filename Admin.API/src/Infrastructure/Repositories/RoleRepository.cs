/*
 * 文件名称: RoleRepository.cs
 * 功能描述: 角色仓储实现类，实现角色相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Domain.Shared.Utils;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IRoleRepository"/>
/// <remarks>
/// <para>角色仓储负责：</para>
/// <list type="bullet">
///   <item> 提供角色实体的持久化操作 </item>
///   <item> 支持通过编码查询角色 </item>
///   <item> 支持检查编码是否存在 </item>
///   <item> 支持检测角色继承循环 </item>
/// </list>
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentException（编码为空或空白、ID为空）</item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
/// </list>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class RoleRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<RoleRepository> logger)
    : AggregateTreeRepository<Role>(client, eventBus, userContextProvider, "角色", logger), IRoleRepository {
    /// <summary>
    /// 根据编码查找角色
    /// </summary>
    /// <param name="code">角色编码，不能为 null、空字符串或空白字符串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>找到的角色实体，不存在则返回 null</returns>
    /// <exception cref="ArgumentException">当 code 为 null、空字符串或空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<Role?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        try {
            _logger.LogDebug("开始根据编码查询角色: {Code}", code);
            var result = await GetAsync(t => t.Code == code, cancellationToken);

            if (result != null) {
                _logger.LogDebug("根据编码查询角色成功: {Code}, 角色ID: {RoleId}", code, result.Id);
            }
            else {
                _logger.LogDebug("根据编码未找到角色: {Code}", code);
            }

            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "根据编码查询角色失败: {Code}", code);
            throw RepositoryExceptionHelper.HandleException(ex, "查询角色", _entityName);
        }
    }

    /// <summary>
    /// 检查编码是否已存在
    /// </summary>
    /// <param name="code">要检查的角色编码，不能为 null、空字符串或空白字符串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>编码已存在返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 code 为 null、空字符串或空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(code, nameof(code));

        try {
            _logger.LogDebug("开始检查角色编码是否存在: {Code}", code);
            var exists = await ExistsAsync(t => t.Code == code, cancellationToken);
            _logger.LogDebug("检查角色编码是否存在完成: {Code}, 结果: {Exists}", code, exists);
            return exists;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "检查角色编码是否存在失败: {Code}", code);
            throw RepositoryExceptionHelper.HandleException(ex, "检查角色编码", _entityName);
        }
    }

    /// <summary>
    /// 检测角色继承是否存在循环引用
    /// </summary>
    /// <param name="roleId">要检测的角色ID，不能为 Guid.Empty</param>
    /// <param name="parentId">父角色ID，不能为 Guid.Empty</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在循环引用返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 roleId 或 parentId 为 Guid.Empty 时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> HasInheritanceCycleAsync(Guid roleId, Guid parentId, CancellationToken cancellationToken = default) {
        if (roleId == Guid.Empty) {
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
        }

        if (parentId == Guid.Empty) {
            throw new ArgumentException("父角色ID不能为空", nameof(parentId));
        }

        _logger.LogDebug("开始检测角色继承循环: 角色ID: {RoleId}, 父角色ID: {ParentId}", roleId, parentId);

        try {
            var hasCycle = await HasCircularReferenceAsync(roleId, parentId, cancellationToken);

            if (hasCycle) {
                _logger.LogWarning("检测到角色继承循环: 角色ID: {RoleId}, 父角色ID: {ParentId}", roleId, parentId);
            }
            else {
                _logger.LogDebug("未检测到角色继承循环: 角色ID: {RoleId}, 父角色ID: {ParentId}", roleId, parentId);
            }

            return hasCycle;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "检测角色继承循环失败: 角色ID: {RoleId}, 父角色ID: {ParentId}", roleId, parentId);
            throw RepositoryExceptionHelper.HandleException(ex, "检测继承循环", _entityName);
        }
    }
}
