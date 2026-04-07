/*
 * 文件名称：DtoValidatorBase.cs
 * 功能描述：DTO 验证器基类，提供通用的验证规则和扩展方法
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-04
 */

using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// DTO 验证器基类
/// <para>提供通用的验证规则和扩展方法</para>
/// </summary>
/// <typeparam name="T">要验证的 DTO 类型</typeparam>
/// <remarks>
/// <para>职责：提供统一的验证规则和错误消息格式</para>
/// <para>所有 DTO 验证器都应继承此基类</para>
/// </remarks>
public abstract class DtoValidatorBase<T> : AbstractValidator<T> {
    #region 字符串验证规则

    /// <summary>
    /// 验证字符串不为空
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> NotEmpty(
        IRuleBuilder<T, string> ruleBuilder,
        string? message = null) {
        return ruleBuilder.NotEmpty().WithMessage(message ?? "不能为空");
    }

    /// <summary>
    /// 验证字符串最大长度
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> MaxLength(
        IRuleBuilder<T, string> ruleBuilder,
        int maxLength,
        string? message = null) {
        return ruleBuilder.MaximumLength(maxLength)
            .WithMessage(message ?? $"长度不能超过 {maxLength} 个字符");
    }

    /// <summary>
    /// 验证字符串长度范围
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="minLength">最小长度</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Length(
        IRuleBuilder<T, string> ruleBuilder,
        int minLength,
        int maxLength,
        string? message = null) {
        return ruleBuilder.Length(minLength, maxLength)
            .WithMessage(message ?? $"长度必须在 {minLength} 到 {maxLength} 个字符之间");
    }

    #endregion

    #region 格式验证规则

    /// <summary>
    /// 验证邮箱格式
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Email(
        IRuleBuilder<T, string> ruleBuilder,
        string? message = null) {
        return ruleBuilder.EmailAddress()
            .WithMessage(message ?? "邮箱格式无效");
    }

    /// <summary>
    /// 验证手机号格式（中国大陆）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Phone(
        IRuleBuilder<T, string> ruleBuilder,
        string? message = null) {
        return ruleBuilder.Matches(@"^1[3-9]\d{9}$")
            .WithMessage(message ?? "手机号格式无效");
    }

    /// <summary>
    /// 验证 URL 格式
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Url(
        IRuleBuilder<T, string> ruleBuilder,
        string? message = null) {
        return ruleBuilder.Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(message ?? "URL 格式无效");
    }

    #endregion

    #region 数值验证规则

    /// <summary>
    /// 验证整数范围
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, int> Range(
        IRuleBuilder<T, int> ruleBuilder,
        int min,
        int max,
        string? message = null) {
        return ruleBuilder.InclusiveBetween(min, max)
            .WithMessage(message ?? $"必须在 {min} 到 {max} 之间");
    }

    /// <summary>
    /// 验证整数最小值
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="min">最小值</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, int> Min(
        IRuleBuilder<T, int> ruleBuilder,
        int min,
        string? message = null) {
        return ruleBuilder.GreaterThanOrEqualTo(min)
            .WithMessage(message ?? $"不能小于 {min}");
    }

    /// <summary>
    /// 验证整数最大值
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="max">最大值</param>
    /// <param name="message">自定义错误信息</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, int> Max(
        IRuleBuilder<T, int> ruleBuilder,
        int max,
        string? message = null) {
        return ruleBuilder.LessThanOrEqualTo(max)
            .WithMessage(message ?? $"不能大于 {max}");
    }

    #endregion

    #region 组合验证规则

    /// <summary>
    /// 验证名称字段（不为空 + 最大长度 50）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="fieldName">字段名称</param>
    /// <param name="maxLength">最大长度，默认 50</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Name(
        IRuleBuilder<T, string> ruleBuilder,
        string fieldName = "名称",
        int maxLength = 50) {
        return ruleBuilder
            .NotEmpty().WithMessage($"{fieldName}不能为空")
            .MaximumLength(maxLength).WithMessage($"{fieldName}长度不能超过 {maxLength} 个字符");
    }

    /// <summary>
    /// 验证编码字段（不为空 + 最大长度 + 字母数字下划线格式）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="fieldName">字段名称</param>
    /// <param name="maxLength">最大长度，默认 50</param>
    /// <param name="allowColon">是否允许冒号，默认 false</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Code(
        IRuleBuilder<T, string> ruleBuilder,
        string fieldName = "编码",
        int maxLength = 50,
        bool allowColon = false) {
        var pattern = allowColon ? @"^[a-zA-Z0-9_:]+$" : @"^[a-zA-Z0-9_]+$";
        return ruleBuilder
            .NotEmpty().WithMessage($"{fieldName}不能为空")
            .MaximumLength(maxLength).WithMessage($"{fieldName}长度不能超过 {maxLength} 个字符")
            .Matches(pattern).WithMessage($"{fieldName}只能包含字母、数字和下划线{(allowColon ? "和冒号" : "")}");
    }

    /// <summary>
    /// 验证邮箱字段（不为空 + 邮箱格式 + 最大长度 100）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> EmailRequired(
        IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式无效")
            .MaximumLength(100).WithMessage("邮箱长度不能超过 100 个字符");
    }

    /// <summary>
    /// 验证密码字段（不为空 + 长度 8-100 + 复杂度要求）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <param name="requireComplexity">是否要求复杂度，默认 true</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, string> Password(
        IRuleBuilder<T, string> ruleBuilder,
        bool requireComplexity = true) {
        var builder = ruleBuilder
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码长度至少为 8 个字符")
            .MaximumLength(100).WithMessage("密码长度不能超过 100 个字符");

        if (requireComplexity) {
            _ = builder
                .Matches(@"[A-Z]").WithMessage("密码必须包含至少一个大写字母")
                .Matches(@"[a-z]").WithMessage("密码必须包含至少一个小写字母")
                .Matches(@"[0-9]").WithMessage("密码必须包含至少一个数字");
        }

        return builder;
    }

    /// <summary>
    /// 验证排序字段（大于等于 0）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, int> Sort(
        IRuleBuilder<T, int> ruleBuilder) {
        return ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("排序值不能小于 0");
    }

    /// <summary>
    /// 验证权限类型（1-3）
    /// </summary>
    /// <param name="ruleBuilder">规则构建器</param>
    /// <returns>规则构建器</returns>
    protected static IRuleBuilderOptions<T, int> PermissionType(
        IRuleBuilder<T, int> ruleBuilder) {
        return ruleBuilder.InclusiveBetween(1, 3).WithMessage("权限类型必须在 1-3 之间");
    }

    #endregion
}
