/*
 * 文件名称: UserCommandHandlers.cs
 * 功能描述: 用户命令处理器，处理用户相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-13
 */

using Application.Abstractions.Commands;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared.Units;
using Domain.Shared.Utils;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 用户创建命令处理器
/// <para>处理用户的创建操作，包含邮箱唯一性验证和密码哈希处理</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="repository">用户仓储，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserCreateCommandHandler(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper, ILogger<UserCreateCommandHandler> logger)
    : DomainCreateCommandHandler<User, IUserRepository, UserCreateCommand, UserCreateDto>(unitOfWork, repository, mapper, logger) {

    /// <summary>
    /// 在事务内执行用户创建业务逻辑（包含邮箱唯一性验证和密码哈希处理）
    /// </summary>
    protected override async Task<bool> ExecuteInTransactionAsync(UserCreateCommand request, CancellationToken cancellationToken) {
        var duplicateEmails = new List<string>();

        foreach (var dto in request.Data) {
            var emailExists = await Repository.IsEmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists) {
                duplicateEmails.Add(dto.Email);
            }
        }

        if (duplicateEmails.Count > 0) {
            throw new InvalidOperationException(
                $"用户创建失败，以下邮箱已存在: {string.Join(", ", duplicateEmails)}。" +
                "请使用不同的邮箱地址。");
        }

        var entities = Mapper.Map<List<User>>(request.Data);

        foreach (var entity in entities) {
            var dto = request.Data.First(d => d.Email == entity.Email);
            entity.PasswordHash = PasswordUtil.HashPassword(dto.Password);
        }

        return await Repository.InsertAsync(entities, cancellationToken);
    }
}

/// <summary>
/// 用户更新命令处理器
/// <para>处理用户的更新操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="repository">用户仓储，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserUpdateCommandHandler(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper, ILogger<UserUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<User, IUserRepository, UserUpdateCommand, UserUpdateDto>(unitOfWork, repository, mapper, logger);

/// <summary>
/// 用户删除命令处理器
/// <para>处理用户的删除操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="repository">用户仓储，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserDeleteCommandHandler(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper, ILogger<UserDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<User, IUserRepository, UserDeleteCommand, UserActionDto>(unitOfWork, repository, mapper, logger);

/// <summary>
/// 用户恢复命令处理器
/// <para>处理用户的恢复操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="repository">用户仓储，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserRestoreCommandHandler(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper, ILogger<UserRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<User, IUserRepository, UserRestoreCommand, UserActionDto>(unitOfWork, repository, mapper, logger);
