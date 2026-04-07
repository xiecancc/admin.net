/*
 * 文件名称: ApiPermissionQueryHandlers.cs
 * 功能描述: API权限查询处理器，处理API权限相关的查询
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
/// API权限根据ID查询处理器
/// <para>处理API权限的根据ID查询操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ApiPermissionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : AggregateByIdQueryHandler<ApiPermissionByIdQuery, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionDetailDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.PREFIX;
}

/// <summary>
/// API权限列表查询处理器
/// <para>处理API权限的列表查询操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ApiPermissionListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainListQueryHandler<ApiPermissionListQuery, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionListDto, ApiPermissionQueryParameters>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.PREFIX;
}

/// <summary>
/// API权限分页查询处理器
/// <para>用于处理API权限分页获取操作，支持缓存</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">映射器，不能为空</param>
/// <param name="cacheProvider">缓存提供者，不能为空</param>
public class ApiPermissionPagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainPagedQueryHandler<ApiPermissionPagedQuery, ApiPermission, IPermissionRepository<ApiPermission>, ApiPermissionPagedDto, ApiPermissionQueryParameters>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => CacheKeyConstants.Permission.PREFIX;
}
