/*
 * 文件名称: ResponseFilter.cs
 * 功能描述: 响应过滤器，统一包装 API 响应和异常处理
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Authentication;

namespace API.Filters;

/// <summary>
/// 响应过滤器
/// <para>统一包装 API 响应和异常处理</para>
/// </summary>
/// <param name="_logger">日志记录器</param>
public class ResponseFilter(ILogger<ResponseFilter> _logger) : IAsyncExceptionFilter, IAsyncResultFilter
{

    /// <summary>
    /// 处理异常
    /// <para>将异常转换为统一的操作结果格式</para>
    /// </summary>
    /// <param name="context">异常上下文</param>
    public Task OnExceptionAsync(ExceptionContext context) {
        var (statusCode, message) = context.Exception switch {
            ValidationException ex => (400, string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))),
            ArgumentNullException ex => (400, ex.Message),
            ArgumentException ex => (400, ex.Message),
            InvalidOperationException ex => (400, ex.Message),
            AuthenticationException ex => (401, ex.Message),
            UnauthorizedAccessException ex => (403, ex.Message),
            KeyNotFoundException ex => (404, ex.Message),
            _ => (500, "服务器内部错误，请稍后重试")
        };

        if (statusCode >= 500) {
            _logger.LogError(context.Exception, "请求处理异常: {Path}", context.HttpContext.Request.Path);
        }
        else {
            _logger.LogWarning("请求处理失败: {Path}, Status: {StatusCode}, Message: {Message}", context.HttpContext.Request.Path, statusCode, message);
        }

        context.Result = CreateFailedResult(message, statusCode);
        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 包装成功结果
    /// <para>将返回值自动包装为统一格式</para>
    /// </summary>
    /// <param name="context">结果执行上下文</param>
    /// <param name="next">下一个委托</param>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
        switch (context.Result) {
            // ========== 4xx 错误响应 ==========
            case NotFoundObjectResult notFoundResult:
                context.Result = CreateFailedResult(notFoundResult.Value?.ToString() ?? "资源不存在", 404);
                break;

            case NotFoundResult:
                context.Result = CreateFailedResult("资源不存在", 404);
                break;

            case BadRequestObjectResult badRequestResult:
                var badRequestMessage = badRequestResult.Value switch {
                    string s => s,
                    FluentValidation.Results.ValidationResult vr => string.Join("; ", vr.Errors.Select(e => e.ErrorMessage)),
                    _ => badRequestResult.Value?.ToString() ?? "请求参数错误"
                };
                context.Result = CreateFailedResult(badRequestMessage, 400);
                break;

            case BadRequestResult:
                context.Result = CreateFailedResult("请求参数错误", 400);
                break;

            case UnauthorizedObjectResult unauthorizedResult:
                context.Result = CreateFailedResult(unauthorizedResult.Value?.ToString() ?? "未授权访问", 401);
                break;

            case UnauthorizedResult:
                context.Result = CreateFailedResult("未授权访问", 401);
                break;

            case ForbidResult:
                context.Result = CreateFailedResult("权限不足", 403);
                break;

            case ConflictObjectResult conflictResult:
                context.Result = CreateFailedResult(conflictResult.Value?.ToString() ?? "资源冲突", 409);
                break;

            case ConflictResult:
                context.Result = CreateFailedResult("资源冲突", 409);
                break;

            case UnprocessableEntityObjectResult unprocessableResult:
                context.Result = CreateFailedResult(unprocessableResult.Value?.ToString() ?? "无法处理的实体", 422);
                break;

            case UnprocessableEntityResult:
                context.Result = CreateFailedResult("无法处理的实体", 422);
                break;

            case StatusCodeResult { StatusCode: >= 400 } statusCodeResult:
                context.Result = CreateFailedResult($"请求失败，状态码: {statusCodeResult.StatusCode}", statusCodeResult.StatusCode);
                break;

            // ========== 2xx 成功响应 ==========
            case OkObjectResult okResult:
                context.Result = CreateSuccessResult(okResult.Value, 200);
                break;

            case OkResult:
                context.Result = CreateSuccessResult(null, 200);
                break;

            case CreatedResult createdResult:
                context.Result = CreateSuccessResult(createdResult.Value, 201);
                break;

            case CreatedAtActionResult createdAtActionResult:
                context.Result = CreateSuccessResult(createdAtActionResult.Value, 201);
                break;

            case CreatedAtRouteResult createdAtRouteResult:
                context.Result = CreateSuccessResult(createdAtRouteResult.Value, 201);
                break;

            case AcceptedResult acceptedResult:
                context.Result = CreateSuccessResult(acceptedResult.Value, 202);
                break;

            case AcceptedAtActionResult acceptedAtActionResult:
                context.Result = CreateSuccessResult(acceptedAtActionResult.Value, 202);
                break;

            case AcceptedAtRouteResult acceptedAtRouteResult:
                context.Result = CreateSuccessResult(acceptedAtRouteResult.Value, 202);
                break;

            case NoContentResult:
                context.Result = CreateSuccessResult(null, 204);
                break;

            case JsonResult jsonResult:
                context.Result = CreateSuccessResult(jsonResult.Value, jsonResult.StatusCode ?? 200);
                break;

            case ObjectResult objectResult:
                var statusCode = objectResult.StatusCode ?? 200;
                context.Result = statusCode >= 400
                    ? CreateFailedResult(objectResult.Value?.ToString() ?? "请求失败", statusCode)
                    : CreateSuccessResult(objectResult.Value, statusCode);
                break;

            case EmptyResult:
                context.Result = CreateSuccessResult(null, 200);
                break;
        }

        await next();
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    private static ObjectResult CreateSuccessResult(object? data, int statusCode) {
        return new ObjectResult(new {
            Success = true,
            Data = data
        }) {
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    private static ObjectResult CreateFailedResult(string message, int statusCode) {
        return new ObjectResult(new {
            Success = false,
            Message = message
        }) {
            StatusCode = statusCode
        };
    }
}
