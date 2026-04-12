/*
 * 文件名称：RolePermissionDtosValidator.cs
 * 功能描述：角色权限关联相关 DTO 验证器，包含角色权限关联创建、操作等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-12
 */

using Application.Contracts.Dtos;
using FluentValidation;

namespace Application.Contracts.Validators;

/// <summary>
/// 角色权限关联创建 DTO 验证器
/// </summary>
public class RolePermissionCreateDtoValidator : RelationCreateDtoValidatorBase<RolePermissionCreateDto> {
    /// <summary>
    /// 初始化角色权限关联创建 DTO 验证器
    /// </summary>
    public RolePermissionCreateDtoValidator() {
        AddRelationRules(x => x.RoleId, x => x.PermissionId, "角色 ID", "权限 ID");
    }
}

/// <summary>
/// 角色权限关联操作 DTO 验证器
/// </summary>
public class RolePermissionActionDtoValidator : RelationActionDtoValidatorBase<RolePermissionActionDto> {
    /// <summary>
    /// 初始化角色权限关联操作 DTO 验证器
    /// </summary>
    public RolePermissionActionDtoValidator() {
        AddRelationRules(x => x.RoleId, x => x.PermissionId, "角色 ID", "权限 ID");
    }
}

/// <summary>
/// 角色权限关联查询参数 DTO 验证器
/// </summary>
public class RolePermissionQueryDtoValidator : DtoValidatorBase<RolePermissionQueryDto> {
    /// <summary>
    /// 初始化角色权限关联查询参数 DTO 验证器
    /// </summary>
    public RolePermissionQueryDtoValidator() {
    }
}
