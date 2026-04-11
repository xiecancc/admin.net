/*
 * 文件名称：AuthProfile.cs
 * 功能描述：认证相关映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-03-13
 */


using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;
/// <summary>
/// 认证相关映射配置
/// </summary>
public class AuthProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public AuthProfile() {
        CreateMap<Role, RoleListDto>();
    }
}
