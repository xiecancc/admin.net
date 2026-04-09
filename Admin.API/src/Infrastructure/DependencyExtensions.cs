/*
 * 文件名称: DependencyExtensions.cs
 * 功能描述: Infrastructure 层依赖注入扩展类，注册基础设施层所有服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Infrastructure.Contexts;
using Infrastructure.Sugars;
using Infrastructure.Events;
using Infrastructure.Repositories;
using Infrastructure.Caches;
using Infrastructure.Services;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Contexts;
using Infrastructure.Shared.Options;
using Infrastructure.Shared.Utils;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Domain.Shared.Events;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Shared.Units;
using System.Reflection;
using Infrastructure.Shared.Services;

namespace Infrastructure;

/// <summary>
/// 基础设施层依赖注入扩展
/// </summary>
public static class DependencyExtensions {
    /// <summary>
    /// 添加基础设施层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration) {
        _ = services.RegisterOptions(configuration);

        _ = services.AddHttpContextServices();
        _ = services.AddDatabaseServices();
        _ = services.AddDomainEventServices();
        _ = services.AddJwtServices();
        _ = services.AddRepositoryServices();
        _ = services.AddDomainServices();
        _ = services.AddUnitOfWorkServices();
        _ = services.AddCachingServices(configuration);

        return services;
    }

    private static IServiceCollection RegisterOptions(
        this IServiceCollection services,
        IConfiguration configuration) {
        // 数据库配置
        _ = services.AddOption<DatabaseOption>(configuration, "Database");

        // Redis 配置
        _ = services.AddOption<RedisOption>(configuration, "Redis");

        // 内存缓存配置
        _ = services.AddOption<MemoryCacheOption>(configuration, "MemoryCache");

        // JWT 配置
        _ = services.AddOption<JwtOption>(configuration, "Jwt");

        // 限流配置
        _ = services.AddOption<RateLimitOption>(configuration, "RateLimit");

        // 跨域配置
        _ = services.AddOption<CorsOption>(configuration, "Cors");

        // 请求超时配置
        _ = services.AddOption<RequestTimeoutOption>(configuration, "RequestTimeout");

        // 请求体大小限制配置
        _ = services.AddOption<RequestSizeLimitOption>(configuration, "RequestSizeLimit");

        // 遥测配置
        _ = services.AddOption<TelemetryOption>(configuration, "Telemetry");

        return services;
    }

    private static IServiceCollection AddHttpContextServices(this IServiceCollection services) {
        _ = services.AddHttpContextAccessor();
        _ = services.AddScoped<IHttpContextProvider, HttpContextProvider>();
        _ = services.AddScoped<IUserContextProvider, UserContextProvider>();
        return services;
    }

    private static IServiceCollection AddDatabaseServices(this IServiceCollection services) {
        _ = services.AddScoped(sp => {
            DatabaseOption dbOptions = sp.GetRequiredService<IOptions<DatabaseOption>>().Value;
            ILogger<SugarDb>? logger = sp.GetService<ILogger<SugarDb>>();
            return new SugarDb(dbOptions, logger);
        });

        _ = services.AddScoped(sp => sp.GetRequiredService<SugarDb>().GetClient());

        return services;
    }

    private static IServiceCollection AddDomainEventServices(this IServiceCollection services) {
        _ = services.AddSingleton<IDomainEventBus, DomainEventBus>();

        _ = services.ScanAndRegisterDomainEventHandlers(Assembly.GetExecutingAssembly());

        _ = services.AddDomainEventHandlerBinding(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddJwtServices(this IServiceCollection services) {
        _ = services.AddScoped<IJwtService, JwtService>();
        return services;
    }

    private static IServiceCollection AddRepositoryServices(this IServiceCollection services) {
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<IRoleRepository, RoleRepository>();
        _ = services.AddScoped<IMenuPermissionRepository, MenuPermissionRepository>();
        _ = services.AddScoped<IApiPermissionRepository, ApiPermissionRepository>();
        _ = services.AddScoped<IButtonPermissionRepository, ButtonPermissionRepository>();
        _ = services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        _ = services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services) {
        _ = services.AddScoped<IPermissionDomainService, PermissionDomainService>();
        return services;
    }

    private static IServiceCollection AddUnitOfWorkServices(this IServiceCollection services) {
        _ = services.AddScoped<IUnitOfWork, Units.UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration) {
        var memoryOptions = ConfigurationUtil.GetOption<MemoryCacheOption>(configuration, "MemoryCache");
        var redisOptions = ConfigurationUtil.GetOption<RedisOption>(configuration, "Redis");

        _ = services.AddMemoryCache(options => {
            if (memoryOptions.SizeLimit.HasValue) {
                options.SizeLimit = memoryOptions.SizeLimit.Value;
            }
            options.CompactionPercentage = memoryOptions.CompactionPercentage;
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(memoryOptions.ExpirationScanFrequencyMinutes);
        });

        _ = services.AddSingleton<IRedisConnectionManager, RedisConnectionManager>();

        _ = services.AddHybridCache(options => {
            options.DefaultEntryOptions = new HybridCacheEntryOptions {
                Expiration = TimeSpan.FromMinutes(memoryOptions.DefaultExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            };
            options.MaximumPayloadBytes = 1024 * 1024;
        });

        if (redisOptions.Enabled) {
            _ = services.AddStackExchangeRedisCache(options => {
                options.Configuration = redisOptions.ConnectionString;
                options.InstanceName = redisOptions.InstanceName;
            });
        }

        _ = services.AddSingleton<ICacheProvider, HybridCacheProvider>();

        return services;
    }
}
