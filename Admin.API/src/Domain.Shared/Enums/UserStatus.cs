/*
 * 文件名称: UserStatus.cs
 * 功能描述: 用户状态枚举，定义用户的所有可能状态
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Domain.Shared.Enums;

/// <summary>
/// 用户状态枚举
/// <para>定义用户的所有可能状态</para>
/// </summary>
/// <remarks>
/// <para>状态说明：</para>
/// <list type="bullet">
///   <item>Normal：正常状态，用户可以正常登录和使用系统</item>
///   <item>Disabled：禁用状态，用户被管理员禁用，无法登录</item>
///   <item>Locked：锁定状态，用户因多次登录失败或其他原因被锁定</item>
/// </list>
/// </remarks>
public enum UserStatus {
    /// <summary>
    /// 正常
    /// </summary>
    Normal = 0,

    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 1,

    /// <summary>
    /// 锁定
    /// </summary>
    Locked = 2
}
