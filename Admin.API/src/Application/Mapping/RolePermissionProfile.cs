/*
 * 文件名称：RolePermissionProfile.cs
 * 功能描述：角色权限关联映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

/// <summary>
/// 角色权限关联映射配置
/// </summary>
public class RolePermissionProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public RolePermissionProfile() {
        // 角色权限关联创建 DTO 到角色权限关联实体的映射
        _ = CreateMap<RolePermissionCreateDto, RolePermission>();

        // 角色权限关联实体到角色权限关联列表 DTO 的映射
        _ = CreateMap<RolePermission, RolePermissionListDto>()
            .IncludeBase<Domain.Shared.Entities.DomainBase, DomainListDto>();

        // 角色权限关联实体到角色权限关联分页 DTO 的映射
        _ = CreateMap<RolePermission, RolePermissionPagedDto>()
            .IncludeBase<Domain.Shared.Entities.DomainBase, DomainPagedDto>();
    }
}
