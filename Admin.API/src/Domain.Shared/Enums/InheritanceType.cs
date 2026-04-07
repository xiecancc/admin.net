/*
 * 文件名称: InheritanceType.cs
 * 功能描述: 角色继承类型枚举
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-07
 */

namespace Domain.Shared.Enums;

/// <summary>
/// 角色继承类型枚举
/// <para>定义角色权限的继承方式</para>
/// </summary>
public enum InheritanceType {
    /// <summary>
    /// 不继承
    /// </summary>
    None = 0,

    /// <summary>
    /// 向上继承(子角色继承父角色权限)
    /// </summary>
    Upward = 1,

    /// <summary>
    /// 向下继承(父角色继承子角色权限)
    /// </summary>
    Downward = 2
}
