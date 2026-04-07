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
        var basePath = Directory.GetCurrentDirectory();
        var configBuilder = new ConfigurationBuilder().SetBasePath(basePath);

        _ = configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        _ = configBuilder.AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
            optional: true,
            reloadOnChange: false
        );

        var apiConfigPath = FindApiConfigPath(basePath);
        if (apiConfigPath != null) {
            Console.WriteLine($"[INFO] 使用配置文件：{apiConfigPath}\n");
            _ = configBuilder.AddJsonFile(apiConfigPath, optional: true, reloadOnChange: false);
        }

        _ = configBuilder.AddEnvironmentVariables();

        return configBuilder.Build();
    }

    /// <summary>
    /// 查找 API 项目配置文件路径
    /// </summary>
    private static string? FindApiConfigPath(string basePath) {
        var possiblePaths = new[] {
            Path.Combine(basePath, "..", "..", "..", "src", "API", "appsettings.json"),
            Path.Combine(basePath, "..", "src", "API", "appsettings.json"),
            Path.Combine(basePath, "src", "API", "appsettings.json")
        };

        foreach (var path in possiblePaths) {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath)) {
                return fullPath;
            }
        }

        return null;
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
