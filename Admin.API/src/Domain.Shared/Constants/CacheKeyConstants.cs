/*
 * 文件名称: CacheKeyConstants.cs
 * 功能描述: 缓存键常量类，统一管理所有缓存键
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

namespace Domain.Shared.Constants;

/// <summary>
/// 缓存键常量类
/// <para>统一管理所有缓存键，确保缓存键的一致性和可维护性</para>
/// </summary>
/// <remarks>
/// <para>缓存键格式规范：{模块}:{操作}:{标识}[:{子标识}]</para>
/// <para>命名约定：</para>
/// <list type="bullet">
///   <item>使用小写字母和冒号分隔</item>
///   <item>标识符使用具体 ID 或查询键</item>
///   <item>保持语义清晰，便于理解和维护</item>
/// </list>
/// <para>示例：</para>
/// <list type="bullet">
///   <item>user:detail:{userId} - 用户详情缓存</item>
///   <item>user:permissions:{userId} - 用户权限缓存</item>
///   <item>role:inherited_permissions:{roleId} - 角色继承权限缓存</item>
///   <item>user:list:{queryCacheKey} - 用户列表缓存</item>
///   <item>user:paged:{page}:{size}:{queryCacheKey} - 用户分页缓存</item>
/// </list>
/// </remarks>
public static class CacheKeyConstants
{
    /// <summary>
    /// 用户相关缓存键
    /// </summary>
    public static class User {
        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public const string Prefix = "user";

        /// <summary>
        /// 详情缓存键格式
        /// </summary>
        public const string DetailFormat = "user:detail:{0}";

        /// <summary>
        /// 列表缓存键前缀
        /// </summary>
        public const string List = "user:list";

        /// <summary>
        /// 分页缓存键格式
        /// </summary>
        public const string PagedFormat = "user:paged:{0}:{1}:{2}";

        /// <summary>
        /// 权限缓存键格式
        /// </summary>
        public const string PermissionsFormat = "user:permissions:{0}";

        /// <summary>
        /// 状态缓存键格式
        /// </summary>
        public const string StatusFormat = "user:status:{0}";

        /// <summary>
        /// 信息缓存键格式
        /// </summary>
        public const string InfoFormat = "user:info:{0}";

        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid userId) {
            return string.Format(DetailFormat, userId);
        }

        /// <summary>
        /// 获取列表缓存键
        /// </summary>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>列表缓存键</returns>
        public static string ListByKey(string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey) ? List : $"{List}:{queryCacheKey}";
        }

        /// <summary>
        /// 获取分页缓存键
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>分页缓存键</returns>
        public static string Paged(int page, int size, string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey)
                ? string.Format(PagedFormat, page, size, string.Empty).TrimEnd(':')
                : string.Format(PagedFormat, page, size, queryCacheKey);
        }

        /// <summary>
        /// 获取权限缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>权限缓存键</returns>
        public static string Permissions(Guid userId) {
            return string.Format(PermissionsFormat, userId);
        }

        /// <summary>
        /// 获取状态缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>状态缓存键</returns>
        public static string Status(Guid userId) {
            return string.Format(StatusFormat, userId);
        }

        /// <summary>
        /// 获取信息缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>信息缓存键</returns>
        public static string Info(Guid userId) {
            return string.Format(InfoFormat, userId);
        }

        /// <summary>
        /// 获取用户相关的所有缓存键模式
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>缓存键模式列表</returns>
        public static string[] AllPatterns(Guid userId) {
            return [
            Detail(userId),
            Permissions(userId),
            Status(userId),
            Info(userId)
        ];
        }
    }

    /// <summary>
    /// 角色相关缓存键
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public const string Prefix = "role";

        /// <summary>
        /// 详情缓存键格式
        /// </summary>
        public const string DetailFormat = "role:detail:{0}";

        /// <summary>
        /// 列表缓存键前缀
        /// </summary>
        public const string List = "role:list";

        /// <summary>
        /// 分页缓存键格式
        /// </summary>
        public const string PagedFormat = "role:paged:{0}:{1}:{2}";

        /// <summary>
        /// 权限缓存键格式
        /// </summary>
        public const string PermissionsFormat = "role:permissions:{0}";

        /// <summary>
        /// 继承权限缓存键格式
        /// <para>用于缓存角色从父角色继承的权限</para>
        /// </summary>
        public const string InheritedPermissionsFormat = "role:inherited_permissions:{0}";

        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid roleId)
        {
            return string.Format(DetailFormat, roleId);
        }

        /// <summary>
        /// 获取列表缓存键
        /// </summary>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>列表缓存键</returns>
        public static string ListByKey(string queryCacheKey)
        {
            return string.IsNullOrEmpty(queryCacheKey) ? List : $"{List}:{queryCacheKey}";
        }

        /// <summary>
        /// 获取分页缓存键
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>分页缓存键</returns>
        public static string Paged(int page, int size, string queryCacheKey)
        {
            return string.IsNullOrEmpty(queryCacheKey)
                ? string.Format(PagedFormat, page, size, string.Empty).TrimEnd(':')
                : string.Format(PagedFormat, page, size, queryCacheKey);
        }

        /// <summary>
        /// 获取权限缓存键
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>权限缓存键</returns>
        public static string Permissions(Guid roleId)
        {
            return string.Format(PermissionsFormat, roleId);
        }

        /// <summary>
        /// 获取继承权限缓存键
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>继承权限缓存键</returns>
        public static string InheritedPermissions(Guid roleId)
        {
            return string.Format(InheritedPermissionsFormat, roleId);
        }

        /// <summary>
        /// 获取角色相关的所有缓存键模式
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>缓存键模式列表</returns>
        public static string[] AllPatterns(Guid roleId)
        {
            return [
                Detail(roleId),
                Permissions(roleId),
                InheritedPermissions(roleId)
            ];
        }
    }

    /// <summary>
    /// 权限相关缓存键
    /// </summary>
    public static class Permission {
        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public const string Prefix = "permission";

        /// <summary>
        /// 详情缓存键格式
        /// </summary>
        public const string DetailFormat = "permission:detail:{0}";

        /// <summary>
        /// 列表缓存键前缀
        /// </summary>
        public const string List = "permission:list";

        /// <summary>
        /// 分页缓存键格式
        /// </summary>
        public const string PagedFormat = "permission:paged:{0}:{1}:{2}";

        /// <summary>
        /// 菜单权限缓存键前缀
        /// </summary>
        public const string MenuPrefix = "permission:menu";

        /// <summary>
        /// API 权限缓存键前缀
        /// </summary>
        public const string ApiPrefix = "permission:api";

        /// <summary>
        /// 按钮权限缓存键前缀
        /// </summary>
        public const string ButtonPrefix = "permission:button";

        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="permissionId">权限 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid permissionId) {
            return string.Format(DetailFormat, permissionId);
        }

        /// <summary>
        /// 获取列表缓存键
        /// </summary>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>列表缓存键</returns>
        public static string ListByKey(string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey) ? List : $"{List}:{queryCacheKey}";
        }

        /// <summary>
        /// 获取分页缓存键
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页大小</param>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>分页缓存键</returns>
        public static string Paged(int page, int size, string queryCacheKey)
        {
            return string.IsNullOrEmpty(queryCacheKey)
                ? string.Format(PagedFormat, page, size, string.Empty).TrimEnd(':')
                : string.Format(PagedFormat, page, size, queryCacheKey);
        }
    }

    /// <summary>
    /// 缓存过期时间常量
    /// <para>定义各类缓存的默认过期时间</para>
    /// </summary>
    /// <remarks>
    /// <para>过期时间设置原则：</para>
    /// <list type="bullet">
    ///   <item>权限相关缓存：较长时间（30分钟），减少权限验证开销</item>
    ///   <item>状态相关缓存：较短时间（5分钟），确保状态实时性</item>
    ///   <item>详情缓存：中等时间（10分钟），平衡性能与数据一致性</item>
    ///   <item>列表/分页缓存：较短时间（5分钟），确保数据新鲜度</item>
    /// </list>
    /// </remarks>
    public static class Expiration
    {
        /// <summary>
        /// 用户权限缓存过期时间（30分钟）
        /// </summary>
        public static readonly TimeSpan UserPermissions = TimeSpan.FromMinutes(30);

        /// <summary>
        /// 用户状态缓存过期时间（5分钟）
        /// </summary>
        public static readonly TimeSpan UserStatus = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 用户详情缓存过期时间（10分钟）
        /// </summary>
        public static readonly TimeSpan UserDetail = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 角色权限缓存过期时间（30分钟）
        /// </summary>
        public static readonly TimeSpan RolePermissions = TimeSpan.FromMinutes(30);

        /// <summary>
        /// 角色继承权限缓存过期时间（30分钟）
        /// </summary>
        public static readonly TimeSpan RoleInheritedPermissions = TimeSpan.FromMinutes(30);

        /// <summary>
        /// 角色详情缓存过期时间（10分钟）
        /// </summary>
        public static readonly TimeSpan RoleDetail = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 列表缓存过期时间（5分钟）
        /// </summary>
        public static readonly TimeSpan List = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 分页缓存过期时间（5分钟）
        /// </summary>
        public static readonly TimeSpan Paged = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 详情缓存过期时间（10分钟）
        /// </summary>
        public static readonly TimeSpan Detail = TimeSpan.FromMinutes(10);
    }

    /// <summary>
    /// 缓存键工具方法
    /// <para>提供缓存键的批量生成、模式匹配等工具方法</para>
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 批量生成用户权限缓存键
        /// </summary>
        /// <param name="userIds">用户 ID 集合</param>
        /// <returns>缓存键数组</returns>
        public static string[] BatchUserPermissions(IEnumerable<Guid> userIds)
        {
            ArgumentNullException.ThrowIfNull(userIds);
            return userIds.Select(User.Permissions).ToArray();
        }

        /// <summary>
        /// 批量生成角色权限缓存键
        /// </summary>
        /// <param name="roleIds">角色 ID 集合</param>
        /// <returns>缓存键数组</returns>
        public static string[] BatchRolePermissions(IEnumerable<Guid> roleIds)
        {
            ArgumentNullException.ThrowIfNull(roleIds);
            return roleIds.Select(Role.Permissions).ToArray();
        }

        /// <summary>
        /// 批量生成角色继承权限缓存键
        /// </summary>
        /// <param name="roleIds">角色 ID 集合</param>
        /// <returns>缓存键数组</returns>
        public static string[] BatchRoleInheritedPermissions(IEnumerable<Guid> roleIds)
        {
            ArgumentNullException.ThrowIfNull(roleIds);
            return roleIds.Select(Role.InheritedPermissions).ToArray();
        }

        /// <summary>
        /// 生成模块通配符模式
        /// <para>用于批量删除某个模块的所有缓存</para>
        /// </summary>
        /// <param name="module">模块名称（如 user、role）</param>
        /// <returns>通配符模式</returns>
        public static string ModulePattern(string module)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(module);
            return $"{module}:*";
        }

        /// <summary>
        /// 生成用户模块通配符模式
        /// </summary>
        /// <returns>用户模块通配符模式</returns>
        public static string UserModulePattern() => ModulePattern(User.Prefix);

        /// <summary>
        /// 生成角色模块通配符模式
        /// </summary>
        /// <returns>角色模块通配符模式</returns>
        public static string RoleModulePattern() => ModulePattern(Role.Prefix);

        /// <summary>
        /// 生成权限模块通配符模式
        /// </summary>
        /// <returns>权限模块通配符模式</returns>
        public static string PermissionModulePattern() => ModulePattern(Permission.Prefix);

        /// <summary>
        /// 合并多个缓存键数组
        /// </summary>
        /// <param name="keysArrays">缓存键数组集合</param>
        /// <returns>合并后的缓存键数组</returns>
        public static string[] MergeKeys(params string[][] keysArrays)
        {
            ArgumentNullException.ThrowIfNull(keysArrays);
            return keysArrays.SelectMany(keys => keys).ToArray();
        }

        /// <summary>
        /// 验证缓存键格式是否有效
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns>是否有效</returns>
        public static bool IsValidFormat(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return false;
            }

            var parts = cacheKey.Split(':');
            return parts.Length >= 2;
        }
    }
}
