/*
 * 文件名称: PermissionProfile.cs
 * 功能描述: 权限基类映射配置
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
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
        // 权限实体到权限列表 DTO 的映射
        CreateMap<PermissionBase, PermissionListDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

        // 权限实体到权限详情 DTO 的映射
        CreateMap<PermissionBase, PermissionDetailDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

        // 权限实体到权限分页 DTO 的映射
        CreateMap<PermissionBase, PermissionPagedDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));
    }
}
