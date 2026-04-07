/*
 * 文件名称: PermissionType.cs
 * 功能描述: 权限类型枚举，定义权限的所有可能类型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Domain.Shared.Enums;

/// <summary>
/// 权限类型枚举
/// <para>定义权限的所有可能类型</para>
/// </summary>
/// <remarks>
/// <para>类型说明：</para>
/// <list type="bullet">
///   <item>Menu：菜单权限，控制用户可访问的菜单</item>
///   <item>Api：API 权限，控制用户可访问的 API 接口</item>
///   <item>Button：按钮权限，控制用户可操作的按钮</item>
/// </list>
/// </remarks>
public enum PermissionType {
    /// <summary>
    /// 菜单权限
    /// </summary>
    Menu = 0,

    /// <summary>
    /// API 权限
    /// </summary>
    Api = 1,

    /// <summary>
    /// 按钮权限
    /// </summary>
    Button = 2
}
