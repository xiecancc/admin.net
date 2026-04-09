/*
 * 文件名称: JwtUtil.cs
 * 功能描述: JWT 工具类，提供令牌生成、验证、解析等功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Shared.Utils;

/// <summary>
/// JWT 工具类
/// <para>提供令牌生成、验证、解析等功能</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>生成 JWT Token</item>
///   <item>验证 JWT Token</item>
///   <item>从 Token 中获取声明</item>
///   <item>从 Token 中获取特定类型的声明值</item>
///   <item>检查 Token 是否过期</item>
///   <item>生成刷新令牌</item>
///   <item>清除密钥缓存</item>
/// </list>
/// <para>线程安全性：</para>
/// <list type="bullet">
///   <item>所有公共方法都是线程安全的</item>
///   <item>使用 ConcurrentDictionary 缓存密钥，支持并发访问</item>
///   <item>JwtSecurityTokenHandler 每次调用时创建新实例，无共享状态</item>
///   <item>可在多线程环境中安全使用</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 生成 JWT Token
/// var claims = new[]
/// {
///     new Claim(ClaimTypes.NameIdentifier, "123"),
///     new Claim(ClaimTypes.Name, "alice")
/// };
/// var token = JwtUtil.GenerateToken(claims, "your-secret-key", "issuer", "audience", 60);
/// 
/// // 验证 Token
/// var principal = JwtUtil.ValidateToken(token, "your-secret-key", "issuer", "audience");
/// 
/// // 获取声明值
/// var userId = JwtUtil.GetClaimValue(token, ClaimTypes.NameIdentifier);
/// 
/// // 检查是否过期
/// var isExpired = JwtUtil.IsTokenExpired(token);
/// 
/// // 生成刷新令牌
/// var refreshToken = JwtUtil.GenerateRefreshToken();
/// </code>
/// </example>
public static class JwtUtil {
    /// <summary>
    /// 密钥缓存，避免重复创建 SymmetricSecurityKey
    /// </summary>
    private static readonly ConcurrentDictionary<string, SymmetricSecurityKey> _keyCache = new();

    /// <summary>
    /// 获取或创建 SymmetricSecurityKey（带缓存）
    /// </summary>
    /// <param name="secretKey">密钥字符串</param>
    /// <returns>对称安全密钥</returns>
    private static SymmetricSecurityKey GetOrCreateKey(string secretKey) {
        return _keyCache.GetOrAdd(secretKey, key => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)));
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="claims">声明列表</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">签发者</param>
    /// <param name="audience">受众</param>
    /// <param name="expiresMinutes">过期时间（分钟），默认为 60 分钟</param>
    /// <returns>JWT Token 字符串</returns>
    public static string GenerateToken(
        IEnumerable<Claim> claims,
        string secretKey,
        string issuer,
        string audience,
        int expiresMinutes = 60) {
        var key = GetOrCreateKey(secretKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证 JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">签发者</param>
    /// <param name="audience">受众</param>
    /// <returns>验证结果（包含 ClaimsPrincipal），验证失败返回 null</returns>
    public static ClaimsPrincipal? ValidateToken(
        string token,
        string secretKey,
        string issuer,
        string audience) {
        var key = GetOrCreateKey(secretKey);

        var tokenHandler = new JwtSecurityTokenHandler();

        try {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch (SecurityTokenException) {
            return null;
        }
        catch (ArgumentException) {
            return null;
        }
    }

    /// <summary>
    /// 从 Token 中获取声明
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>声明列表，解析失败返回 null</returns>
    public static IEnumerable<Claim>? GetClaimsFromToken(string token) {
        var handler = new JwtSecurityTokenHandler();

        return handler.ReadToken(token) is JwtSecurityToken jwtToken ? jwtToken.Claims : (IEnumerable<Claim>?)null;
    }

    /// <summary>
    /// 从 Token 中获取特定类型的声明值
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="claimType">声明类型</param>
    /// <returns>声明值，不存在返回 null</returns>
    public static string? GetClaimValue(string token, string claimType) {
        var claims = GetClaimsFromToken(token);
        return claims?.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    /// <summary>
    /// 检查 Token 是否过期
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>是否过期</returns>
    public static bool IsTokenExpired(string token) {
        var claims = GetClaimsFromToken(token);
        var expClaim = claims?.FirstOrDefault(c => c.Type == "exp");

        if (expClaim == null) {
            return true;
        }

        var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value, System.Globalization.CultureInfo.InvariantCulture));
        return expTime.UtcDateTime < DateTime.UtcNow;
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌（Base64 编码的随机字符串）</returns>
    public static string GenerateRefreshToken() {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// 清除密钥缓存（用于密钥轮换场景）
    /// </summary>
    public static void ClearKeyCache() {
        _keyCache.Clear();
    }
}
