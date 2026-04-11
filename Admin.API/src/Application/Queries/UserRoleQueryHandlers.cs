/*
 * 文件名称: UserRoleQueryHandlers.cs
 * 功能描述: 用户角色关联查询处理器，处理用户角色查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Queries;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using AutoMapper;

namespace Application.Queries;

/// <summary>
/// 用户角色列表查询处理器
/// </summary>
public class UserRolesQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<UserRolesQueryHandler> logger) : IRequestHandler<UserRolesQuery, List<Role>> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UserRolesQueryHandler> _logger = logger;

    /// <summary>
    /// 处理用户角色列表查询
    /// </summary>
    public async Task<List<Role>> Handle(UserRolesQuery request, CancellationToken cancellationToken) {
        var userRoleRepository = _unitOfWork.GetRepository<IUserRoleRepository, UserRole>();
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(request.UserId, cancellationToken);

        if (roleIds.Count == 0) {
            return [];
        }

        var roleRepository = _unitOfWork.GetRepository<IRoleRepository, Role>();
        var roles = await roleRepository.GetListAsync(predicates: [r => roleIds.Contains(r.Id)], cancellationToken: cancellationToken);

        _logger.LogDebug("查询用户角色列表 | UserId: {UserId} | RoleCount: {Count}", 
            request.UserId, roles.Count);

        return roles;
    }
}

/// <summary>
/// 角色用户列表查询处理器
/// </summary>
public class RoleUsersQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<RoleUsersQueryHandler> logger) : IRequestHandler<RoleUsersQuery, List<User>> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<RoleUsersQueryHandler> _logger = logger;

    /// <summary>
    /// 处理角色用户列表查询
    /// </summary>
    public async Task<List<User>> Handle(RoleUsersQuery request, CancellationToken cancellationToken) {
        var userRoleRepository = _unitOfWork.GetRepository<IUserRoleRepository, UserRole>();
        var userIds = await userRoleRepository.GetUserIdsByRoleIdAsync(request.RoleId, cancellationToken);

        if (userIds.Count == 0) {
            return [];
        }

        var userRepository = _unitOfWork.GetRepository<IUserRepository, User>();
        var users = await userRepository.GetListAsync(predicates: [u => userIds.Contains(u.Id)], cancellationToken: cancellationToken);

        _logger.LogDebug("查询角色用户列表 | RoleId: {RoleId} | UserCount: {Count}", 
            request.RoleId, users.Count);

        return users;
    }
}

/// <summary>
/// 用户角色ID列表查询处理器
/// </summary>
public class UserRoleIdsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<UserRoleIdsQueryHandler> logger) : IRequestHandler<UserRoleIdsQuery, List<Guid>> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UserRoleIdsQueryHandler> _logger = logger;

    /// <summary>
    /// 处理用户角色ID列表查询
    /// </summary>
    public async Task<List<Guid>> Handle(UserRoleIdsQuery request, CancellationToken cancellationToken) {
        var userRoleRepository = _unitOfWork.GetRepository<IUserRoleRepository, UserRole>();
        var roleIds = await userRoleRepository.GetRoleIdsByUserIdAsync(request.UserId, cancellationToken);

        _logger.LogDebug("查询用户角色ID列表 | UserId: {UserId} | RoleCount: {Count}", 
            request.UserId, roleIds.Count);

        return roleIds;
    }
}

/// <summary>
/// 用户角色关联列表查询处理器
/// <para>用于处理用户角色关联关系的列表查询操作，支持缓存</para>
/// </summary>
public class UserRoleListQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainListQueryHandler<UserRoleListQuery, UserRole, IUserRoleRepository, UserRoleListDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => "userrole";

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<UserRole, bool>>> BuildPredicates(UserRoleListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.UserId.HasValue) {
            predicates.Add(t => t.UserId == query.UserId.Value);
        }
        if (query.RoleId.HasValue) {
            predicates.Add(t => t.RoleId == query.RoleId.Value);
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override string BuildCacheKey(UserRoleListQuery query) {
        var parts = new List<string>();

        if (query.UserId.HasValue) {
            parts.Add($"UserId={query.UserId.Value}");
        }
        if (query.RoleId.HasValue) {
            parts.Add($"RoleId={query.RoleId.Value}");
        }

        return string.Join("|", parts);
    }
}

/// <summary>
/// 用户角色关联分页查询处理器
/// <para>用于处理用户角色关联关系的分页查询操作，支持缓存</para>
/// </summary>
public class UserRolePagedQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider) : DomainPagedQueryHandler<UserRolePagedQuery, UserRole, IUserRoleRepository, UserRolePagedDto>(unitOfWork, mapper, cacheProvider) {
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    protected override string CacheKeyPrefix => "userrole";

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<UserRole, bool>>> BuildPredicates(UserRolePagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.UserId.HasValue) {
            predicates.Add(t => t.UserId == query.UserId.Value);
        }
        if (query.RoleId.HasValue) {
            predicates.Add(t => t.RoleId == query.RoleId.Value);
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override string BuildCacheKey(UserRolePagedQuery query) {
        var parts = new List<string>();

        if (query.UserId.HasValue) {
            parts.Add($"UserId={query.UserId.Value}");
        }
        if (query.RoleId.HasValue) {
            parts.Add($"RoleId={query.RoleId.Value}");
        }

        return string.Join("|", parts);
    }
}
