/*
 * 文件名称: AuthCommands.cs
 * 功能描述: 认证相关的命令类，包含登录、注册、刷新令牌、登出等命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;

namespace Application.Contracts.Commands;

/// <summary>
/// 用户登录命令
/// <para>用于用户登录认证</para>
/// </summary>
/// <param name="email">邮箱</param>
/// <param name="password">密码</param>
public class LoginCommand(string email, string password) : Command<LoginResponseDto> {
    /// <summary>邮箱</summary>
    public string Email { get; set; } = email;
    /// <summary>密码</summary>
    public string Password { get; set; } = password;
}

/// <summary>
/// 用户注册命令
/// <para>用于创建新用户账号</para>
/// </summary>
/// <param name="email">邮箱</param>
/// <param name="password">密码</param>
/// <param name="nickName">昵称</param>
/// <param name="phone">手机号</param>
public class RegisterCommand(string email, string password, string? nickName, string? phone) : Command<bool> {
    /// <summary>邮箱</summary>
    public string Email { get; set; } = email;
    /// <summary>密码</summary>
    public string Password { get; set; } = password;
    /// <summary>昵称</summary>
    public string? NickName { get; set; } = nickName;
    /// <summary>手机号</summary>
    public string? Phone { get; set; } = phone;
}

/// <summary>
/// 刷新令牌命令
/// <para>用于刷新访问令牌</para>
/// </summary>
/// <param name="accessToken">访问令牌</param>
/// <param name="refreshToken">刷新令牌</param>
public class RefreshTokenCommand(string accessToken, string refreshToken) : Command<LoginResponseDto> {
    /// <summary>访问令牌</summary>
    public string AccessToken { get; set; } = accessToken;
    /// <summary>刷新令牌</summary>
    public string RefreshToken { get; set; } = refreshToken;
}

/// <summary>
/// 用户登出命令
/// <para>用于用户登出，将令牌加入黑名单</para>
/// </summary>
/// <param name="token">JWT 令牌</param>
public class LogoutCommand(string token) : Command<bool> {
    /// <summary>JWT 令牌</summary>
    public string Token { get; set; } = token;
}
