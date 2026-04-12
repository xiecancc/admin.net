/*
 * 文件名称: CommandHandlerExceptionTests.cs
 * 功能描述: 命令处理器异常处理测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Commands;
using Application.Contracts.Abstractions.Commands;
using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Shared.Entities;
using Domain.Shared.Events;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Test.Handlers;

/// <summary>
/// 命令处理器异常处理测试类
/// <para>测试命令处理器的异常处理逻辑</para>
/// </summary>
public class CommandHandlerExceptionTests {
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<TestCommandHandler>> _mockLogger;

    public CommandHandlerExceptionTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<TestCommandHandler>>();
    }

    #region ValidateRequest Tests

    /// <summary>
    /// 测试验证请求 - null 请求
    /// <para>场景：传入 null 命令请求</para>
    /// <para>预期：抛出 ArgumentNullException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithNullRequest_ShouldThrowArgumentNullException() {
        var handler = CreateHandler();

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleTest(null!));
    }

    /// <summary>
    /// 测试验证请求 - 已取消的取消令牌
    /// <para>场景：传入已取消的取消令牌</para>
    /// <para>预期：抛出 OperationCanceledException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException() {
        var handler = CreateHandler();
        var command = CreateCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => handler.HandleTest(command, cts.Token));
    }

    #endregion

    #region HandleException Tests

    /// <summary>
    /// 测试处理异常 - ArgumentNullException
    /// <para>场景：抛出 ArgumentNullException</para>
    /// <para>预期：原样抛出异常</para>
    /// </summary>
    [Fact]
    public void HandleException_WithArgumentNullException_ShouldReturnOriginalException() {
        var handler = CreateHandler();
        var exception = new ArgumentNullException("param");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.Same(exception, result);
    }

    /// <summary>
    /// 测试处理异常 - ArgumentException
    /// <para>场景：抛出 ArgumentException</para>
    /// <para>预期：原样抛出异常</para>
    /// </summary>
    [Fact]
    public void HandleException_WithArgumentException_ShouldReturnOriginalException() {
        var handler = CreateHandler();
        var exception = new ArgumentException("Invalid argument");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.Same(exception, result);
    }

    /// <summary>
    /// 测试处理异常 - InvalidOperationException
    /// <para>场景：抛出 InvalidOperationException</para>
    /// <para>预期：原样抛出异常</para>
    /// </summary>
    [Fact]
    public void HandleException_WithInvalidOperationException_ShouldReturnOriginalException() {
        var handler = CreateHandler();
        var exception = new InvalidOperationException("Invalid operation");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.Same(exception, result);
    }

    /// <summary>
    /// 测试处理异常 - KeyNotFoundException
    /// <para>场景：抛出 KeyNotFoundException</para>
    /// <para>预期：原样抛出异常</para>
    /// </summary>
    [Fact]
    public void HandleException_WithKeyNotFoundException_ShouldReturnOriginalException() {
        var handler = CreateHandler();
        var exception = new KeyNotFoundException("Key not found");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.Same(exception, result);
    }

    /// <summary>
    /// 测试处理异常 - OperationCanceledException
    /// <para>场景：抛出 OperationCanceledException</para>
    /// <para>预期：转换为包含上下文信息的 OperationCanceledException</para>
    /// </summary>
    [Fact]
    public void HandleException_WithOperationCanceledException_ShouldReturnOperationCanceledException() {
        var handler = CreateHandler();
        var exception = new OperationCanceledException("Operation cancelled");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.IsType<OperationCanceledException>(result);
        Assert.Contains("操作被取消", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    /// <summary>
    /// 测试处理异常 - TimeoutException
    /// <para>场景：抛出 TimeoutException</para>
    /// <para>预期：转换为包含超时信息的 InvalidOperationException</para>
    /// </summary>
    [Fact]
    public void HandleException_WithTimeoutException_ShouldReturnInvalidOperationException() {
        var handler = CreateHandler();
        var exception = new TimeoutException("Timeout occurred");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.IsType<InvalidOperationException>(result);
        Assert.Contains("操作超时", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    /// <summary>
    /// 测试处理异常 - 未知异常
    /// <para>场景：抛出未知类型的异常</para>
    /// <para>预期：转换为 InvalidOperationException</para>
    /// </summary>
    [Fact]
    public void HandleException_WithUnknownException_ShouldReturnInvalidOperationException() {
        var handler = CreateHandler();
        var exception = new Exception("Unknown error");

        var result = handler.HandleExceptionTest(exception, "测试");

        Assert.IsType<InvalidOperationException>(result);
        Assert.Contains("测试操作失败", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    /// <summary>
    /// 测试处理异常 - 包含上下文信息
    /// <para>场景：处理异常时提供上下文信息</para>
    /// <para>预期：错误消息包含上下文信息</para>
    /// </summary>
    [Fact]
    public void HandleException_WithContext_ShouldIncludeContextInMessage() {
        var handler = CreateHandler();
        var exception = new Exception("Unknown error");
        var context = "ID: 123";

        var result = handler.HandleExceptionTest(exception, "测试", context);

        Assert.Contains(context, result.Message);
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// 测试完整处理流程 - 仓储抛出异常
    /// <para>场景：仓储操作抛出异常</para>
    /// <para>预期：异常被正确处理</para>
    /// </summary>
    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldHandleException() {
        var command = CreateCommand();
        var repositoryException = new InvalidOperationException("Database error");

        var mockRepository = new Mock<IDomainRepository<TestDomain>>();
        mockRepository.Setup(x => x.InsertAsync(It.IsAny<List<TestDomain>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(repositoryException);

        _mockUnitOfWork.Setup(x => x.GetRepository<IDomainRepository<TestDomain>, TestDomain>())
            .Returns(mockRepository.Object);

        _mockUnitOfWork.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<bool>>>(), It.IsAny<CancellationToken>()))
            .Returns(async (Func<Task<bool>> action, CancellationToken _) => await action());

        _mockMapper.Setup(x => x.Map<List<TestDomain>>(It.IsAny<List<TestCreateDto>>()))
            .Returns([new TestDomain()]);

        var handler = CreateHandler();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleTest(command));

        Assert.Contains("Database error", exception.Message);
    }

    /// <summary>
    /// 测试完整处理流程 - 操作被取消
    /// <para>场景：操作被取消</para>
    /// <para>预期：抛出 OperationCanceledException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WhenOperationCancelled_ShouldThrowOperationCanceledException() {
        var command = CreateCommand();
        var cts = new CancellationTokenSource();

        var mockRepository = new Mock<IDomainRepository<TestDomain>>();
        mockRepository.Setup(x => x.InsertAsync(It.IsAny<List<TestDomain>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork.Setup(x => x.GetRepository<IDomainRepository<TestDomain>, TestDomain>())
            .Returns(mockRepository.Object);

        _mockUnitOfWork.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<bool>>>(), It.IsAny<CancellationToken>()))
            .Returns(async (Func<Task<bool>> action, CancellationToken _) => await action());

        _mockMapper.Setup(x => x.Map<List<TestDomain>>(It.IsAny<List<TestCreateDto>>()))
            .Returns([new TestDomain()]);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<OperationCanceledException>(() => handler.HandleTest(command, cts.Token));
    }

    /// <summary>
    /// 测试完整处理流程 - 成功
    /// <para>场景：操作成功完成</para>
    /// <para>预期：返回 true</para>
    /// </summary>
    [Fact]
    public async Task Handle_WhenSuccessful_ShouldReturnTrue() {
        var command = CreateCommand();

        var mockRepository = new Mock<IDomainRepository<TestDomain>>();
        mockRepository.Setup(x => x.InsertAsync(It.IsAny<List<TestDomain>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUnitOfWork.Setup(x => x.GetRepository<IDomainRepository<TestDomain>, TestDomain>())
            .Returns(mockRepository.Object);

        _mockUnitOfWork.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<bool>>>(), It.IsAny<CancellationToken>()))
            .Returns(async (Func<Task<bool>> action, CancellationToken _) => await action());

        _mockMapper.Setup(x => x.Map<List<TestDomain>>(It.IsAny<List<TestCreateDto>>()))
            .Returns([new TestDomain()]);

        var handler = CreateHandler();

        var result = await handler.HandleTest(command);

        Assert.True(result);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 创建测试命令处理器
    /// </summary>
    private TestCommandHandler CreateHandler() {
        return new TestCommandHandler(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    /// <summary>
    /// 创建测试命令
    /// </summary>
    private static TestCommand CreateCommand() {
        return new TestCommand {
            Data = [new TestCreateDto { Name = "Test" }]
        };
    }

    #endregion
}

#region Test Helpers

/// <summary>
/// 测试领域实体
/// </summary>
internal class TestDomain : DomainBase;

/// <summary>
/// 测试创建 DTO
/// </summary>
internal class TestCreateDto : CreateDto {
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 测试命令
/// </summary>
internal class TestCommand : CreateCommand<TestCreateDto> {
    public TestCommand() : base([]) { }
    public TestCommand(List<TestCreateDto> data) : base(data) { }
}

/// <summary>
/// 测试命令处理器
/// </summary>
internal class TestCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<TestCommandHandler> logger)
    : CommandHandler<TestDomain, IDomainRepository<TestDomain>, TestCommand, bool>(unitOfWork, mapper, logger) {

    public async Task<bool> HandleTest(TestCommand request, CancellationToken cancellationToken = default) {
        ValidateRequest(request, cancellationToken);

        try {
            var entities = Mapper.Map<List<TestDomain>>(request.Data);
            return await Repository.InsertAsync(entities, cancellationToken);
        }
        catch (Exception ex) {
            LogException(ex, "创建", $"数量: {request.Data.Count}");
            throw HandleException(ex, "创建", $"数量: {request.Data.Count}");
        }
    }

    public override Task<bool> Handle(TestCommand request, CancellationToken cancellationToken) {
        return HandleTest(request, cancellationToken);
    }

    public Exception HandleExceptionTest(Exception ex, string operation, string? context = null) {
        return HandleException(ex, operation, context);
    }
}

#endregion
