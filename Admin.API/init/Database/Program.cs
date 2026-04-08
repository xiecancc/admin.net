/*
 * 文件名称: Program.cs
 * 功能描述: 数据库初始化程序入口，负责构建配置、创建数据库上下文并执行初始化
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using Infrastructure.Sugars;
using Infrastructure.Shared.Options;
using Microsoft.Extensions.Configuration;

namespace Database;

/// <summary>
/// 数据库初始化程序
/// </summary>
internal class Program {
    /// <summary>
    /// 程序入口
    /// </summary>
    /// <param name="args">命令行参数</param>
    static async Task Main(string[] args) {

        try {
            var configuration = BuildConfiguration();

            Console.WriteLine("[INFO] 开始初始化数据库...\n");

            var option = configuration.GetSection("Database").Get<DatabaseOption>();

            if (option == null || string.IsNullOrEmpty(option.ConnectionString)) {
                Console.WriteLine("[ERROR] 数据库连接字符串未配置，请检查 appsettings.json 文件");
                return;
            }

            var dbContext = new SugarDb(option);

            var initializer = new Initializer(dbContext.GetClient());
            await initializer.InitializeAsync();
        }
        catch (Exception ex) {
            LogException(ex, "数据库初始化失败");
        }
    }

    /// <summary>
    /// 构建配置
    /// </summary>
    private static IConfigurationRoot BuildConfiguration() {
        // 使用相对路径加载配置文件
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables();

        var config = configBuilder.Build();
        var connectionString = config.GetSection("Database:ConnectionString").Value;
        Console.WriteLine($"[INFO] 读取到的连接字符串：{connectionString}");
        
        return config;
    }

    /// <summary>
    /// 写入异常日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="message">日志消息</param>
    /// <param name="args">格式化参数</param>
    private static void LogException(Exception ex, string message, params object[] args) {
        var formattedMessage = string.Format(message, args);
        Console.WriteLine($"[ERROR] {formattedMessage}");
        Console.WriteLine($"[ERROR] 异常类型：{ex.GetType().Name}");
        Console.WriteLine($"[ERROR] 详细信息：{ex.Message}");
        Console.WriteLine($"[ERROR] 堆栈跟踪：{ex.StackTrace}");
        if (ex.InnerException != null) {
            Console.WriteLine($"[ERROR] 内部异常：{ex.InnerException.Message}");
        }
    }
}
