/*
 * 文件名称: RoleQueryBuilder.cs
 * 功能描述: 角色查询条件构建器，提供共享的 BuildPredicates 和 BuildCacheParams 逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Caching;
using Application.Contracts.Dtos;
using Domain.Entities;
using System.Linq.Expressions;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 角色查询条件构建器
/// <para>提供共享的 BuildPredicates 和 BuildCacheParams 逻辑，供 List 和 Paged 查询处理器使用</para>
/// </summary>
public static class RoleQueryBuilder {
    /// <summary>
    /// 构建角色查询条件
    /// </summary>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <param name="basePredicates">基础条件列表（可选）</param>
    /// <returns>查询条件列表</returns>
    public static List<Expression<Func<Role, bool>>> BuildPredicates(
        RoleQueryDto? queryDto,
        List<Expression<Func<Role, bool>>>? basePredicates = null) {
        var predicates = basePredicates ?? [];

        if (queryDto is null) {
            return predicates;
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Code)) {
            predicates.Add(r => r.Code.Contains(queryDto.Code!));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Name)) {
            predicates.Add(r => r.Name.Contains(queryDto.Name!));
        }

        if (queryDto.ParentId.HasValue) {
            predicates.Add(r => r.ParentId == queryDto.ParentId.Value);
        }

        return predicates;
    }

    /// <summary>
    /// 构建角色查询缓存键参数
    /// </summary>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <param name="baseBuilder">基础 StringBuilder（可选）</param>
    /// <returns>缓存键参数 StringBuilder</returns>
    public static StringBuilder BuildCacheParams(
        RoleQueryDto? queryDto,
        StringBuilder? baseBuilder = null) {
        var builder = baseBuilder ?? new StringBuilder();

        if (queryDto is null) {
            return builder;
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Code)) {
            builder.Append($"|Code={queryDto.Code}");
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Name)) {
            builder.Append($"|Name={queryDto.Name}");
        }

        if (queryDto.ParentId.HasValue) {
            builder.Append($"|ParentId={queryDto.ParentId.Value}");
        }

        return builder;
    }

    /// <summary>
    /// 使用 CacheKeyBuilder 构建角色查询缓存键
    /// </summary>
    /// <param name="builder">缓存键构建器</param>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <returns>缓存键构建器</returns>
    public static CacheKeyBuilder AppendRoleParams(
        this CacheKeyBuilder builder,
        RoleQueryDto? queryDto) {
        if (queryDto is null) {
            return builder;
        }

        builder
            .AppendIfNotEmpty("code", queryDto.Code)
            .AppendIfNotEmpty("name", queryDto.Name)
            .AppendIfNotEmpty("parentId", queryDto.ParentId);

        return builder;
    }
}
