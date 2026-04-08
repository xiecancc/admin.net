/*
 * 文件名称: DepartmentSeeder.cs
 * 功能描述: 部门数据初始化器，负责初始化系统默认部门结构
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using SqlSugar;
using Domain.Entities;
using Database.Bases;

namespace Database.Seeders;

/// <summary>
/// 部门数据初始化器
/// <para>负责初始化系统默认部门结构</para>
/// </summary>
/// <param name="client">SqlSugar 客户端</param>
public class DepartmentSeeder(ISqlSugarClient client) : Seeder<Department>(client) {

    /// <summary>
    /// 初始化部门数据
    /// </summary>
    /// <returns>异步任务</returns>
    /// <exception cref="Exception">当初始化失败时抛出</exception>
    public override async Task SeedAsync() {
        try {
            if (!await IsTableEmptyAsync()) {
                LogInfo("部门数据已存在，跳过初始化");
                return;
            }

            LogInfo("正在初始化部门数据...");
        var departments = new List<Department>
        {
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Code = "COMPANY",
                Name = "总公司",
                Description = "公司总部，负责整体战略规划和管理",
                Sort = 1
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Code = "HR",
                Name = "人力资源部",
                Description = "负责招聘、培训、绩效考核等人力资源管理工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Sort = 1
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Code = "IT",
                Name = "信息技术部",
                Description = "负责公司的信息技术支持和系统开发工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Sort = 2
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Code = "FINANCE",
                Name = "财务部",
                Description = "负责公司的财务管理、预算编制和会计核算工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Sort = 3
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Code = "MARKETING",
                Name = "市场部",
                Description = "负责公司的市场推广、品牌建设和销售工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Sort = 4
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                Code = "IT_DEV",
                Name = "开发组",
                Description = "负责公司的软件系统开发工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Sort = 1
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                Code = "IT_OPS",
                Name = "运维组",
                Description = "负责公司的系统运维和网络管理工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Sort = 2
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000008"),
                Code = "IT_TEST",
                Name = "测试组",
                Description = "负责公司软件系统的测试工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Sort = 3
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000009"),
                Code = "HR_RECRUIT",
                Name = "招聘组",
                Description = "负责公司的人才招聘工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Sort = 1
            },
            new() {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                Code = "HR_TRAIN",
                Name = "培训组",
                Description = "负责公司的员工培训工作",
                ParentId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Sort = 2
            }
        };

            _ = await _client.Insertable(departments.ToArray()).ExecuteCommandAsync();
            LogInfo("部门数据初始化完成，共 {0} 个部门", departments.Count);
        }
        catch (Exception ex) {
            LogException(ex, "初始化部门数据失败");
            throw;
        }
    }

    /// <summary>
    /// 根据部门代码获取部门
    /// </summary>
    /// <param name="code">部门代码</param>
    /// <returns>部门实体，如果不存在则返回 null</returns>
    public async Task<Department?> GetDepartmentByCodeAsync(string code) {
        return await _client.Queryable<Department>().FirstAsync(d => d.Code == code);
    }
}