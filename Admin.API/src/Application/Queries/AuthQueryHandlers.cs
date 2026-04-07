/*
 * 文件名称: AuthQueryHandlers.cs
 * 功能描述: 认证相关的查询处理器，包含获取当前用户信息等查询的处理逻辑
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-06
 */

using Application.Contracts.Dtos;
using Application.Contracts.Queries;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Units;
using MediatR;

namespace Application.Queries;

/// <summary>
/// 获取当前用户信息查询处理器
/// <para>处理获取当前用户信息的请求</para>
/// </summary>
/// <param name="unitOfWork">工作单元</param>
public class GetCurrentUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCurrentUserQuery, LoginUserInfoDTO> {
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// 处理获取当前用户信息查询
    /// </summary>
    public async Task<LoginUserInfoDTO> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken) {
        var userRepository = _unitOfWork.GetRepository<IUserRepository, User>();
        var user = await userRepository.GetAsync(request.UserId, cancellationToken) ?? throw new ArgumentException("用户不存在");

        var userRoleRepository = _unitOfWork.GetRepository<IUserRoleRepository, UserRole>();
        var userRoles = await userRoleRepository.GetListAsync(ur => ur.UserId == user.Id, null, cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roleRepository = _unitOfWork.GetRepository<IRoleRepository, Role>();
        var roles = await roleRepository.GetListAsync(r => roleIds.Contains(r.Id), null, cancellationToken);

        return new LoginUserInfoDTO {
            Id = user.Id,
            Email = user.Email,
            NickName = user.NickName,
            Avatar = user.AvatarUrl,
            Roles = roles.Select(r => r.Code).ToList()
        };
    }
}
