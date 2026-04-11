/*
 * 文件名称: UserInfoDTO.cs
 * 功能描述: 用户缓存数据传输对象，用于缓存用户基本信息和角色列表
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-04
 */

namespace Domain.Shared.Dtos;

/// <summary>
/// 用户缓存数据传输对象
/// <para>用于缓存用户基本信息和角色列表</para>
/// </summary>
/// <remarks>
/// <para>主要用途：</para>
/// <list type="bullet">
///   <item>用户身份认证缓存</item>
///   <item>权限验证缓存</item>
///   <item>用户会话管理</item>
/// </list>
/// <para>数据来源：从 User 实体和 Role 实体映射</para>
/// </remarks>
public class UserInfoDTO {
    /// <summary>
    /// 用户标识
    /// </summary>
    /// <value>用户的唯一标识符，不能为空</value>
    public required Guid Id {
        get; init;
    }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户的邮箱地址，作为登录账号，不能为空</value>
    public required string Email {
        get; init;
    }

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; init;
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <value>用户拥有的角色名称列表，不能为空</value>
    public required IReadOnlyList<string> Roles {
        get; init;
    }
}
