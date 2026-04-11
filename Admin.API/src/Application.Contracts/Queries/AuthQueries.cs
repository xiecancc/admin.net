/*
 * 文件名称: AuthQueries.cs
 * 功能描述: 认证相关的查询类，包含获取当前用户信息等查询
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 获取当前用户信息查询
/// <para>用于获取当前登录用户的详细信息</para>
/// </summary>
/// <param name="userId">用户 ID</param>
public class GetCurrentUserQuery(Guid userId) : Query<LoginUserInfoDto> {
    /// <summary>用户 ID</summary>
    public Guid UserId { get; set; } = userId;
}
