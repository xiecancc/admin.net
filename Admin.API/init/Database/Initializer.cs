/*
 * 文件名称: Initializer.cs
 * 功能描述: 数据库初始化协调器，负责协调数据库创建、表创建和种子数据初始化
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-03-30
 */

using SqlSugar;
using Infrastructure.Sugars;
using Domain.Shared.Constants;
using Database.Seeders;
using Database.Creators;

namespace Database;

/// <summary>
/// 数据库初始化协调器
/// <para>负责协调数据库创建、表创建和种子数据初始化</para>
/// </summary>
/// <remarks>
/// <para>主要功能：</para>
/// <list type="bullet">
///   <item>创建数据库</item>
///   <item>创建数据表</item>
///   <item>初始化角色数据</item>
///   <item>初始化权限数据</item>
///   <item>初始化管理员用户</item>
///   <item>初始化用户角色关系</item>
///   <item>初始化角色权限关系</item>
/// </list>
/// </remarks>
public class Initializer {
    /// <summary>
    /// 数据库创建器
    /// </summary>
    private readonly DatabaseCreator _databaseCreator;

    /// <summary>
    /// 数据表创建器
    /// </summary>
    private readonly TableCreator _tableCreator;

    /// <summary>
    /// 角色数据初始化器
    /// </summary>
    private readonly RoleSeeder _roleSeeder;

    /// <summary>
    /// 菜单权限数据初始化器
    /// </summary>
    private readonly MenuPermissionSeeder _menuPermissionSeeder;

    /// <summary>
    /// API 权限数据初始化器
    /// </summary>
    private readonly ApiPermissionSeeder _apiPermissionSeeder;

    /// <summary>
    /// 按钮权限数据初始化器
    /// </summary>
    private readonly ButtonPermissionSeeder _buttonPermissionSeeder;

    /// <summary>
    /// 管理员用户数据初始化器
    /// </summary>
    private readonly AdminUserSeeder _adminUserSeeder;

    /// <summary>
    /// 用户角色关系数据初始化器
    /// </summary>
    private readonly UserRoleSeeder _userRoleSeeder;

    /// <summary>
    /// 角色权限关系数据初始化器
    /// </summary>
    private readonly RolePermissionSeeder _rolePermissionSeeder;



    /// <summary>
    /// 构造函数（用于 ASP.NET Core）
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public Initializer(SugarDb dbContext) : this(dbContext.GetClient()) {
    }

    /// <summary>
    /// 构造函数（用于控制台程序）
    /// </summary>
    /// <param name="client">SqlSugar 客户端，不能为空</param>
    /// <exception cref="ArgumentNullException">当 client 为 null 时抛出</exception>
    /// <remarks>
    /// <para>使用 ArgumentNullException.ThrowIfNull 进行参数校验</para>
    /// </remarks>
    public Initializer(ISqlSugarClient client) {
        ArgumentNullException.ThrowIfNull(client);

        _databaseCreator = new DatabaseCreator(client);
        _tableCreator = new TableCreator(client);
        _roleSeeder = new RoleSeeder(client);
        _menuPermissionSeeder = new MenuPermissionSeeder(client);
        _apiPermissionSeeder = new ApiPermissionSeeder(client);
        _buttonPermissionSeeder = new ButtonPermissionSeeder(client);
        _adminUserSeeder = new AdminUserSeeder(client);
        _userRoleSeeder = new UserRoleSeeder(client);
        _rolePermissionSeeder = new RolePermissionSeeder(client);
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public async Task InitializeAsync() {
        try {
            _databaseCreator.Create();
            _tableCreator.Create();
            await _roleSeeder.SeedAsync();
            await _menuPermissionSeeder.SeedAsync();
            await _apiPermissionSeeder.SeedAsync();
            await _buttonPermissionSeeder.SeedAsync();
            await _adminUserSeeder.SeedAsync();
            var adminUserId = _adminUserSeeder.GetAdminUserId();

            if (adminUserId.HasValue) {
                await _userRoleSeeder.SeedAsync(adminUserId.Value, RoleConstants.Administrator.Code);

                var allPermissions = new List<Guid>();
                allPermissions.AddRange(await _menuPermissionSeeder.GetAllPermissionIdsAsync());
                allPermissions.AddRange(await _apiPermissionSeeder.GetAllPermissionIdsAsync());
                allPermissions.AddRange(await _buttonPermissionSeeder.GetAllPermissionIdsAsync());

                var adminRole = await _roleSeeder.GetRoleByCodeAsync(RoleConstants.Administrator.Code);
                if (adminRole != null) {
                    await _rolePermissionSeeder.SeedAsync(RoleConstants.Administrator.Id, allPermissions);
                }
            }
        }
        catch (Exception ex) {
            LogException(ex, "数据库初始化失败");
            throw;
        }
    }

    /// <summary>
    /// 写入异常日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="message">日志消息</param>
    private static void LogException(Exception ex, string message) {
        Console.WriteLine($"[ERROR] {message}");
        Console.WriteLine($"[ERROR] 异常类型：{ex.GetType().Name}");
        Console.WriteLine($"[ERROR] 详细信息：{ex.Message}");
        Console.WriteLine($"[ERROR] 堆栈跟踪：{ex.StackTrace}");
        if (ex.InnerException != null) {
            Console.WriteLine($"[ERROR] 内部异常：{ex.InnerException.Message}");
        }
    }
}
