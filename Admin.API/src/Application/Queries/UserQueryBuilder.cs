/*
 * 文件名称: UserQueryBuilder.cs
 * 功能描述: 用户查询条件构建器，提供共享的 BuildPredicates 和 BuildCacheParams 逻辑
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
/// 用户查询条件构建器
/// <para>提供共享的 BuildPredicates 和 BuildCacheParams 逻辑，供 List 和 Paged 查询处理器使用</para>
/// </summary>
public static class UserQueryBuilder {
    /// <summary>
    /// 构建用户查询条件
    /// </summary>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <param name="basePredicates">基础条件列表（可选）</param>
    /// <returns>查询条件列表</returns>
    public static List<Expression<Func<User, bool>>> BuildPredicates(
        UserQueryDto? queryDto,
        List<Expression<Func<User, bool>>>? basePredicates = null) {
        var predicates = basePredicates ?? [];

        if (queryDto is null) {
            return predicates;
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Email)) {
            predicates.Add(u => u.Email.Contains(queryDto.Email!));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.NickName)) {
            predicates.Add(u => u.NickName != null && u.NickName.Contains(queryDto.NickName!));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Phone)) {
            predicates.Add(u => u.Phone != null && u.Phone.Contains(queryDto.Phone!));
        }

        return predicates;
    }

    /// <summary>
    /// 构建用户查询缓存键参数
    /// </summary>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <param name="baseBuilder">基础 StringBuilder（可选）</param>
    /// <returns>缓存键参数 StringBuilder</returns>
    public static StringBuilder BuildCacheParams(
        UserQueryDto? queryDto,
        StringBuilder? baseBuilder = null) {
        var builder = baseBuilder ?? new StringBuilder();

        if (queryDto is null) {
            return builder;
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Email)) {
            builder.Append($"|Email={queryDto.Email}");
        }

        if (!string.IsNullOrWhiteSpace(queryDto.NickName)) {
            builder.Append($"|NickName={queryDto.NickName}");
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Phone)) {
            builder.Append($"|Phone={queryDto.Phone}");
        }

        return builder;
    }

    /// <summary>
    /// 使用 CacheKeyBuilder 构建用户查询缓存键
    /// </summary>
    /// <param name="builder">缓存键构建器</param>
    /// <param name="queryDto">查询参数 DTO</param>
    /// <returns>缓存键构建器</returns>
    public static CacheKeyBuilder AppendUserParams(
        this CacheKeyBuilder builder,
        UserQueryDto? queryDto) {
        if (queryDto is null) {
            return builder;
        }

        builder
            .AppendIfNotEmpty("email", queryDto.Email)
            .AppendIfNotEmpty("nickName", queryDto.NickName)
            .AppendIfNotEmpty("phone", queryDto.Phone);

        return builder;
    }
}
