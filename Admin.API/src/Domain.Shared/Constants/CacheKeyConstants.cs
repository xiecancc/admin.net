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
        public const string PREFIX = "user";

        /// <summary>
        /// 详情缓存键格式
        /// </summary>
        public const string DETAIL_FORMAT = "user:detail:{0}";

        /// <summary>
        /// 列表缓存键前缀
        /// </summary>
        public const string LIST = "user:list";

        /// <summary>
        /// 分页缓存键格式
        /// </summary>
        public const string PAGED_FORMAT = "user:paged:{0}:{1}:{2}";

        /// <summary>
        /// 权限缓存键格式
        /// </summary>
        public const string PERMISSIONS_FORMAT = "user:permissions:{0}";

        /// <summary>
        /// 状态缓存键格式
        /// </summary>
        public const string STATUS_FORMAT = "user:status:{0}";

        /// <summary>
        /// 信息缓存键格式
        /// </summary>
        public const string INFO_FORMAT = "user:info:{0}";

        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid userId) {
            return string.Format(DETAIL_FORMAT, userId);
        }

        /// <summary>
        /// 获取列表缓存键
        /// </summary>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>列表缓存键</returns>
        public static string ListByKey(string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey) ? LIST : $"{LIST}:{queryCacheKey}";
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
                ? string.Format(PAGED_FORMAT, page, size, string.Empty).TrimEnd(':')
                : string.Format(PAGED_FORMAT, page, size, queryCacheKey);
        }

        /// <summary>
        /// 获取权限缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>权限缓存键</returns>
        public static string Permissions(Guid userId) {
            return string.Format(PERMISSIONS_FORMAT, userId);
        }

        /// <summary>
        /// 获取状态缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>状态缓存键</returns>
        public static string Status(Guid userId) {
            return string.Format(STATUS_FORMAT, userId);
        }

        /// <summary>
        /// 获取信息缓存键
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>信息缓存键</returns>
        public static string Info(Guid userId) {
            return string.Format(INFO_FORMAT, userId);
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
        public const string PREFIX = "role";

        /// <summary>
        /// 详情缓存键格式
        /// </summary>
        public const string DETAIL_FORMAT = "role:detail:{0}";

        /// <summary>
        /// 列表缓存键前缀
        /// </summary>
        public const string LIST = "role:list";

        /// <summary>
        /// 分页缓存键格式
        /// </summary>
        public const string PAGED_FORMAT = "role:paged:{0}:{1}:{2}";

        /// <summary>
        /// 权限缓存键格式
        /// </summary>
        public const string PERMISSIONS_FORMAT = "role:permissions:{0}";

        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid roleId) {
            return string.Format(DETAIL_FORMAT, roleId);
        }

        /// <summary>
        /// 获取列表缓存键
        /// </summary>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>列表缓存键</returns>
        public static string ListByKey(string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey) ? LIST : $"{LIST}:{queryCacheKey}";
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
                ? string.Format(PAGED_FORMAT, page, size, string.Empty).TrimEnd(':')
                : string.Format(PAGED_FORMAT, page, size, queryCacheKey);
        }

        /// <summary>
        /// 获取权限缓存键
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>权限缓存键</returns>
        public static string Permissions(Guid roleId) {
            return string.Format(PERMISSIONS_FORMAT, roleId);
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
        public const string PREFIX = "permission";

        /// <summary>
        /// 详情缓存键格式
        /// </summary>
        public const string DETAIL_FORMAT = "permission:detail:{0}";

        /// <summary>
        /// 列表缓存键前缀
        /// </summary>
        public const string LIST = "permission:list";

        /// <summary>
        /// 分页缓存键格式
        /// </summary>
        public const string PAGED_FORMAT = "permission:paged:{0}:{1}:{2}";

        /// <summary>
        /// 菜单权限缓存键前缀
        /// </summary>
        public const string MENU_PREFIX = "permission:menu";

        /// <summary>
        /// API 权限缓存键前缀
        /// </summary>
        public const string API_PREFIX = "permission:api";

        /// <summary>
        /// 按钮权限缓存键前缀
        /// </summary>
        public const string BUTTON_PREFIX = "permission:button";

        /// <summary>
        /// 获取详情缓存键
        /// </summary>
        /// <param name="permissionId">权限 ID</param>
        /// <returns>详情缓存键</returns>
        public static string Detail(Guid permissionId) {
            return string.Format(DETAIL_FORMAT, permissionId);
        }

        /// <summary>
        /// 获取列表缓存键
        /// </summary>
        /// <param name="queryCacheKey">查询参数缓存键</param>
        /// <returns>列表缓存键</returns>
        public static string ListByKey(string queryCacheKey) {
            return string.IsNullOrEmpty(queryCacheKey) ? LIST : $"{LIST}:{queryCacheKey}";
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
                ? string.Format(PAGED_FORMAT, page, size, string.Empty).TrimEnd(':')
                : string.Format(PAGED_FORMAT, page, size, queryCacheKey);
        }
    }
}
