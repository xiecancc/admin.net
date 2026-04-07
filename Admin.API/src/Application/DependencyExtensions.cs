/*
 * 文件名称: DependencyExtensions.cs
 * 功能描述: Application 层依赖注入扩展类，注册应用层所有服务
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

/// <summary>
/// Application 层依赖注入扩展类
/// <para>注册应用层所有服务</para>
/// </summary>
public static class DependencyExtensions {
    /// <summary>
    /// 添加应用层所有服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        _ = services.AddMediatR(config => {
            _ = config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            _ = config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        _ = services.AddAutoMapper(config => {
            config.AddMaps(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
