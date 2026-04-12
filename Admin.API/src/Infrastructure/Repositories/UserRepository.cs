/*
 * 文件名称: UserRepository.cs
 * 功能描述: 用户仓储实现类，实现用户相关的数据访问操作
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

/// <inheritdoc cref="IUserRepository"/>
/// <remarks>
/// <para>用户仓储负责：</para>
/// <list type="bullet">
///   <item> 提供用户实体的持久化操作 </item>
///   <item> 支持通过邮箱查询用户 </item>
///   <item> 支持检查邮箱是否存在 </item>
/// </list>
/// <para>异常处理策略：</para>
/// <list type="bullet">
///   <item> 参数验证异常：ArgumentException（邮箱为空或空白）</item>
///   <item> 数据库异常：通过 RepositoryExceptionHelper 转换为业务异常 </item>
/// </list>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="userContextProvider">用户上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class UserRepository(
    ISqlSugarClient client,
    IDomainEventBus eventBus,
    IUserContextProvider userContextProvider,
    ILogger<UserRepository> logger)
    : AggregateRepository<User>(client, eventBus, userContextProvider, nameof(User), logger), IUserRepository {
    /// <summary>
    /// 根据邮箱查找用户
    /// </summary>
    /// <param name="email">用户邮箱地址，不能为 null、空字符串或空白字符串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>找到的用户实体，不存在则返回 null</returns>
    /// <exception cref="ArgumentException">当 email 为 null、空字符串或空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));

        try {
            _logger.LogDebug("开始根据邮箱查询用户: {Email}", email);
            var result = await GetAsync(u => u.Email == email, cancellationToken);

            if (result != null) {
                _logger.LogDebug("根据邮箱查询用户成功: {Email}, 用户ID: {UserId}", email, result.Id);
            }
            else {
                _logger.LogDebug("根据邮箱未找到用户: {Email}", email);
            }

            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "根据邮箱查询用户失败: {Email}", email);
            throw RepositoryExceptionHelper.HandleException(ex, "查询用户", _entityName);
        }
    }

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    /// <param name="email">要检查的邮箱地址，不能为 null、空字符串或空白字符串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>邮箱已存在返回 true，否则返回 false</returns>
    /// <exception cref="ArgumentException">当 email 为 null、空字符串或空白字符串时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据库操作失败时抛出</exception>
    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));

        try {
            _logger.LogDebug("开始检查邮箱是否存在: {Email}", email);
            var exists = await ExistsAsync(t => t.Email == email, cancellationToken);
            _logger.LogDebug("检查邮箱是否存在完成: {Email}, 结果: {Exists}", email, exists);
            return exists;
        }
        catch (Exception ex) when (ex is not ArgumentException) {
            _logger.LogError(ex, "检查邮箱是否存在失败: {Email}", email);
            throw RepositoryExceptionHelper.HandleException(ex, "检查邮箱", _entityName);
        }
    }
}
