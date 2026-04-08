/*
 * 文件名称: DepartmentPermissionAttribute.cs
 * 功能描述: 部门权限验证属性，用于验证用户对部门的操作权限
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Filters;

/// <summary>
/// 部门权限验证属性
/// <para>用于验证用户对部门的操作权限</para>
/// </summary>
/// <remarks>
/// <para>使用示例：</para>
/// <code>
/// // 验证用户对指定部门的管理权限
/// [HttpPut("{id:guid}")]
/// [DepartmentPermission]
/// public async Task<ActionResult<bool>> UpdateAsync(Guid id, [FromBody] DepartmentUpdateDto dto, CancellationToken cancellationToken = default) {
///     // 方法实现
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class DepartmentPermissionAttribute : ActionFilterAttribute {
    /// <summary>
    /// 部门ID参数名称
    /// <para>默认为 "id"，对应路由中的 {id:guid} 参数</para>
    /// </summary>
    public string DepartmentIdParamName { get; set; } = "id";

    /// <inheritdoc/>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        var departmentPermissionService = context.HttpContext.RequestServices.GetRequiredService<IDepartmentPermissionService>();
        var userId = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid)) {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!context.ActionArguments.TryGetValue(DepartmentIdParamName, out var departmentIdValue)) {
            context.Result = new BadRequestObjectResult("部门ID参数不存在");
            return;
        }

        if (departmentIdValue is not Guid departmentId) {
            context.Result = new BadRequestObjectResult("部门ID参数类型错误");
            return;
        }

        try {
            var hasPermission = await departmentPermissionService.HasDepartmentPermissionAsync(userIdGuid, departmentId, context.HttpContext.RequestAborted);
            if (!hasPermission) {
                context.Result = new ForbidResult();
                return;
            }
        } catch (Exception ex) {
            context.Result = new StatusCodeResult(500);
            return;
        }

        await next();
    }
}