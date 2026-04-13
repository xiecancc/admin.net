/*
 * 文件名称: IPermissionRepository.cs
 * 功能描述: 权限仓储接口，定义权限相关的数据访问操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Domain.Entities;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;

namespace Domain.Repositories;

/// <summary>
/// 权限仓储接口
/// <para>定义权限相关的数据访问操作</para>
/// </summary>
/// <typeparam name="TPermission">权限实体类型</typeparam>
/// <remarks>
/// <para>继承自 IAggregateTreeRepository&lt;TPermission&gt;，提供聚合根和树形结构的基本操作</para>
/// <para>扩展功能：</para>
/// <list type="bullet">
///   <item>根据编码查找权限</item>
///   <item>检查权限编码是否存在</item>
/// </list>
/// </remarks>
public interface IPermissionRepository<TPermission> : IAggregateTreeRepository<TPermission>
    where TPermission : PermissionBase, IAggregateTree<TPermission>, new() {
    /// <summary>
    /// 根据编码查找权限
    /// </summary>
    /// <param name="code">权限编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限实体，不存在则返回 null</returns>
    Task<TPermission?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查权限编码是否存在
    /// </summary>
    /// <param name="code">权限编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistsAsync(string code, CancellationToken cancellationToken = default);
}

/// <inheritdoc cref="IPermissionRepository{TPermission}"/>
public interface IMenuPermissionRepository : IPermissionRepository<MenuPermission> {
}

/// <inheritdoc cref="IPermissionRepository{TPermission}"/>
public interface IApiPermissionRepository : IPermissionRepository<ApiPermission> {
}

/// <inheritdoc cref="IPermissionRepository{TPermission}"/>
public interface IButtonPermissionRepository : IPermissionRepository<ButtonPermission> {
}
