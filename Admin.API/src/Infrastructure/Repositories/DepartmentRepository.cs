/*
 * 文件名称: DepartmentRepository.cs
 * 功能描述: 部门仓储实现，提供部门相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Events;
using Infrastructure.Shared.Contexts;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Infrastructure.Repositories;

/// <inheritdoc cref="IDepartmentRepository"/>
/// <remarks>
/// <para>部门仓储负责：</para>
/// <list type="bullet">
///   <item> 提供部门的基本CRUD操作 </item>
///   <item> 提供部门树形结构的查询和操作 </item>
///   <item> 提供根据编码查找部门的功能 </item>
///   <item> 提供检查部门编码是否存在的功能 </item>
/// </list>
/// <para>实现策略：继承自 AggregateTreeRepository，利用其树形结构操作能力</para>
/// <para>异常处理：所有操作失败时抛出异常，由调用方处理</para>
/// </remarks>
/// <param name="client">SqlSugar 客户端，用于数据库操作</param>
/// <param name="eventBus">领域事件总线，用于发布领域事件</param>
/// <param name="httpContextProvider">HTTP 上下文提供者，用于获取当前用户信息</param>
/// <param name="logger">日志记录器，用于记录操作日志</param>
public class DepartmentRepository(ISqlSugarClient client, IDomainEventBus eventBus, IHttpContextProvider httpContextProvider, ILogger<DepartmentRepository> logger)
    : AggregateTreeRepository<Department>(client, eventBus, httpContextProvider, "Department", logger), IDepartmentRepository {

    /// <inheritdoc/>
    public async Task<Department?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(code)) {
            _logger.LogWarning("根据编码查找部门失败：编码为空");
            return null;
        }

        try {
            var department = await _client.Queryable<Department>()
                .Where(d => d.Code == code && !d.IsDeleted)
                .FirstAsync(cancellationToken);

            _logger.LogInformation("根据编码查找部门成功：编码: {Code}, 部门ID: {DepartmentId}", code, department?.Id ?? Guid.Empty);
            return department;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "根据编码查找部门失败：编码: {Code}", code);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(code)) {
            _logger.LogWarning("检查部门编码是否存在失败：编码为空");
            return false;
        }

        try {
            var exists = await _client.Queryable<Department>()
                .Where(d => d.Code == code && !d.IsDeleted)
                .AnyAsync(cancellationToken);

            _logger.LogInformation("检查部门编码是否存在：编码: {Code}, 结果: {Exists}", code, exists);
            return exists;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "检查部门编码是否存在失败：编码: {Code}", code);
            throw;
        }
    }
}