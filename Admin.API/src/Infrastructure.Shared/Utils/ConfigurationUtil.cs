/*
 * 文件名称: ConfigurationUtil.cs
 * 功能描述: 配置工具类，封装配置读取和验证功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-09
 */

using Infrastructure.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 配置工具类
/// <para>封装配置读取和验证功能，提供类型安全的配置获取方法</para>
/// </summary>
public static class ConfigurationUtil {
    /// <summary>
    /// 从配置中获取指定类型的选项
    /// </summary>
    /// <typeparam name="TOption">选项类型，必须继承自 OptionBase</typeparam>
    /// <param name="configuration">配置对象</param>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>配置选项对象</returns>
    /// <exception cref="ArgumentNullException">当 configuration 或 sectionName 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    /// <example>
    /// <code>
    /// var dbOptions = ConfigurationUtil.GetOption&lt;DatabaseOption&gt;(configuration, "Database");
    /// var redisOptions = ConfigurationUtil.GetOption&lt;RedisOption&gt;(configuration, "Redis");
    /// </code>
    /// </example>
    public static TOption GetOption<TOption>(IConfiguration configuration, string sectionName) 
        where TOption : OptionBase, new() {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        var section = configuration.GetSection(sectionName);
        
        if (!section.Exists()) {
            throw new InvalidOperationException($"配置节 '{sectionName}' 不存在");
        }
        
        var option = section.Get<TOption>() ?? new TOption();
        option.Validate();
        return option;
    }

    /// <summary>
    /// 从配置中获取指定类型的选项，并注册到服务容器
    /// </summary>
    /// <typeparam name="TOption">选项类型，必须继承自 OptionBase</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置对象</param>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>服务集合</returns>
    /// <exception cref="ArgumentNullException">当 services、configuration 或 sectionName 为 null 时抛出</exception>
    /// <exception cref="InvalidOperationException">配置无效时抛出异常</exception>
    /// <example>
    /// <code>
    /// services.AddOption&lt;DatabaseOption&gt;(configuration, "Database");
    /// services.AddOption&lt;RedisOption&gt;(configuration, "Redis");
    /// </code>
    /// </example>
    public static IServiceCollection AddOption<TOption>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName) 
        where TOption : OptionBase, new() {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        var option = GetOption<TOption>(configuration, sectionName);
        services.Configure<TOption>(options => configuration.GetSection(sectionName).Bind(options));
        return services;
    }
}