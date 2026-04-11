/*
 * 文件名称: UserQueries.cs
 * 功能描述: 用户相关查询类，包含用户的所有查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;

namespace Application.Contracts.Queries;

/// <summary>
/// 用户根据ID查询
/// <para>用于根据ID获取用户详情</para>
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="id">用户ID</param>
public class UserByIdQuery(Guid id) : AggregateByIdQuery<UserDetailDto> {
    /// <summary>
    /// 主键 ID
    /// </summary>
    public new Guid Id { get; set; } = id;
}

/// <summary>
/// 用户列表查询
/// <para>用于获取用户列表</para>
/// </summary>
public class UserListQuery : AggregateListQuery<UserListDto> {
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户邮箱，用于模糊搜索</value>
    public string? Email { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户昵称，用于模糊搜索</value>
    public string? NickName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户手机号，用于模糊搜索</value>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户状态，用于筛选</value>
    public bool? Status { get; set; }
}

/// <summary>
/// 用户分页查询
/// <para>用于分页获取用户列表</para>
/// </summary>
public class UserPagedQuery : AggregatePagedQuery<UserPagedDto> {
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户邮箱，用于模糊搜索</value>
    public string? Email { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户昵称，用于模糊搜索</value>
    public string? NickName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户手机号，用于模糊搜索</value>
    public string? Phone { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <value>用户状态，用于筛选</value>
    public bool? Status { get; set; }
}
