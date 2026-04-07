/*
 * 文件名称：RoleProfile.cs
 * 功能描述：角色相关映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;
/// <summary>
/// 角色相关映射配置
/// </summary>
public class RoleProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public RoleProfile() {
        // 角色创建 DTO 到角色实体的映射
        _ = CreateMap<RoleCreateDto, Role>();

        // 角色更新 DTO 到角色实体的映射
        _ = CreateMap<RoleUpdateDto, Role>();

        // 角色实体到角色列表 DTO 的映射
        _ = CreateMap<Role, RoleListDto>()
            .IncludeBase<Domain.Shared.Entities.AggregateBase, AggregateListDto>();

        // 角色实体到角色详情 DTO 的映射
        _ = CreateMap<Role, RoleDetailDto>()
            .IncludeBase<Domain.Shared.Entities.AggregateBase, AggregateDetailDto>()
            .ForMember(dest => dest.PermissionIds, opt => opt.MapFrom(src => src.Permissions.Select(p => p.Id)));

        // 角色实体到角色分页 DTO 的映射
        _ = CreateMap<Role, RolePagedDto>()
            .IncludeBase<Domain.Shared.Entities.AggregateBase, AggregatePagedDto>();
    }
}
