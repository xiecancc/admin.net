/*
 * 文件名称: QueryHandlerExceptionTests.cs
 * 功能描述: 查询处理器异常处理测试类
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Application.Abstractions.Queries;
using Application.Contracts.Abstractions.Queries;
using Application.Contracts.Dtos;
using AutoMapper;
using Domain.Shared.Entities;
using Domain.Shared.Repositories;
using Infrastructure.Shared.Caches;
using Infrastructure.Shared.Units;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Test.Handlers;

/// <summary>
/// 查询处理器异常处理测试类
/// <para>测试查询处理器的异常处理逻辑</para>
/// </summary>
public class QueryHandlerExceptionTests {
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICacheProvider> _mockCacheProvider;
    private readonly Mock<ILogger<TestQueryHandler>> _mockLogger;

    public QueryHandlerExceptionTests() {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockCacheProvider = new Mock<ICacheProvider>();
        _mockLogger = new Mock<ILogger<TestQueryHandler>>();
    }

    #region GetByIdQuery Tests

    /// <summary>
    /// 测试根据 ID 查询 - null 请求
    /// <para>场景：传入 null 查询请求</para>
    /// <para>预期：抛出 ArgumentNullException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithNullRequest_ShouldThrowArgumentNullException() {
        var handler = CreateHandler();

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleTest(null!));
    }

    /// <summary>
    /// 测试根据 ID 查询 - 空 ID
    /// <para>场景：传入空的 ID</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyId_ShouldThrowArgumentException() {
        var handler = CreateHandler();
        var query = new TestByIdQuery(new TestQueryDto()) { Id = Guid.Empty };

        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleTest(query));
    }

    /// <summary>
    /// 测试根据 ID 查询 - 实体不存在
    /// <para>场景：查询不存在的实体</para>
    /// <para>预期：抛出 KeyNotFoundException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithNonExistentEntity_ShouldThrowKeyNotFoundException() {
        var query = new TestByIdQuery(new TestQueryDto()) { Id = Guid.NewGuid() };

        var mockRepository = new Mock<IAggregateRepository<TestAggregate>>();
        mockRepository.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestAggregate?)null);

        _mockUnitOfWork.Setup(x => x.GetRepository<IAggregateRepository<TestAggregate>, TestAggregate>())
            .Returns(mockRepository.Object);

        _mockCacheProvider.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TestDetailDto?>>>(), It.IsAny<TimeSpan?>()))
            .Returns(async (string _, Func<Task<TestDetailDto?>> factory, TimeSpan? _) => await factory());

        var handler = CreateHandler();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.HandleTest(query));
    }

    /// <summary>
    /// 测试根据 ID 查询 - 成功
    /// <para>场景：查询存在的实体</para>
    /// <para>预期：返回正确的 DTO</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithExistingEntity_ShouldReturnDto() {
        var entityId = Guid.NewGuid();
        var query = new TestByIdQuery(new TestQueryDto()) { Id = entityId };
        var entity = new TestAggregate { Id = entityId };
        var expectedDto = new TestDetailDto { Id = entityId };

        var mockRepository = new Mock<IAggregateRepository<TestAggregate>>();
        mockRepository.Setup(x => x.GetAsync(entityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mockUnitOfWork.Setup(x => x.GetRepository<IAggregateRepository<TestAggregate>, TestAggregate>())
            .Returns(mockRepository.Object);

        _mockMapper.Setup(x => x.Map<TestDetailDto>(entity))
            .Returns(expectedDto);

        _mockCacheProvider.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TestDetailDto?>>>(), It.IsAny<TimeSpan?>()))
            .Returns(async (string _, Func<Task<TestDetailDto?>> factory, TimeSpan? _) => await factory());

        var handler = CreateHandler();

        var result = await handler.HandleTest(query);

        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
    }

    /// <summary>
    /// 测试根据 ID 查询 - 操作被取消
    /// <para>场景：操作被取消</para>
    /// <para>预期：抛出 OperationCanceledException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WhenOperationCancelled_ShouldThrowOperationCanceledException() {
        var query = new TestByIdQuery(new TestQueryDto()) { Id = Guid.NewGuid() };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockRepository = new Mock<IAggregateRepository<TestAggregate>>();
        mockRepository.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        _mockUnitOfWork.Setup(x => x.GetRepository<IAggregateRepository<TestAggregate>, TestAggregate>())
            .Returns(mockRepository.Object);

        _mockCacheProvider.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TestDetailDto?>>>(), It.IsAny<TimeSpan?>()))
            .Returns(async (string _, Func<Task<TestDetailDto?>> factory, TimeSpan? _) => await factory());

        var handler = CreateHandler();

        await Assert.ThrowsAsync<OperationCanceledException>(() => handler.HandleTest(query, cts.Token));
    }

    /// <summary>
    /// 测试根据 ID 查询 - 缓存命中
    /// <para>场景：缓存中存在数据</para>
    /// <para>预期：不调用仓储</para>
    /// </summary>
    [Fact]
    public async Task Handle_WithCacheHit_ShouldNotCallRepository() {
        var entityId = Guid.NewGuid();
        var query = new TestByIdQuery(new TestQueryDto()) { Id = entityId };
        var cachedDto = new TestDetailDto { Id = entityId };

        var mockRepository = new Mock<IAggregateRepository<TestAggregate>>();

        _mockUnitOfWork.Setup(x => x.GetRepository<IAggregateRepository<TestAggregate>, TestAggregate>())
            .Returns(mockRepository.Object);

        _mockCacheProvider.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TestDetailDto?>>>(), It.IsAny<TimeSpan?>()))
            .ReturnsAsync(cachedDto);

        var handler = CreateHandler();

        var result = await handler.HandleTest(query);

        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
        mockRepository.Verify(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// 测试根据 ID 查询 - 仓储异常
    /// <para>场景：仓储抛出异常</para>
    /// <para>预期：抛出 InvalidOperationException</para>
    /// </summary>
    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldThrowInvalidOperationException() {
        var query = new TestByIdQuery(new TestQueryDto()) { Id = Guid.NewGuid() };

        var mockRepository = new Mock<IAggregateRepository<TestAggregate>>();
        mockRepository.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        _mockUnitOfWork.Setup(x => x.GetRepository<IAggregateRepository<TestAggregate>, TestAggregate>())
            .Returns(mockRepository.Object);

        _mockCacheProvider.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TestDetailDto?>>>(), It.IsAny<TimeSpan?>()))
            .Returns(async (string _, Func<Task<TestDetailDto?>> factory, TimeSpan? _) => await factory());

        var handler = CreateHandler();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleTest(query));

        Assert.Contains("查询 TestAggregate 详情时发生异常", exception.Message);
    }

    #endregion

    #region Cache Key Tests

    /// <summary>
    /// 测试缓存键生成
    /// <para>场景：生成缓存键</para>
    /// <para>预期：缓存键格式正确</para>
    /// </summary>
    [Fact]
    public async Task Handle_ShouldGenerateCorrectCacheKey() {
        var entityId = Guid.NewGuid();
        var query = new TestByIdQuery(new TestQueryDto()) { Id = entityId };
        string? capturedCacheKey = null;

        var mockRepository = new Mock<IAggregateRepository<TestAggregate>>();
        mockRepository.Setup(x => x.GetAsync(entityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestAggregate { Id = entityId });

        _mockUnitOfWork.Setup(x => x.GetRepository<IAggregateRepository<TestAggregate>, TestAggregate>())
            .Returns(mockRepository.Object);

        _mockMapper.Setup(x => x.Map<TestDetailDto>(It.IsAny<TestAggregate>()))
            .Returns(new TestDetailDto { Id = entityId });

        _mockCacheProvider.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<TestDetailDto?>>>(), It.IsAny<TimeSpan?>()))
            .Callback<string, Func<Task<TestDetailDto?>>, TimeSpan?>((key, _, _) => capturedCacheKey = key)
            .ReturnsAsync(new TestDetailDto { Id = entityId });

        var handler = CreateHandler();

        await handler.HandleTest(query);

        Assert.NotNull(capturedCacheKey);
        Assert.Contains("testaggregate:detail:", capturedCacheKey);
        Assert.Contains(entityId.ToString(), capturedCacheKey);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 创建测试查询处理器
    /// </summary>
    private TestQueryHandler CreateHandler() {
        return new TestQueryHandler(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockCacheProvider.Object,
            _mockLogger.Object
        );
    }

    #endregion
}

#region Test Helpers

/// <summary>
/// 测试聚合根
/// </summary>
internal class TestAggregate : AggregateBase;

/// <summary>
/// 测试详情 DTO
/// </summary>
internal class TestDetailDto : AggregateDetailDto;

/// <summary>
/// 测试查询参数 DTO
/// </summary>
internal class TestQueryDto : AggregateQueryDto;

/// <summary>
/// 测试根据 ID 查询
/// </summary>
internal class TestByIdQuery(TestQueryDto QueryDto) : AggregateByIdQuery<TestQueryDto, TestDetailDto>(QueryDto);

/// <summary>
/// 测试查询处理器
/// </summary>
internal class TestQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICacheProvider cacheProvider,
    ILogger<TestQueryHandler> logger) : AggregateByIdQueryHandler<TestAggregate, IAggregateRepository<TestAggregate>, TestByIdQuery, TestQueryDto, TestDetailDto>(unitOfWork, mapper, cacheProvider, logger) {

    public async Task<TestDetailDto> HandleTest(TestByIdQuery request, CancellationToken cancellationToken = default) {
        return await Handle(request, cancellationToken);
    }
}

#endregion
