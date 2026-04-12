/*
 * 文件名称: UserCommandHandlers.cs
 * 功能描述: 用户命令处理器，处理用户相关的命令
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Commands;
using Application.Contracts.Commands;
using Application.Contracts.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Shared.Units;
using Infrastructure.Shared.Utils;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// 用户创建命令处理器
/// <para>处理用户的创建操作，包含邮箱唯一性验证和密码哈希处理</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserCreateCommandHandler> logger)
    : DomainCreateCommandHandler<User, IUserRepository, UserCreateCommand, UserCreateDto>(unitOfWork, mapper, logger) {

    /// <summary>
    /// 处理用户创建命令
    /// </summary>
    /// <param name="request">创建命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <exception cref="ArgumentNullException">当命令或数据为 null 时抛出</exception>
    /// <exception cref="ArgumentException">当数据集合为空时抛出</exception>
    /// <exception cref="InvalidOperationException">当邮箱已存在或创建操作失败时抛出</exception>
    public override async Task<bool> Handle(UserCreateCommand request, CancellationToken cancellationToken) {
        ValidateRequest(request, cancellationToken);
        ValidateCreateData(request.Data);

        try {
            Logger.LogInformation("开始创建用户，数量: {Count}", request.Data.Count);

            var result = await UnitOfWork.ExecuteInTransactionAsync(async () => {
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
            }, cancellationToken);

            if (result) {
                Logger.LogInformation("创建用户成功，数量: {Count}", request.Data.Count);
            }

            return result;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("创建用户操作被取消");
            throw;
        }
        catch (InvalidOperationException) {
            throw;
        }
        catch (Exception ex) {
            LogException(ex, "创建", $"数量: {request.Data.Count}");
            throw HandleException(ex, "创建", $"数量: {request.Data.Count}");
        }
    }
}

/// <summary>
/// 用户更新命令处理器
/// <para>处理用户的更新操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserUpdateCommandHandler> logger)
    : AggregateUpdateCommandHandler<User, IUserRepository, UserUpdateCommand, UserUpdateDto>(unitOfWork, mapper, logger);

/// <summary>
/// 用户删除命令处理器
/// <para>处理用户的删除操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserDeleteCommandHandler> logger)
    : AggregateDeleteCommandHandler<User, IUserRepository, UserDeleteCommand, UserActionDto>(unitOfWork, mapper, logger);

/// <summary>
/// 用户恢复命令处理器
/// <para>处理用户的恢复操作</para>
/// </summary>
/// <param name="unitOfWork">工作单元，不能为空</param>
/// <param name="mapper">对象映射器，不能为空</param>
/// <param name="logger">日志记录器，不能为空</param>
public class UserRestoreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserRestoreCommandHandler> logger)
    : AggregateRestoreCommandHandler<User, IUserRepository, UserRestoreCommand, UserActionDto>(unitOfWork, mapper, logger);
