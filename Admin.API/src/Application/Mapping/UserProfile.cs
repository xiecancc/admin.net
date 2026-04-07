/*
 * 文件名称：UserProfile.cs
 * 功能描述：用户相关映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Shared.Enums;

namespace Application.Mapping;
/// <summary>
/// 用户相关映射配置
/// </summary>
public class UserProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public UserProfile() {
        // 用户创建 DTO 到用户实体的映射
        _ = CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ? UserStatus.Normal : UserStatus.Disabled))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar));

        // 用户更新 DTO 到用户实体的映射
        _ = CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ? UserStatus.Normal : UserStatus.Disabled))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar));

        // 用户实体到用户列表 DTO 的映射
        _ = CreateMap<User, UserListDto>()
            .IncludeBase<Domain.Shared.Entities.AggregateBase, AggregateListDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == UserStatus.Normal))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarUrl));

        // 用户实体到用户详情 DTO 的映射
        _ = CreateMap<User, UserDetailDto>()
            .IncludeBase<Domain.Shared.Entities.AggregateBase, AggregateDetailDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == UserStatus.Normal))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.Roles.Select(r => r.Id)));

        // 用户实体到用户分页 DTO 的映射
        _ = CreateMap<User, UserPagedDto>()
            .IncludeBase<Domain.Shared.Entities.AggregateBase, AggregatePagedDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == UserStatus.Normal))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.AvatarUrl));
    }
}
