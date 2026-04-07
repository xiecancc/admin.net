/*
 * 文件名称: UserConstants.cs
 * 功能描述: 用户常量定义，包含用户相关的常量配置
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

namespace Domain.Shared.Constants;

/// <summary>
/// 用户常量定义
/// <para>包含用户相关的常量配置</para>
/// </summary>
/// <remarks>
/// <para>主要包含：</para>
/// <list type="bullet">
///   <item>系统管理员默认账号配置</item>
///   <item>普通用户默认配置</item>
///   <item>默认密码获取方法</item>
/// </list>
/// </remarks>
public static class UserConstants {
    /// <summary>
    /// 系统管理员默认账号
    /// </summary>
    public static class Administrator {
        /// <summary>
        /// 编号
        /// </summary>
        public static readonly Guid Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string EMAIL = "admin@example.com";
        /// <summary>
        /// 默认密码
        /// </summary>
        public const string PASSWORD = "Admin@123456!";
        /// <summary>
        /// 昵称
        /// </summary>
        public const string NICK_NAME = "超级管理员";
    }

    /// <summary>
    /// 普通用户默认配置
    /// </summary>
    public static class User {
        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DEFAULT_PASSWORD = "user@123!";
    }

    /// <summary>
    /// 获取默认密码
    /// </summary>
    public static string GetDefaultPassword(string? roleCode = null) {
        return roleCode == RoleConstants.Administrator.CODE ? Administrator.PASSWORD : User.DEFAULT_PASSWORD;
    }
}
