/*
 * 文件名称：PermissionProfile.cs
 * 功能描述：权限基类映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

/// <summary>
/// 权限基类映射配置
/// </summary>
public class PermissionProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public PermissionProfile() {
        // 权限创建 DTO 到权限实体的映射
        _ = CreateMap<PermissionCreateDto, Permission>()
            .ForMember(dest => dest.Type, opt => opt.Ignore()); // 类型由具体权限类型决定

        // 权限更新 DTO 到权限实体的映射
        _ = CreateMap<PermissionUpdateDto, Permission>()
            .ForMember(dest => dest.Type, opt => opt.Ignore()); // 类型由具体权限类型决定

        // 权限实体到权限列表 DTO 的映射
        _ = CreateMap<Permission, PermissionListDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

        // 权限实体到权限详情 DTO 的映射
        _ = CreateMap<Permission, PermissionDetailDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

        // 权限实体到权限分页 DTO 的映射
        _ = CreateMap<Permission, PermissionPagedDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));
    }
}
