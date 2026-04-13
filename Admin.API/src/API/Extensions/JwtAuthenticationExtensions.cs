/*
 * 文件名称: JwtAuthenticationExtensions.cs
 * 功能描述: JWT 认证扩展方法，配置 JWT 认证服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Domain.Repositories;
using Infrastructure.Shared.Options;
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
    ///   <item>验证用户是否存在</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services) {
        services.AddAuthorization();

        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

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

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetAsync(userId);
                if (user == null) {
                    context.Fail("用户不存在");
                }
            }
        };
    }
}
