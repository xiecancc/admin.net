/*
 * 文件名称: Program.cs
 * 功能描述: 应用程序入口类，负责配置和启动 Web API 服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-09
 */

using API.Extensions;
using API.Filters;
using Application;
using Asp.Versioning;
using Infrastructure;
using Infrastructure.Shared.Utils;

namespace API;

/// <summary>
/// 程序入口类
/// <para>负责配置和启动 Web API 服务</para>
/// </summary>
public class Program {
    /// <summary>
    /// 程序入口方法
    /// </summary>
    /// <param name="args">命令行参数</param>
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        await app.RunAsync();
    }

    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="builder">Web 应用构建器</param>
    private static void ConfigureServices(WebApplicationBuilder builder) {
        // 日志服务
        _ = builder.AddSerilogLogging();

        // 应用层服务
        _ = builder.Services.AddApplicationServices();

        // 基础设施层服务
        _ = builder.Services.AddInfrastructureServices(builder.Configuration);

        // 限流服务
        _ = builder.Services.AddRateLimitingServices();

        // JWT 认证服务
        _ = builder.Services.AddJwtAuthentication();

        // 权限校验服务
        _ = builder.Services.AddHttpContextAccessor();
        _ = builder.Services.AddScoped<PermissionAuthorizationHandler>();

        // 授权服务
        _ = builder.Services.AddAuthorization();

        // Swagger 文档服务
        _ = builder.Services.AddSwaggerServices();

        // 健康检查服务
        _ = builder.Services.AddHealthCheckServices();

        // 性能优化服务（响应压缩、输出缓存、响应缓存）
        _ = builder.Services.AddPerformanceServices();

        // OpenTelemetry 遥测服务
        _ = builder.Services.AddTelemetryServices(builder.Configuration);

        // 请求超时服务
        _ = builder.Services.AddRequestTimeoutServices(builder.Configuration);

        // 请求体大小限制服务
        _ = builder.Services.AddRequestSizeLimitServices(builder.Configuration);

        // 跨域服务
        _ = builder.Services.AddCorsServices();

        // MVC 控制器服务
        _ = builder.Services.AddControllers(options => {
            _ = options.Filters.Add<ResponseFilter>();
        })
            .AddJsonOptions(options => {
                foreach (var converter in JsonUtil.DefaultOptions.Converters) {
                    options.JsonSerializerOptions.Converters.Add(converter);
                }
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonUtil.DefaultOptions.PropertyNamingPolicy;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonUtil.DefaultOptions.DefaultIgnoreCondition;
            });

        // API 版本控制服务
        _ = builder.Services.AddApiVersioning(options => {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new QueryStringApiVersionReader("api-version")
            );
        })
            .AddApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    /// <summary>
    /// 配置中间件管道
    /// </summary>
    /// <remarks>
    /// <para>中间件顺序遵循 ASP.NET Core 最佳实践：</para>
    /// <list type="number">
    ///   <item>日志中间件 - 最先记录请求日志</item>
    ///   <item>开发环境中间件（Swagger/HSTS）- 环境特定配置</item>
    ///   <item>安全头部中间件 - 添加安全相关 HTTP 头</item>
    ///   <item>健康检查端点 - 不需要认证的健康检查</item>
    ///   <item>跨域中间件 - 处理跨域请求</item>
    ///   <item>HTTPS 重定向 - 强制 HTTPS</item>
    ///   <item>性能优化中间件 - 响应压缩、缓存</item>
    ///   <item>静态资源 - 提供静态文件服务</item>
    ///   <item>请求超时 - 控制请求处理时间</item>
    ///   <item>认证中间件 - JWT 令牌验证</item>
    ///   <item>限流中间件 - 请求频率限制</item>
    ///   <item>授权中间件 - 权限验证</item>
    ///   <item>控制器路由 - 最终处理请求</item>
    /// </list>
    /// </remarks>
    /// <param name="app">Web 应用程序</param>
    private static void ConfigureMiddleware(WebApplication app) {
        // 1. 日志中间件 - 最先记录请求日志
        _ = app.UseSerilogLogging();

        // 2. 开发环境中间件
        if (app.Environment.IsDevelopment()) {
            _ = app.UseSwaggerMiddlewares();
        }
        else {
            // 生产环境启用 HSTS
            _ = app.UseHsts();
        }

        // 3. 安全头部中间件 - 添加安全相关 HTTP 头
        _ = app.UseSecurityHeaders();

        // 4. 健康检查端点 - 不需要认证的健康检查
        _ = app.UseHealthCheckEndpoints();

        // 5. 跨域中间件 - 处理跨域请求
        _ = app.UseCors();

        // 6. HTTPS 重定向 - 强制 HTTPS
        _ = app.UseHttpsRedirection();

        // 7. 性能优化中间件 - 响应压缩、输出缓存、响应缓存
        _ = app.UsePerformanceMiddlewares();

        // 8. 静态资源 - 提供静态文件服务
        _ = app.MapOptimizedStaticAssets();

        // 9. 请求超时 - 控制请求处理时间
        _ = app.UseRequestTimeoutMiddleware();

        // 10. 认证中间件 - JWT 令牌验证
        _ = app.UseAuthentication();

        // 11. 限流中间件 - 请求频率限制（仅在启用时使用）
        var rateLimitOption = app.Services.GetService<Microsoft.Extensions.Options.IOptions<Infrastructure.Shared.Options.RateLimitOption>>()?.Value;
        if (rateLimitOption?.Enabled == true) {
            _ = app.UseRateLimiter();
        }

        // 12. 授权中间件 - 权限验证
        _ = app.UseAuthorization();

        // 13. 控制器路由 - 最终处理请求
        _ = app.MapControllers();
    }
}
