/*
 * 文件名称: ButtonPermissionQueryHandlers.cs
 * 功能描述: 按钮权限相关查询处理器，包含按钮权限的所有查询处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

using Application.Abstractions.Queries;
using Application.Contracts.Queries;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Constants;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using AutoMapper;

namespace Application.Queries;

/// <summary>
/// 按钮权限根据ID查询处理器
/// <para>用于处理按钮权限根据ID获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ButtonPermissionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<ButtonPermissionByIdQuery, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionDetailDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.Prefix;
}

/// <summary>
/// 按钮权限列表查询处理器
/// <para>用于处理按钮权限列表获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ButtonPermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainListQueryHandler<ButtonPermissionListQuery, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionListDto, ButtonPermissionQueryParameters>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.Prefix;
}

/// <summary>
/// 按钮权限分页查询处理器
/// <para>用于处理按钮权限分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ButtonPermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainPagedQueryHandler<ButtonPermissionPagedQuery, ButtonPermission, IPermissionRepository<ButtonPermission>, ButtonPermissionPagedDto, ButtonPermissionQueryParameters>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.Prefix;
}
