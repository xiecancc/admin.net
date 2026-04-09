/*
 * 文件名称: PermissionAuthorizationHandler.cs
 * 功能描述: 权限校验授权处理器，实现基于策略的权限校验功能
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-09
 */

using Infrastructure.Shared.Contexts;
using Microsoft.AspNetCore.Authorization;

namespace API.Filters;

/// <summary>
/// 权限需求类
/// <para>实现 IAuthorizationRequirement 接口，用于定义权限校验的需求</para>
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement {
    /// <summary>
    /// 权限码列表
    /// </summary>
    public string[] PermissionCodes { get; }

    /// <summary>
    /// 逻辑运算符
    /// <para>默认为 Or，即只需拥有任意一个权限</para>
    /// </summary>
    public LogicalOperator LogicalOperator { get; }

    /// <summary>
    /// 初始化权限需求
    /// </summary>
    /// <param name="permissionCodes">权限码数组</param>
    /// <param name="logicalOperator">逻辑运算符，默认为 Or</param>
    /// <exception cref="ArgumentException">权限码数组为空或包含空值</exception>
    public PermissionRequirement(string[] permissionCodes, LogicalOperator logicalOperator = LogicalOperator.Or) {
        if (permissionCodes == null || permissionCodes.Length == 0) {
            throw new ArgumentException("权限码不能为空", nameof(permissionCodes));
        }

        if (permissionCodes.Any(string.IsNullOrWhiteSpace)) {
            throw new ArgumentException("权限码不能包含空值", nameof(permissionCodes));
        }

        PermissionCodes = permissionCodes;
        LogicalOperator = logicalOperator;
    }
}

/// <summary>
/// 权限校验授权处理器
/// <para>继承自 AuthorizationHandler&lt;PermissionRequirement&gt;，处理权限校验逻辑</para>
/// </summary>
/// <remarks>
/// <para>使用示例：</para>
/// <code>
/// // 在 Program.cs 中注册策略
/// builder.Services.AddAuthorization(options => {
///     options.AddPolicy("UserView", policy => 
///         policy.Requirements.Add(new PermissionRequirement(new[] { "user:view" })));
///     
///     options.AddPolicy("UserManage", policy => 
///         policy.Requirements.Add(new PermissionRequirement(
///             new[] { "user:view", "user:edit" }, 
///             LogicalOperator.And)));
/// });
/// 
/// // 在控制器中使用
/// [Authorize(Policy = "UserView")]
/// public IActionResult GetUser() { ... }
/// </code>
/// </remarks>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement> {
    private readonly IUserContextProvider _userContext;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 初始化权限校验授权处理器
    /// </summary>
    /// <param name="userContext">用户上下文提供者</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="httpContextAccessor">HTTP 上下文访问器</param>
    public PermissionAuthorizationHandler(
        IUserContextProvider userContext,
        ILogger<PermissionAuthorizationHandler> logger,
        IHttpContextAccessor httpContextAccessor) {
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// 处理权限需求
    /// </summary>
    /// <param name="context">授权处理器上下文</param>
    /// <param name="requirement">权限需求</param>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement) {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null) {
            _logger.LogWarning("无法获取 HttpContext，权限校验失败");
            return;
        }

        if (!_userContext.IsAuthenticated) {
            _logger.LogWarning(
                "用户未登录，权限校验失败: Permissions=[{Permissions}], Path={Path}",
                string.Join(", ", requirement.PermissionCodes),
                httpContext.Request.Path
            );
            return;
        }

        try {
            var hasPermission = await CheckPermissionsAsync(
                requirement.PermissionCodes,
                requirement.LogicalOperator,
                httpContext.RequestAborted
            );

            if (hasPermission) {
                _logger.LogDebug(
                    "权限校验成功: UserId={UserId}, Permissions=[{Permissions}], Operator={Operator}",
                    _userContext.UserId,
                    string.Join(", ", requirement.PermissionCodes),
                    requirement.LogicalOperator
                );
                context.Succeed(requirement);
            } else {
                _logger.LogWarning(
                    "权限校验失败: UserId={UserId}, Permissions=[{Permissions}], Operator={Operator}, Path={Path}",
                    _userContext.UserId,
                    string.Join(", ", requirement.PermissionCodes),
                    requirement.LogicalOperator,
                    httpContext.Request.Path
                );
            }
#pragma warning disable CA1031 // 权限验证需要捕获所有异常以确保不影响主业务流程
        } catch (Exception ex) {
            _logger.LogError(
                ex,
                "权限校验异常: UserId={UserId}, Permissions=[{Permissions}], Operator={Operator}",
                _userContext.UserId,
                string.Join(", ", requirement.PermissionCodes),
                requirement.LogicalOperator
            );
        }
#pragma warning restore CA1031
    }

    /// <summary>
    /// 异步检查权限
    /// </summary>
    /// <param name="permissionCodes">权限码数组</param>
    /// <param name="logicalOperator">逻辑运算符</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否拥有权限</returns>
    private async Task<bool> CheckPermissionsAsync(
        string[] permissionCodes,
        LogicalOperator logicalOperator,
        CancellationToken cancellationToken) {
        if (permissionCodes.Length == 1) {
            return await _userContext.HasPermissionAsync(permissionCodes[0], cancellationToken);
        }

        return logicalOperator == LogicalOperator.Or
            ? await _userContext.HasAnyPermissionAsync(permissionCodes, cancellationToken)
            : await _userContext.HasAllPermissionsAsync(permissionCodes, cancellationToken);
    }
}
