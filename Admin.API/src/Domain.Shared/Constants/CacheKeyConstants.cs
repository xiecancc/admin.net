/*
 * 文件名称: CacheKeyConstants.cs
 * 功能描述: 缓存键常量类，统一管理所有缓存键
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

namespace Domain.Shared.Constants;

/// <summary>
/// 缓存键常量类
/// <para>统一管理所有缓存键，确保缓存键的一致性和可维护性</para>
/// </summary>
/// <remarks>
/// <para>缓存键格式规范：{模块}:{操作}:{标识}[:{子标识}]</para>
/// <para>示例：</para>
/// <list type="bullet">
///   <item>user:detail:{userId} - 用户详情缓存</item>
///   <item>user:list:{queryCacheKey} - 用户列表缓存</item>
///   <item>user:paged:{page}:{size}:{queryCacheKey} - 用户分页缓存</item>
/// </list>
/// </remarks>
public static class CacheKeyConstants {
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
    public static class Role {
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
        /// 获取详情缓存键
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid roleId) {
            return string.Format(DetailFormat, roleId);
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
        /// <param name="roleId">角色 ID</param>
        /// <returns>权限缓存键</returns>
        public static string Permissions(Guid roleId) {
            return string.Format(PermissionsFormat, roleId);
        }

        /// <summary>
        /// 获取角色相关的所有缓存键模式
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>缓存键模式列表</returns>
        public static string[] AllPatterns(Guid roleId) {
            return [
            Detail(roleId),
            Permissions(roleId)
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
        public static string Paged(int page, int size, string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey)
                ? string.Format(PagedFormat, page, size, string.Empty).TrimEnd(':')
                : string.Format(PagedFormat, page, size, queryCacheKey);
        }
    }
}
