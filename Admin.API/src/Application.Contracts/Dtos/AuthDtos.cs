/*
 * 文件名称: AuthDtos.cs
 * 功能描述: 认证相关的数据传输对象，包含登录、注册、刷新令牌等请求和响应模型
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

namespace Application.Contracts.Dtos;

/// <summary>
/// 登录请求 DTO
/// <para>用于用户登录认证</para>
/// </summary>
public class LoginRequestDTO {
    /// <summary>
    /// 邮箱（登录账号）
    /// </summary>
    /// <value>用户的邮箱地址，不能为空</value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <value>用户的密码，不能为空</value>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 记住我
    /// </summary>
    /// <value>是否记住用户登录状态，默认为 false</value>
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// 登录响应 DTO
/// <para>登录成功后返回的响应数据</para>
/// </summary>
public class LoginResponseDTO {
    /// <summary>
    /// 访问令牌
    /// </summary>
    /// <value>用于访问受保护资源的令牌</value>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <value>用于刷新访问令牌的令牌</value>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 令牌类型（通常为 Bearer）
    /// </summary>
    /// <value>令牌的类型，默认为 "Bearer"</value>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// 过期时间（秒）
    /// </summary>
    /// <value>访问令牌的过期时间，单位为秒</value>
    public int ExpiresIn {
        get; set;
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    /// <value>登录用户的详细信息</value>
    public LoginUserInfoDTO User { get; set; } = new();
}

/// <summary>
/// 登录用户信息 DTO
/// <para>包含用户的基本信息和角色，用于登录响应</para>
/// </summary>
public class LoginUserInfoDTO {
    /// <summary>
    /// 用户 ID
    /// </summary>
    /// <value>用户的唯一标识符</value>
    public Guid Id {
        get; set;
    }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户的邮箱地址</value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 头像 URL
    /// </summary>
    /// <value>用户的头像地址，可以为空</value>
    public string? Avatar {
        get; set;
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <value>用户拥有的角色名称列表</value>
    public List<string> Roles { get; set; } = [];
}

/// <summary>
/// 刷新令牌请求 DTO
/// <para>用于刷新访问令牌</para>
/// </summary>
public class RefreshTokenRequestDTO {
    /// <summary>
    /// 访问令牌
    /// </summary>
    /// <value>当前的访问令牌</value>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <value>用于刷新访问令牌的令牌</value>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 注册请求 DTO
/// <para>用于用户注册</para>
/// </summary>
public class RegisterRequestDTO {
    /// <summary>
    /// 邮箱
    /// </summary>
    /// <value>用户的邮箱地址，不能为空</value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <value>用户的密码，不能为空</value>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    /// <value>用户的昵称，可以为空</value>
    public string? NickName {
        get; set;
    }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <value>用户的手机号码，可以为空</value>
    public string? Phone {
        get; set;
    }
}
