/*
 * 文件名称: User.cs
 * 功能描述: 用户实体类，定义用户的基本属性和关联关系
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Domain.Shared.Entities;
using Domain.Shared.Enums;

namespace Domain.Entities;

/// <summary>
/// 用户实体
/// <para>定义用户的基本属性和关联关系</para>
/// </summary>
/// <remarks>
/// <para>主要属性：</para>
/// <list type="bullet">
///   <item>Email：邮箱，作为登录账号</item>
///   <item>PasswordHash：密码哈希值</item>
///   <item>Status：用户状态</item>
///   <item>NickName：用户昵称</item>
///   <item>AvatarUrl：头像URL</item>
///   <item>Phone：手机号</item>
/// </list>
/// <para>关联关系：</para>
/// <list type="bullet">
///   <item>与 Role 多对多关系，通过 UserRole 中间表</item>
/// </list>
/// </remarks>
[SugarTable("Users", "用户表")]
[SugarIndex("IX_Users_Email", nameof(Email), OrderByType.Asc, true)]
[SugarIndex("IX_Users_Status", nameof(Status), OrderByType.Asc)]
public class User : AggregateBase {
    /// <summary>
    /// 邮箱（登录账号）
    /// </summary>
    /// <value>邮箱地址，长度不超过100个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "邮箱", Length = 100, IsNullable = false)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码哈希
    /// </summary>
    /// <value>密码的哈希值，长度不超过255个字符，不能为空</value>
    [SugarColumn(ColumnDescription = "密码哈希", Length = 255, IsNullable = false)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 用户状态
    /// </summary>
    /// <value>用户的状态，默认为正常状态</value>
    [SugarColumn(ColumnDescription = "用户状态", DefaultValue = "0", IsNullable = false)]
    public UserStatus Status { get; set; } = UserStatus.Normal;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，长度不超过50个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "昵称", Length = 50, IsNullable = true)]
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户头像的URL地址，长度不超过500个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "头像 URL", Length = 500, IsNullable = true)]
    public string? AvatarUrl {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，长度不超过20个字符，可以为空</value>
    [SugarColumn(ColumnDescription = "手机号", Length = 20, IsNullable = true)]
    public string? Phone {
        get; set;
    }

    /// <summary>
    /// 用户关联的角色（多对多）
    /// </summary>
    /// <value>用户所属的角色列表</value>
    [Navigate(typeof(UserRole), nameof(UserRole.UserId), nameof(UserRole.RoleId))]
    public List<Role> Roles { get; set; } = [];


}
