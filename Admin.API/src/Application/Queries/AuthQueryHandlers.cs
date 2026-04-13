/*
 * 文件名称: AuthQueryHandlers.cs
 * 功能描述: 认证相关的查询处理器，包含获取当前用户信息等查询的处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Units;
using MediatR;

namespace Application.Queries;

/// <summary>
/// 获取当前用户信息查询处理器
/// <para>处理获取当前用户信息的请求</para>
/// </summary>
public class GetProfileQueryHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository) : IRequestHandler<GetProfileQuery, LoginUserInfoDto> {
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    private readonly IRoleRepository _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));

    /// <summary>
    /// 处理获取当前用户信息查询
    /// </summary>
    public async Task<LoginUserInfoDto> Handle(GetProfileQuery request, CancellationToken cancellationToken) {
        var user = await _userRepository.GetAsync(request.UserId, cancellationToken) ?? throw new ArgumentException("用户不存在");

        var userRoles = await _userRoleRepository.GetListAsync([ur => ur.UserId == user.Id], null, cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = await _roleRepository.GetListAsync([r => roleIds.Contains(r.Id)], null, cancellationToken);

        return new LoginUserInfoDto {
            Id = user.Id,
            Email = user.Email,
            NickName = user.NickName,
            Avatar = user.AvatarUrl,
            Roles = roles.Select(r => r.Code).ToList()
        };
    }
}
