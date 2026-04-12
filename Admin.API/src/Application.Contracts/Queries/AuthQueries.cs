/*
 * 文件名称: AuthQueries.cs
 * 功能描述: 认证相关的查询类，包含获取当前用户信息等查询
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-12
 */

using Application.Contracts.Abstractions;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 获取当前用户资料查询
/// <para>用于获取当前登录用户的详细资料</para>
/// </summary>
public class GetProfileQuery : Request<LoginUserInfoDto> {
    /// <summary>
    /// 用户ID
    /// </summary>
    public required Guid UserId { get; init; }
}
