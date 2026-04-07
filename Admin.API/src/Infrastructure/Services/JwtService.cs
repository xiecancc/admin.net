/*
 * 文件名称: JwtService.cs
 * 功能描述: JWT服务实现
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-25
 */

using System.Security.Claims;
using Infrastructure.Shared.Options;
using Infrastructure.Shared.Services;
using Infrastructure.Shared.Utils;
using Infrastructure.Shared.Caches;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// JWT服务实现
/// <para>封装JWT令牌的生成、验证和管理功能</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>生成JWT令牌</item>
///   <item>验证JWT令牌</item>
///   <item>管理令牌黑名单</item>
///   <item>生成刷新令牌</item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 在依赖注入中使用
/// public class AuthService
/// {
///     private readonly IJwtService _jwtService;
///     
///     public AuthService(IJwtService jwtService)
///     {
///         _jwtService = jwtService;
///     }
///     
///     public string GenerateToken(User user)
///     {
///         var claims = new[]
///         {
///             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
///             new Claim(ClaimTypes.Name, user.Email)
///         };
///         return _jwtService.GenerateToken(claims);
///     }
///     
///     public async Task&lt;ClaimsPrincipal?&gt; ValidateTokenAsync(string token)
///     {
///         return await _jwtService.ValidateTokenAsync(token);
///     }
/// }
/// </code>
/// </example>
/// <param name="jwtOption">JWT配置选项</param>
/// <param name="cacheProvider">缓存提供者</param>
public class JwtService(IOptions<JwtOption> jwtOption, ICacheProvider cacheProvider) : IJwtService {
    private readonly JwtOption _jwtOption = jwtOption.Value;
    private readonly ICacheProvider _cacheProvider = cacheProvider;
    private const string BLACKLIST_PREFIX = "jwt_blacklist:";

    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    /// <param name="claims">声明列表</param>
    /// <returns>JWT令牌字符串</returns>
    public string GenerateToken(IEnumerable<Claim> claims) {
        return JwtUtil.GenerateToken(
            claims,
            _jwtOption.SecretKey,
            _jwtOption.Issuer,
            _jwtOption.Audience,
            _jwtOption.ExpiresInMinutes
        );
    }

    /// <summary>
    /// 验证JWT令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>验证结果（包含ClaimsPrincipal）</returns>
    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token) {
        // 检查令牌是否在黑名单中
        return await IsTokenInBlacklistAsync(token)
            ? null
            : JwtUtil.ValidateToken(
            token,
            _jwtOption.SecretKey,
            _jwtOption.Issuer,
            _jwtOption.Audience
        );
    }

    /// <summary>
    /// 验证JWT令牌（同步版本）
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>验证结果（包含ClaimsPrincipal）</returns>
    public ClaimsPrincipal? ValidateToken(string token) {
        // 同步版本，不检查黑名单
        return JwtUtil.ValidateToken(
            token,
            _jwtOption.SecretKey,
            _jwtOption.Issuer,
            _jwtOption.Audience
        );
    }

    /// <summary>
    /// 从令牌中获取声明
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>声明列表</returns>
    public IEnumerable<Claim>? GetClaimsFromToken(string token) {
        return JwtUtil.GetClaimsFromToken(token);
    }

    /// <summary>
    /// 从令牌中获取特定类型的声明值
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <param name="claimType">声明类型</param>
    /// <returns>声明值</returns>
    public string? GetClaimValue(string token, string claimType) {
        return JwtUtil.GetClaimValue(token, claimType);
    }

    /// <summary>
    /// 检查令牌是否过期
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否过期</returns>
    public bool IsTokenExpired(string token) {
        return JwtUtil.IsTokenExpired(token);
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌（Base64编码的随机字符串）</returns>
    public string GenerateRefreshToken() {
        return JwtUtil.GenerateRefreshToken();
    }

    /// <summary>
    /// 将令牌添加到黑名单
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <param name="expiration">令牌过期时间</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AddTokenToBlacklistAsync(string token, DateTime expiration) {
        try {
            var cacheKey = $"{BLACKLIST_PREFIX}{token}";
            var expirationTime = expiration - DateTime.UtcNow;

            if (expirationTime > TimeSpan.Zero) {
                await _cacheProvider.SetAsync(cacheKey, true, expirationTime);
            }

            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    /// <summary>
    /// 检查令牌是否在黑名单中
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否在黑名单中</returns>
    public async Task<bool> IsTokenInBlacklistAsync(string token) {
        try {
            var cacheKey = $"{BLACKLIST_PREFIX}{token}";
            var result = await _cacheProvider.GetAsync<bool>(cacheKey);
            return result;
        }
        catch (Exception) {
            return false;
        }
    }

    /// <summary>
    /// 从黑名单中移除令牌
    /// </summary>
    /// <param name="token">JWT令牌</param>
    /// <returns>是否成功</returns>
    public async Task<bool> RemoveTokenFromBlacklistAsync(string token) {
        try {
            var cacheKey = $"{BLACKLIST_PREFIX}{token}";
            await _cacheProvider.RemoveAsync(cacheKey);
            return true;
        }
        catch (Exception) {
            return false;
        }
    }
}
