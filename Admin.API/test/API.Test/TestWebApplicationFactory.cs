/*
 * 文件名称: TestWebApplicationFactory.cs
 * 功能描述: 测试 Web 应用程序工厂类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Shared.Options;

namespace API.Test;

/// <summary>
/// 测试 Web 应用程序工厂类
/// <para>用于创建测试用的 WebApplicationFactory 实例，配置测试环境和数据库</para>
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<API.Program> {
    /// <summary>
    /// 配置测试 Web 主机
    /// <para>设置测试用的配置文件和数据库连接</para>
    /// </summary>
    /// <param name="builder">Web 主机构建器</param>
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder) {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ConfigureAppConfiguration((context, config) => {
            config.AddJsonFile("appsettings.Test.json", optional: true)
                  .AddEnvironmentVariables();
        });

        builder.ConfigureServices(services => {
            services.Configure<DatabaseOption>(options => {
                options.ConnectionString = "Data Source=:memory:";
            });
        });
    }
}
