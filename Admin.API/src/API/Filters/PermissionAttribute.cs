/*
 * 文件名称: PermissionAttribute.cs
 * 功能描述: 权限校验特性，实现声明式权限校验功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-09
 */

using Infrastructure.Shared.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// 逻辑运算符枚举
/// <para>定义多个权限码之间的逻辑关系</para>
/// </summary>
public enum LogicalOperator {
    /// <summary>
    /// 与运算
    /// <para>需要同时拥有所有权限</para>
    /// </summary>
    And,

    /// <summary>
    /// 或运算
    /// <para>只需拥有任意一个权限</para>
    /// </summary>
    Or
}

/// <summary>
/// 权限校验特性
/// <para>实现声明式权限校验功能，支持单个或多个权限码的 AND/OR 逻辑校验</para>
/// </summary>
/// <remarks>
/// <para>使用示例：</para>
/// <code>
/// // 单个权限
/// [Permission("user:view")]
/// 
/// // 多个权限（OR 逻辑，默认）
/// [Permission("user:view", "user:edit")]
/// 
/// // 多个权限（AND 逻辑）
/// [Permission("user:view", "user:edit", LogicalOperator = LogicalOperator.And)]
/// 
/// // 自定义失败消息
/// [Permission("admin:access", ErrorMessage = "需要管理员权限")]
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter {
    /// <summary>
    /// 权限码数组
    /// </summary>
    public string[] PermissionCodes { get; }

    /// <summary>
    /// 逻辑运算符
    /// <para>默认为 Or，即只需拥有任意一个权限</para>
    /// </summary>
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.Or;

    /// <summary>
    /// 权限校验失败的自定义消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 初始化权限校验特性
    /// </summary>
    /// <param name="permissionCodes">权限码数组</param>
    /// <exception cref="ArgumentException">权限码数组为空或包含空值</exception>
    public PermissionAttribute(params string[] permissionCodes) {
        if (permissionCodes == null || permissionCodes.Length == 0) {
            throw new ArgumentException("权限码不能为空", nameof(permissionCodes));
        }

        if (permissionCodes.Any(string.IsNullOrWhiteSpace)) {
            throw new ArgumentException("权限码不能包含空值", nameof(permissionCodes));
        }

        PermissionCodes = permissionCodes;
    }

    /// <summary>
    /// 异步权限校验
    /// </summary>
    /// <param name="context">授权过滤器上下文</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {
        var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<PermissionAttribute>)) as ILogger<PermissionAttribute>;

        if (context.HttpContext.RequestServices.GetService(typeof(IUserContextProvider)) is not IUserContextProvider userContext)
        {
            logger?.LogError("IUserContextProvider 服务未注册");
            context.Result = new StatusCodeResult(500);
            return;
        }

        if (!userContext.IsAuthenticated) {
            logger?.LogWarning("用户未登录，拒绝访问: {Path}", context.HttpContext.Request.Path);
            context.Result = new ObjectResult(new {
                Success = false,
                Message = "用户未登录"
            }) {
                StatusCode = 401
            };
            return;
        }

        bool hasPermission = await CheckPermissionsAsync(userContext, context.HttpContext.RequestAborted);

        if (!hasPermission) {
            string message = ErrorMessage ?? BuildDefaultErrorMessage();
            logger?.LogWarning(
                "权限校验失败: UserId={UserId}, Permissions=[{Permissions}], Operator={Operator}, Path={Path}",
                userContext.UserId,
                string.Join(", ", PermissionCodes),
                LogicalOperator,
                context.HttpContext.Request.Path
            );

            context.Result = new ObjectResult(new {
                Success = false,
                Message = message
            }) {
                StatusCode = 403
            };
        }
    }

    /// <summary>
    /// 异步检查权限
    /// </summary>
    /// <param name="userContext">用户上下文提供者</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有权限</returns>
    private async Task<bool> CheckPermissionsAsync(
        IUserContextProvider userContext,
        CancellationToken cancellationToken) {
        if (PermissionCodes.Length == 1) {
            return await userContext.HasPermissionAsync(PermissionCodes[0], cancellationToken);
        }

        return LogicalOperator == LogicalOperator.Or
            ? await userContext.HasAnyPermissionAsync(PermissionCodes, cancellationToken)
            : await userContext.HasAllPermissionsAsync(PermissionCodes, cancellationToken);
    }

    /// <summary>
    /// 构建默认错误消息
    /// </summary>
    /// <returns>错误消息</returns>
    private string BuildDefaultErrorMessage() {
        string permissionList = string.Join("、", PermissionCodes);
        return LogicalOperator == LogicalOperator.And
            ? $"需要同时拥有以下权限: {permissionList}"
            : $"需要拥有以下任意权限: {permissionList}";
    }
}
