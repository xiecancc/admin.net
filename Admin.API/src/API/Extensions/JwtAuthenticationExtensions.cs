/*
 * 文件名称: JwtAuthenticationExtensions.cs
 * 功能描述: JWT 认证扩展方法，配置 JWT 认证服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Enums;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Options;
using Infrastructure.Shared.Units;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

/// <summary>
/// JWT 认证扩展方法
/// <para>配置 JWT 认证服务</para>
/// </summary>
public static class JwtAuthenticationExtensions {
    /// <summary>
    /// 添加 JWT 认证服务
    /// <para>配置 JWT Bearer 认证，包括令牌验证和用户状态检查</para>
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    /// <remarks>
    /// <para>认证流程：</para>
    /// <list type="number">
    ///   <item>验证 JWT 令牌的签名、颁发者、受众和有效期</item>
    ///   <item>从令牌中提取用户 ID</item>
    ///   <item>优先从缓存获取用户状态</item>
    ///   <item>缓存未命中时从数据库获取用户状态并缓存</item>
    ///   <item>验证用户状态是否为正常状态</item>
    /// </list>
    /// <para>用户状态缓存策略：</para>
    /// <list type="bullet">
    ///   <item>缓存键格式：user_status:{userId}</item>
    ///   <item>缓存过期时间：5 分钟</item>
    ///   <item>仅缓存状态为 Normal、Disabled、Locked 的用户</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services) {
        _ = services.AddAuthorization();

        _ = services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        _ = services.ConfigureOptions<ConfigureJwtBearerOptions>();

        return services;
    }
}

/// <summary>
/// 配置 JWT Bearer 选项
/// <para>从 DI 容器注入 JwtOption 配置</para>
/// </summary>
/// <remarks>
/// <para>实现 IConfigureNamedOptions&lt;JwtBearerOptions&gt; 以配置 JWT Bearer 认证选项</para>
/// <para>配置会在以下情况被应用：</para>
/// <list type="bullet">
///   <item>name 为 "Bearer" 时（默认 scheme）</item>
///   <item>name 为空或 null 时（默认配置）</item>
/// </list>
/// </remarks>
public sealed class ConfigureJwtBearerOptions(IOptions<JwtOption> jwtOption) : IConfigureNamedOptions<JwtBearerOptions> {
    private readonly JwtOption _jwtOption = jwtOption.Value;

    /// <summary>
    /// 配置 JWT Bearer 选项（默认配置）
    /// </summary>
    public void Configure(JwtBearerOptions options) {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    /// <summary>
    /// 配置 JWT Bearer 选项（命名配置）
    /// </summary>
    /// <param name="name">认证方案名称</param>
    /// <param name="options">JWT Bearer 选项</param>
    public void Configure(string? name, JwtBearerOptions options) {
        if (name != JwtBearerDefaults.AuthenticationScheme && !string.IsNullOrEmpty(name)) {
            return;
        }

        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOption.Issuer,
            ValidAudience = _jwtOption.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents {
            OnTokenValidated = async context => {
                var userIdClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId)) {
                    context.Fail("无效的用户标识");
                    return;
                }

                var cacheProvider = context.HttpContext.RequestServices.GetRequiredService<ICacheProvider>();
                var cacheKey = $"user_status:{userId}";
                var userStatus = await cacheProvider.GetAsync<UserStatus?>(cacheKey);

                if (userStatus == null) {
                    var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                    var userRepository = unitOfWork.GetRepository<IUserRepository, User>();
                    var user = await userRepository.GetAsync(userId);
                    userStatus = user?.Status;

                    if (userStatus.HasValue) {
                        await cacheProvider.SetAsync(cacheKey, userStatus.Value, TimeSpan.FromMinutes(5));
                    }
                }

                if (userStatus != UserStatus.Normal) {
                    context.Fail("用户已禁用");
                }
            }
        };
    }
}
