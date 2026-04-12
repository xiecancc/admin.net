/*
 * 文件名称: DependencyExtensions.cs
 * 功能描述: Infrastructure 层依赖注入扩展类，注册基础设施层所有服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
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
using Microsoft.Extensions.Hosting;
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
        services.RegisterOptions(configuration);

        services.AddHttpContextServices();
        services.AddDatabaseServices();
        services.AddDomainEventServices();
        services.AddJwtServices();
        services.AddRepositoryServices();
        services.AddDomainServices();
        services.AddUnitOfWorkServices();
        services.AddCachingServices(configuration);
        services.AddCacheWarmupServices();

        return services;
    }

    private static IServiceCollection RegisterOptions(
        this IServiceCollection services,
        IConfiguration configuration) {
        services.AddOption<DatabaseOption>(configuration, "Database");

        services.AddOption<RedisOption>(configuration, "Redis");

        services.AddOption<MemoryCacheOption>(configuration, "MemoryCache");

        services.AddOption<HybridCacheOption>(configuration, "HybridCache");

        services.AddOption<JwtOption>(configuration, "Jwt");

        services.AddOption<RateLimitOption>(configuration, "RateLimit");

        services.AddOption<CorsOption>(configuration, "Cors");

        services.AddOption<RequestTimeoutOption>(configuration, "RequestTimeout");

        services.AddOption<RequestSizeLimitOption>(configuration, "RequestSizeLimit");

        services.AddOption<TelemetryOption>(configuration, "Telemetry");

        return services;
    }

    private static IServiceCollection AddHttpContextServices(this IServiceCollection services) {
        services.AddHttpContextAccessor();
        services.AddScoped<IHttpContextProvider, HttpContextProvider>();
        services.AddScoped<IUserContextProvider, UserContextProvider>();
        return services;
    }

    private static IServiceCollection AddDatabaseServices(this IServiceCollection services) {
        services.AddScoped(sp => {
            DatabaseOption dbOptions = sp.GetRequiredService<IOptions<DatabaseOption>>().Value;
            ILogger<SugarDb>? logger = sp.GetService<ILogger<SugarDb>>();
            return new SugarDb(dbOptions, logger);
        });

        services.AddScoped(sp => sp.GetRequiredService<SugarDb>().GetClient());

        return services;
    }

    private static IServiceCollection AddDomainEventServices(this IServiceCollection services) {
        services.AddSingleton<IDomainEventBus, DomainEventBus>();

        services.ScanAndRegisterDomainEventHandlers(Assembly.GetExecutingAssembly());

        services.AddDomainEventHandlerBinding(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddJwtServices(this IServiceCollection services) {
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }

    private static IServiceCollection AddRepositoryServices(this IServiceCollection services) {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IMenuPermissionRepository, MenuPermissionRepository>();
        services.AddScoped<IApiPermissionRepository, ApiPermissionRepository>();
        services.AddScoped<IButtonPermissionRepository, ButtonPermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services) {
        services.AddScoped<IPermissionDomainService, PermissionDomainService>();
        services.AddScoped<IPermissionCacheService, PermissionCacheService>();
        return services;
    }

    private static IServiceCollection AddUnitOfWorkServices(this IServiceCollection services) {
        services.AddScoped<IUnitOfWork, Units.UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration) {
        var memoryOptions = ConfigurationUtil.GetOption<MemoryCacheOption>(configuration, "MemoryCache");
        var redisOptions = ConfigurationUtil.GetOption<RedisOption>(configuration, "Redis");
        var hybridOptions = ConfigurationUtil.GetOption<HybridCacheOption>(configuration, "HybridCache");

        services.AddMemoryCache(options => {
            if (memoryOptions.SizeLimit.HasValue) {
                options.SizeLimit = memoryOptions.SizeLimit.Value;
            }
            options.CompactionPercentage = memoryOptions.CompactionPercentage;
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(memoryOptions.ExpirationScanFrequencyMinutes);
        });

        services.AddSingleton<IRedisConnectionManager, RedisConnectionManager>();

        services.AddHybridCache(options => {
            options.DefaultEntryOptions = new HybridCacheEntryOptions {
                Expiration = TimeSpan.FromMinutes(hybridOptions.DefaultExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            };
            options.MaximumPayloadBytes = 1024 * 1024;
        });

        if (redisOptions.Enabled) {
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = redisOptions.ConnectionString;
                options.InstanceName = redisOptions.InstanceName;
            });
        }

        services.AddSingleton<ICacheProvider, HybridCacheProvider>();

        return services;
    }

    /// <summary>
    /// 添加缓存预热服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    private static IServiceCollection AddCacheWarmupServices(this IServiceCollection services) {
        services.AddHostedService<PermissionCacheWarmupService>();
        return services;
    }
}
