/*
 * 文件名称: IJwtService.cs
 * 功能描述: JWT服务接口
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-25
 */

using System.Security.Claims;

namespace Domain.Shared.Services;

/// <summary>
/// JWT服务接口
/// <para>提供JWT令牌的生成、验证和管理功能</para>
/// </summary>
public interface IJwtService {
    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    /// <param name="claims">声明列表</param>
    /// <returns>JWT令牌字符串</returns>
    string GenerateToken(IEnumerable<Claim> claims);

    /// <summary>
    /// 验证JWT令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>验证结果（包含ClaimsPrincipal）</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// 验证JWT令牌（异步版本，包含黑名单检查）
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>验证结果（包含ClaimsPrincipal）</returns>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);

    /// <summary>
    /// 从令牌中获取声明
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>声明列表</returns>
    IEnumerable<Claim>? GetClaimsFromToken(string token);

    /// <summary>
    /// 从令牌中获取特定类型的声明值
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <param name="claimType">声明类型</param>
    /// <returns>声明值</returns>
    string? GetClaimValue(string token, string claimType);

    /// <summary>
    /// 检查令牌是否过期
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否过期</returns>
    bool IsTokenExpired(string token);

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌（Base64编码的随机字符串）</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// 将令牌添加到黑名单
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <param name="expiration">令牌过期时间</param>
    /// <returns>是否成功</returns>
    Task<bool> AddTokenToBlacklistAsync(string token, DateTime expiration);

    /// <summary>
    /// 检查令牌是否在黑名单中
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否在黑名单中</returns>
    Task<bool> IsTokenInBlacklistAsync(string token);

    /// <summary>
    /// 从黑名单中移除令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否成功</returns>
    Task<bool> RemoveTokenFromBlacklistAsync(string token);
}
