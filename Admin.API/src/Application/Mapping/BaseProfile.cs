/*
 * 文件名称：BaseProfile.cs
 * 功能描述：基础映射配置
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-01
 */

using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Shared.Dtos;
using Domain.Shared.Entities;

namespace Application.Mapping;
/// <summary>
/// 基础映射配置
/// </summary>
public class BaseProfile : Profile {
    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseProfile() {
        // 基础映射配置
        CreateMap<Domain.Shared.Entities.DomainBase, DetailDto>();
        CreateMap<CreateDto, Domain.Shared.Entities.DomainBase>();
        CreateMap<UpdateDto, Domain.Shared.Entities.DomainBase>();
        CreateMap<ActionDto, Domain.Shared.Entities.DomainBase>();

        // 聚合根映射配置
        CreateMap<AggregateBase, AggregateListDto>();
        CreateMap<AggregateBase, AggregateDetailDto>();
        CreateMap<AggregateBase, AggregatePagedDto>();
        CreateMap<AggregateCreateDto, AggregateBase>();
        CreateMap<AggregateUpdateDto, AggregateBase>();
        CreateMap<AggregateActionDto, AggregateBase>();

        // 分页结果 DTO 的映射
        CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>))
            .ConvertUsing(typeof(PagedResultDtoConverter<,>));
    }
}

/// <summary>
/// 分页结果 DTO 转换器
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TDestination">目标类型</typeparam>
public class PagedResultDtoConverter<TSource, TDestination> : ITypeConverter<PagedResponse<TSource>, PagedResponse<TDestination>> {
    /// <summary>
    /// 执行转换
    /// </summary>
    /// <param name="source">源对象</param>
    /// <param name="destination">目标对象</param>
    /// <param name="context">上下文</param>
    /// <returns>转换后的目标对象</returns>
    public PagedResponse<TDestination> Convert(PagedResponse<TSource> source, PagedResponse<TDestination> destination, ResolutionContext context) {
        return new(context.Mapper.Map<List<TDestination>>(source.Items), source.Total, source.Page, source.Size);
    }
}
