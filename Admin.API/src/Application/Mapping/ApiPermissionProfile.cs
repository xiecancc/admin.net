/*
 * 文件名称：ApiPermissionProfile.cs
 * 功能描述：API 权限映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Shared.Enums;

namespace Application.Mapping;

/// <summary>
/// API 权限映射配置
/// </summary>
public class ApiPermissionProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public ApiPermissionProfile() {
        // API 权限实体到 API 权限列表 DTO 的映射
        _ = CreateMap<ApiPermission, ApiPermissionListDto>()
            .IncludeBase<Permission, PermissionListDto>();

        // API 权限实体到 API 权限详情 DTO 的映射
        _ = CreateMap<ApiPermission, ApiPermissionDetailDto>()
            .IncludeBase<Permission, PermissionDetailDto>();

        // API 权限实体到 API 权限分页 DTO 的映射
        _ = CreateMap<ApiPermission, ApiPermissionPagedDto>()
            .IncludeBase<Permission, PermissionPagedDto>();

        // API 权限创建 DTO 到 API 权限实体的映射
        _ = CreateMap<ApiPermissionCreateDto, ApiPermission>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => PermissionType.Api));

        // API 权限更新 DTO 到 API 权限实体的映射
        _ = CreateMap<ApiPermissionUpdateDto, ApiPermission>();
    }
}
