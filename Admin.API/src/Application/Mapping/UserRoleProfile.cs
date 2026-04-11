/*
 * 文件名称：UserRoleProfile.cs
 * 功能描述：用户角色关联映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

/// <summary>
/// 用户角色关联映射配置
/// </summary>
public class UserRoleProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public UserRoleProfile() {
        // 用户角色关联创建 DTO 到用户角色关联实体的映射
        CreateMap<UserRoleCreateDto, UserRole>();

        // 用户角色关联实体到用户角色关联列表 DTO 的映射
        CreateMap<UserRole, UserRoleListDto>()
            .IncludeBase<Domain.Shared.Entities.DomainBase, ListDto>();

        // 用户角色关联实体到用户角色关联分页 DTO 的映射
        CreateMap<UserRole, UserRolePagedDto>()
            .IncludeBase<Domain.Shared.Entities.DomainBase, PagedDto>();
    }
}
