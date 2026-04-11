/*
 * 文件名称：UserRoleDtosValidator.cs
 * 功能描述：用户角色关联相关 DTO 验证器，包含用户角色关联创建、操作等验证规则
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-11
 */

using Application.Contracts.Dtos;

namespace Application.Contracts.Validators;

/// <summary>
/// 用户角色关联创建 DTO 验证器
/// </summary>
public class UserRoleCreateDtoValidator : RelationCreateDtoValidatorBase<UserRoleCreateDto> {
    /// <summary>
    /// 初始化用户角色关联创建 DTO 验证器
    /// </summary>
    public UserRoleCreateDtoValidator() {
        AddRelationRules(x => x.UserId, x => x.RoleId, "用户 ID", "角色 ID");
    }
}

/// <summary>
/// 用户角色关联操作 DTO 验证器
/// </summary>
public class UserRoleActionDtoValidator : RelationActionDtoValidatorBase<UserRoleActionDto> {
    /// <summary>
    /// 初始化用户角色关联操作 DTO 验证器
    /// </summary>
    public UserRoleActionDtoValidator() {
        AddRelationRules(x => x.UserId, x => x.RoleId, "用户 ID", "角色 ID");
    }
}
