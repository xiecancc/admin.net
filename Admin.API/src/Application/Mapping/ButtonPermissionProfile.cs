/*
 * 文件名称: ButtonPermissionProfile.cs
 * 功能描述: 按钮权限映射配置
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Shared.Enums;

namespace Application.Mapping;

/// <summary>
/// 按钮权限映射配置
/// </summary>
public class ButtonPermissionProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public ButtonPermissionProfile() {
        // 按钮权限实体到按钮权限列表 DTO 的映射
        CreateMap<ButtonPermission, ButtonPermissionListDto>()
            .IncludeBase<PermissionBase, PermissionListDto>();

        // 按钮权限实体到按钮权限详情 DTO 的映射
        CreateMap<ButtonPermission, ButtonPermissionDetailDto>()
            .IncludeBase<PermissionBase, PermissionDetailDto>();

        // 按钮权限实体到按钮权限分页 DTO 的映射
        CreateMap<ButtonPermission, ButtonPermissionPagedDto>()
            .IncludeBase<PermissionBase, PermissionPagedDto>();

        // 按钮权限创建 DTO 到按钮权限实体的映射
        CreateMap<ButtonPermissionCreateDto, ButtonPermission>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => PermissionType.Button));

        // 按钮权限更新 DTO 到按钮权限实体的映射
        CreateMap<ButtonPermissionUpdateDto, ButtonPermission>();
    }
}
