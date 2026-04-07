/*
 * 文件名称：MenuPermissionProfile.cs
 * 功能描述：菜单权限映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Shared.Enums;

namespace Application.Mapping;

/// <summary>
/// 菜单权限映射配置
/// </summary>
public class MenuPermissionProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public MenuPermissionProfile() {
        // 菜单权限实体到菜单权限列表 DTO 的映射
        _ = CreateMap<MenuPermission, MenuPermissionListDto>()
            .IncludeBase<Permission, PermissionListDto>();

        // 菜单权限实体到菜单权限详情 DTO 的映射
        _ = CreateMap<MenuPermission, MenuPermissionDetailDto>()
            .IncludeBase<Permission, PermissionDetailDto>();

        // 菜单权限实体到菜单权限分页 DTO 的映射
        _ = CreateMap<MenuPermission, MenuPermissionPagedDto>()
            .IncludeBase<Permission, PermissionPagedDto>();

        // 菜单权限创建 DTO 到菜单权限实体的映射
        _ = CreateMap<MenuPermissionCreateDto, MenuPermission>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => PermissionType.Menu));

        // 菜单权限更新 DTO 到菜单权限实体的映射
        _ = CreateMap<MenuPermissionUpdateDto, MenuPermission>();
    }
}
