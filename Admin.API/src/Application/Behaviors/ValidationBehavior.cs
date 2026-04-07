/*
 * 文件名称: ValidationBehavior.cs
 * 功能描述: 验证管道行为，在请求处理前自动执行所有注册的验证器进行数据验证
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using FluentValidation;
using MediatR;

namespace Application.Behaviors;

/// <summary>
/// 验证管道行为
/// <para>在请求处理前自动执行所有注册的验证器进行数据验证</para>
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
/// <remarks>
/// <para>验证流程：</para>
/// <list type="number">
///   <item>并行执行所有注册的验证器</item>
///   <item>收集所有验证错误</item>
///   <item>如果存在错误则抛出 ValidationException</item>
///   <item>否则继续执行下一个处理器</item>
/// </list>
/// </remarks>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="request">请求对象</param>
    /// <param name="next">下一个处理器</param>
    /// <param name="cancellation">取消令牌</param>
    /// <returns>响应对象</returns>
    /// <exception cref="ValidationException">验证失败时抛出</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellation) {
        if (!validators.Any()) {
            return await next();
        }

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(request, cancellation)));

        var validationFailures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        return validationFailures.Count != 0
            ? throw new ValidationException(validationFailures)
            : await next();
    }
}
