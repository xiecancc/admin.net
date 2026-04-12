/*
 * 文件名称：ApiPermissionDtosValidator.cs
 * 功能描述：API 权限相关 DTO 验证器，包含 API 权限创建、更新、查询参数等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-11
 */

using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// API 权限创建 DTO 验证器
/// </summary>
public class ApiPermissionCreateDtoValidator : PermissionCreateDtoValidatorBase<ApiPermissionCreateDto> {
    /// <summary>
    /// 初始化 API 权限创建 DTO 验证器
    /// </summary>
    public ApiPermissionCreateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
        AddApiPermissionRules();
    }

    /// <summary>
    /// 添加 API 权限特有验证规则
    /// </summary>
    private void AddApiPermissionRules() {
        RuleFor(x => x.HttpMethod)
            .MaximumLength(10).WithMessage("HTTP 方法长度不能超过 10 个字符")
            .Must(method => string.IsNullOrEmpty(method) || new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" }.Contains(method.ToUpper()))
            .WithMessage("HTTP 方法必须是 GET、POST、PUT、DELETE、PATCH、HEAD 或 OPTIONS")
            .When(x => !string.IsNullOrEmpty(x.HttpMethod));

        RuleFor(x => x.ApiPath)
            .NotEmpty().WithMessage("API 路径不能为空")
            .MaximumLength(300).WithMessage("API 路径长度不能超过 300 个字符")
            .Must(path => path.StartsWith("/")).WithMessage("API 路径必须以 / 开头");

        RuleFor(x => x.ModuleName)
            .MaximumLength(100).WithMessage("模块名称长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.ModuleName));
    }
}

/// <summary>
/// API 权限更新 DTO 验证器
/// </summary>
public class ApiPermissionUpdateDtoValidator : PermissionUpdateDtoValidatorBase<ApiPermissionUpdateDto> {
    /// <summary>
    /// 初始化 API 权限更新 DTO 验证器
    /// </summary>
    public ApiPermissionUpdateDtoValidator() {
        AddPermissionRules(x => x.Name, x => x.Code, x => x.Type, x => x.Sort);
        AddApiPermissionRules();
    }

    /// <summary>
    /// 添加 API 权限特有验证规则
    /// </summary>
    private void AddApiPermissionRules() {
        RuleFor(x => x.HttpMethod)
            .MaximumLength(10).WithMessage("HTTP 方法长度不能超过 10 个字符")
            .Must(method => string.IsNullOrEmpty(method) || new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" }.Contains(method.ToUpper()))
            .WithMessage("HTTP 方法必须是 GET、POST、PUT、DELETE、PATCH、HEAD 或 OPTIONS")
            .When(x => !string.IsNullOrEmpty(x.HttpMethod));

        RuleFor(x => x.ApiPath)
            .NotEmpty().WithMessage("API 路径不能为空")
            .MaximumLength(300).WithMessage("API 路径长度不能超过 300 个字符")
            .Must(path => path.StartsWith("/")).WithMessage("API 路径必须以 / 开头");

        RuleFor(x => x.ModuleName)
            .MaximumLength(100).WithMessage("模块名称长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.ModuleName));
    }
}

/// <summary>
/// API 权限操作 DTO 验证器
/// </summary>
public class ApiPermissionActionDtoValidator : DtoValidatorBase<ApiPermissionActionDto> {
    /// <summary>
    /// 初始化 API 权限操作 DTO 验证器
    /// </summary>
    public ApiPermissionActionDtoValidator() {
    }
}

/// <summary>
/// API 权限列表查询验证器
/// </summary>
public class ApiPermissionListQueryValidator : DtoValidatorBase<ApiPermissionListQuery> {
    /// <summary>
    /// 初始化 API 权限列表查询验证器
    /// </summary>
    public ApiPermissionListQueryValidator() {
        RuleFor(x => x.QueryDto.Code)
            .MaximumLength(100).WithMessage("权限编码长度不能超过 100 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.Code));

        RuleFor(x => x.QueryDto.Name)
            .MaximumLength(50).WithMessage("权限名称长度不能超过 50 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.Name));

        RuleFor(x => x.QueryDto.ApiPath)
            .MaximumLength(300).WithMessage("API 路径长度不能超过 300 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.ApiPath));

        RuleFor(x => x.QueryDto.HttpMethod)
            .MaximumLength(10).WithMessage("HTTP 方法长度不能超过 10 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.HttpMethod));

        RuleFor(x => x.QueryDto.ModuleName)
            .MaximumLength(100).WithMessage("模块名称长度不能超过 100 个字符")
            .When(x => x.QueryDto != null && !string.IsNullOrEmpty(x.QueryDto.ModuleName));
    }
}

/// <summary>
/// API 权限分页查询验证器
/// </summary>
public class ApiPermissionPagedQueryValidator : DtoValidatorBase<ApiPermissionPagedQuery> {
    /// <summary>
    /// 初始化 API 权限分页查询验证器
    /// </summary>
    public ApiPermissionPagedQueryValidator() {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("页码必须大于等于 1");

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1).WithMessage("每页大小必须大于等于 1")
            .LessThanOrEqualTo(100).WithMessage("每页大小不能超过 100");
    }
}

/// <summary>
/// API 权限查询参数 DTO 验证器
/// </summary>
public class ApiPermissionQueryDtoValidator : DtoValidatorBase<ApiPermissionQueryDto> {
    /// <summary>
    /// 初始化 API 权限查询参数 DTO 验证器
    /// </summary>
    public ApiPermissionQueryDtoValidator() {
        RuleFor(x => x.Code)
            .MaximumLength(100).WithMessage("权限编码长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("权限名称长度不能超过 50 个字符")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.ApiPath)
            .MaximumLength(300).WithMessage("API 路径长度不能超过 300 个字符")
            .When(x => !string.IsNullOrEmpty(x.ApiPath));

        RuleFor(x => x.HttpMethod)
            .MaximumLength(10).WithMessage("HTTP 方法长度不能超过 10 个字符")
            .When(x => !string.IsNullOrEmpty(x.HttpMethod));

        RuleFor(x => x.ModuleName)
            .MaximumLength(100).WithMessage("模块名称长度不能超过 100 个字符")
            .When(x => !string.IsNullOrEmpty(x.ModuleName));
    }
}
