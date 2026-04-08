/*
 * 文件名称: DepartmentProfile.cs
 * 功能描述: 部门相关的 AutoMapper 配置
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-08
 */

using Application.Contracts.Dtos;
using Domain.Entities;
using AutoMapper;

namespace Application.Mapping;

/// <summary>
/// 部门相关的 AutoMapper 配置
/// <para>配置部门实体与 DTO 之间的映射关系</para>
/// </summary>
public class DepartmentProfile : Profile {
    /// <summary>
    /// 初始化部门 AutoMapper 配置
    /// </summary>
    public DepartmentProfile() {
        // 实体到 DTO 的映射
        CreateMap<Department, DepartmentListDto>()
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));

        CreateMap<Department, DepartmentDetailDto>()
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null))
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));

        CreateMap<Department, DepartmentPagedDto>()
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null));

        // DTO 到实体的映射
        CreateMap<DepartmentCreateDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Parent, opt => opt.Ignore())
            .ForMember(dest => dest.Children, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore());

        CreateMap<DepartmentUpdateDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Parent, opt => opt.Ignore())
            .ForMember(dest => dest.Children, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore());

        // 用户部门角色映射
        CreateMap<UserDepartmentRoleDto, UserDepartmentRole>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore());
    }
}