/*
 * 文件名称: RoleConstants.cs
 * 功能描述: 角色常量定义，包含角色相关的常量配置
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Domain.Shared.Constants;

/// <summary>
/// 角色常量定义
/// <para>包含角色相关的常量配置</para>
/// </summary>
/// <remarks>
/// <para>主要包含：</para>
/// <list type="bullet">
///   <item>超级管理员配置</item>
///   <item>普通管理员配置</item>
///   <item>普通用户配置</item>
///   <item>内置角色查询方法</item>
/// </list>
/// </remarks>
public static class RoleConstants {

    /// <summary>
    /// 超级管理员
    /// </summary>
    public static class Administrator {
        /// <summary>
        /// 编号
        /// </summary>
        public static readonly Guid Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        /// <summary>
        /// 编码
        /// </summary>
        public const string CODE = "Administrator";
        /// <summary>
        /// 名称
        /// </summary>
        public const string NAME = "超级管理员";
    }

    /// <summary>
    /// 普通管理员
    /// </summary>
    public static class Manager {
        /// <summary>
        /// 编号
        /// </summary>
        public static readonly Guid Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
        /// <summary>
        /// 编码
        /// </summary>
        public const string CODE = "Manager";
        /// <summary>
        /// 名称
        /// </summary>
        public const string NAME = "普通管理员";
    }

    /// <summary>
    /// 普通用户
    /// </summary>
    public static class User {
        /// <summary>
        /// 编号
        /// </summary>
        public static readonly Guid Id = Guid.Parse("00000000-0000-0000-0000-000000000003");
        /// <summary>
        /// 编码
        /// </summary>
        public const string CODE = "User";
        /// <summary>
        /// 名称
        /// </summary>
        public const string NAME = "普通用户";
    }

    /// <summary>
    /// 获取所有内置角色编码
    /// </summary>
    public static string[] BuiltInRoles => [
        Administrator.CODE,
        Manager.CODE,
        User.CODE
    ];

    /// <summary>
    /// 获取所有内置角色 ID
    /// </summary>
    public static Guid[] BuiltInRoleIds => [
        Administrator.Id,
        Manager.Id,
        User.Id
    ];

    /// <summary>
    /// 根据角色编码获取角色 ID
    /// </summary>
    public static Guid GetRoleIdByCode(string code) {
        return code switch {
            Administrator.CODE => Administrator.Id,
            Manager.CODE => Manager.Id,
            User.CODE => User.Id,
            _ => throw new ArgumentException($"未知的角色编码：{code}")
        };
    }

    /// <summary>
    /// 根据角色编码获取角色名称
    /// </summary>
    public static string GetRoleNameByCode(string code) {
        return code switch {
            Administrator.CODE => Administrator.NAME,
            Manager.CODE => Manager.NAME,
            User.CODE => User.NAME,
            _ => code
        };
    }
}
