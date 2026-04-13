/*
 * 文件名称: UserRoleQueryHandlers.cs
 * 功能描述: 用户角色关联查询处理器，处理用户角色查询操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Abstractions.Queries;
using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Caches;
using Domain.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using AutoMapper;
using System.Text;

namespace Application.Queries;

/// <summary>
/// 用户角色列表查询处理器
/// </summary>
public class UserRolesQueryHandler(
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    IMapper mapper,
    ILogger<UserRolesQueryHandler> logger) : IRequestHandler<UserRolesQuery, List<RoleListDto>> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserRolesQueryHandler> _logger = logger;

    /// <summary>
    /// 处理用户角色列表查询
    /// </summary>
    public async Task<List<RoleListDto>> Handle(UserRolesQuery request, CancellationToken cancellationToken) {
        var roleIds = await _userRoleRepository.GetRoleIdsByUserIdAsync(request.UserId, cancellationToken);

        if (roleIds.Count == 0) {
            return [];
        }

        var roles = await _roleRepository.GetListAsync(predicates: [r => roleIds.Contains(r.Id)], cancellationToken: cancellationToken);

        _logger.LogDebug("查询用户角色列表 | UserId: {UserId} | RoleCount: {Count}", 
            request.UserId, roles.Count);

        return _mapper.Map<List<RoleListDto>>(roles);
    }
}

/// <summary>
/// 角色用户列表查询处理器
/// </summary>
public class RoleUsersQueryHandler(
    IUserRoleRepository userRoleRepository,
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<RoleUsersQueryHandler> logger) : IRequestHandler<RoleUsersQuery, List<UserListDto>> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RoleUsersQueryHandler> _logger = logger;

    /// <summary>
    /// 处理角色用户列表查询
    /// </summary>
    public async Task<List<UserListDto>> Handle(RoleUsersQuery request, CancellationToken cancellationToken) {
        var userIds = await _userRoleRepository.GetUserIdsByRoleIdAsync(request.RoleId, cancellationToken);

        if (userIds.Count == 0) {
            return [];
        }

        var users = await _userRepository.GetListAsync(predicates: [u => userIds.Contains(u.Id)], cancellationToken: cancellationToken);

        _logger.LogDebug("查询角色用户列表 | RoleId: {RoleId} | UserCount: {Count}", 
            request.RoleId, users.Count);

        return _mapper.Map<List<UserListDto>>(users);
    }
}

/// <summary>
/// 用户角色ID列表查询处理器
/// </summary>
public class UserRoleIdsQueryHandler(
    IUserRoleRepository userRoleRepository,
    ILogger<UserRoleIdsQueryHandler> logger) : IRequestHandler<UserRoleIdsQuery, List<Guid>> {
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly ILogger<UserRoleIdsQueryHandler> _logger = logger;

    /// <summary>
    /// 处理用户角色ID列表查询
    /// </summary>
    public async Task<List<Guid>> Handle(UserRoleIdsQuery request, CancellationToken cancellationToken) {
        var roleIds = await _userRoleRepository.GetRoleIdsByUserIdAsync(request.UserId, cancellationToken);

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
    IUserRoleRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserRoleListQueryHandler> logger) : ListQueryHandler<UserRole, IUserRoleRepository, UserRoleListQuery, UserRoleQueryDto, UserRoleListDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<UserRole, bool>>> BuildPredicates(UserRoleListQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.UserId.HasValue) {
                predicates.Add(t => t.UserId == query.QueryDto.UserId.Value);
            }
            if (query.QueryDto.RoleId.HasValue) {
                predicates.Add(t => t.RoleId == query.QueryDto.RoleId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(UserRoleListQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.UserId.HasValue) {
                builder.Append($"|UserId={query.QueryDto.UserId.Value}");
            }
            if (query.QueryDto.RoleId.HasValue) {
                builder.Append($"|RoleId={query.QueryDto.RoleId.Value}");
            }
        }

        return builder;
    }
}

/// <summary>
/// 用户角色关联分页查询处理器
/// <para>用于处理用户角色关联关系的分页查询操作，支持缓存</para>
/// </summary>
public class UserRolePagedQueryHandler(
    IUnitOfWork unitOfWork,
    IUserRoleRepository repository,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<UserRolePagedQueryHandler> logger) : PagedQueryHandler<UserRole, IUserRoleRepository, UserRolePagedQuery, UserRoleQueryDto, UserRolePagedDto>(unitOfWork, repository, mapper, cacheProvider, logger) {

    /// <summary>
    /// 构建查询条件
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>查询条件列表</returns>
    protected override List<Expression<Func<UserRole, bool>>> BuildPredicates(UserRolePagedQuery query) {
        var predicates = base.BuildPredicates(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.UserId.HasValue) {
                predicates.Add(t => t.UserId == query.QueryDto.UserId.Value);
            }
            if (query.QueryDto.RoleId.HasValue) {
                predicates.Add(t => t.RoleId == query.QueryDto.RoleId.Value);
            }
        }

        return predicates;
    }

    /// <summary>
    /// 构建缓存键参数部分
    /// </summary>
    /// <param name="query">查询请求</param>
    /// <returns>缓存键参数部分</returns>
    protected override StringBuilder BuildCacheParams(UserRolePagedQuery query) {
        var builder = base.BuildCacheParams(query);

        if (query.QueryDto != null) {
            if (query.QueryDto.UserId.HasValue) {
                builder.Append($"|UserId={query.QueryDto.UserId.Value}");
            }
            if (query.QueryDto.RoleId.HasValue) {
                builder.Append($"|RoleId={query.QueryDto.RoleId.Value}");
            }
        }

        return builder;
    }
}
