/*
 * 文件名称: RepositoryExceptionHelperTests.cs
 * 功能描述: 仓储异常处理辅助类测试
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-11
 */

using Infrastructure.Shared.Utils;
using System.Transactions;

namespace Infrastructure.Test.Utils;

/// <summary>
/// 仓储异常处理辅助类测试
/// <para>测试异常识别和转换功能</para>
/// </summary>
public class RepositoryExceptionHelperTests {
    #region HandleException Tests

    /// <summary>
    /// 测试处理异常 - null 异常
    /// <para>场景：传入 null 异常</para>
    /// <para>预期：抛出 ArgumentNullException</para>
    /// </summary>
    [Fact]
    public void HandleException_WithNullException_ShouldThrowArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => RepositoryExceptionHelper.HandleException(null!, "插入", "用户"));
    }

    /// <summary>
    /// 测试处理异常 - 空操作名称
    /// <para>场景：传入空的操作名称</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void HandleException_WithEmptyOperation_ShouldThrowArgumentException(string operation) {
        var exception = new Exception("Test exception");

        Assert.Throws<ArgumentException>(() => RepositoryExceptionHelper.HandleException(exception, operation, "用户"));
    }

    /// <summary>
    /// 测试处理异常 - 空实体名称
    /// <para>场景：传入空的实体名称</para>
    /// <para>预期：抛出 ArgumentException</para>
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void HandleException_WithEmptyEntityName_ShouldThrowArgumentException(string entityName) {
        var exception = new Exception("Test exception");

        Assert.Throws<ArgumentException>(() => RepositoryExceptionHelper.HandleException(exception, "插入", entityName));
    }

    #endregion

    #region TimeoutException Tests

    /// <summary>
    /// 测试处理超时异常
    /// <para>场景：传入 TimeoutException</para>
    /// <para>预期：返回包含超时信息的 InvalidOperationException</para>
    /// </summary>
    [Fact]
    public void HandleException_WithTimeoutException_ShouldReturnTimeoutMessage() {
        var exception = new TimeoutException("Operation timed out");

        var result = RepositoryExceptionHelper.HandleException(exception, "查询", "用户");

        Assert.IsType<InvalidOperationException>(result);
        Assert.Contains("超时", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    #endregion

    #region TransactionException Tests

    /// <summary>
    /// 测试处理事务异常
    /// <para>场景：传入 TransactionException</para>
    /// <para>预期：返回包含事务失败信息的 InvalidOperationException</para>
    /// </summary>
    [Fact]
    public void HandleException_WithTransactionException_ShouldReturnTransactionMessage() {
        var exception = new TransactionException("Transaction failed");

        var result = RepositoryExceptionHelper.HandleException(exception, "更新", "角色");

        Assert.IsType<InvalidOperationException>(result);
        Assert.Contains("事务操作失败", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    #endregion

    #region UniqueKeyViolation Tests

    /// <summary>
    /// 测试唯一键冲突检测 - SQL Server 错误码 2601
    /// <para>场景：SQL Server 唯一键冲突</para>
    /// <para>预期：检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithSqlServerErrorCode2601_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(2601, "Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_Email'");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.True(result);
        Assert.NotEmpty(fieldName);
    }

    /// <summary>
    /// 测试唯一键冲突检测 - SQL Server 错误码 2627
    /// <para>场景：SQL Server 主键冲突</para>
    /// <para>预期：检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithSqlServerErrorCode2627_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(2627, "Violation of PRIMARY KEY constraint 'PK_Users'");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.True(result);
    }

    /// <summary>
    /// 测试唯一键冲突检测 - MySQL 错误码 1062
    /// <para>场景：MySQL 唯一键冲突</para>
    /// <para>预期：检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithMySqlErrorCode1062_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(1062, "Duplicate entry 'test@example.com' for key 'IX_Users_Email'");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.True(result);
        Assert.Equal("test@example.com", fieldName);
    }

    /// <summary>
    /// 测试唯一键冲突检测 - PostgreSQL 错误码 23505
    /// <para>场景：PostgreSQL 唯一键冲突</para>
    /// <para>预期：检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithPostgreSqlErrorCode23505_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(23505, "duplicate key value violates unique constraint \"IX_Users_Email\"");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.True(result);
    }

    /// <summary>
    /// 测试唯一键冲突检测 - SQLite
    /// <para>场景：SQLite 唯一键冲突</para>
    /// <para>预期：检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithSqliteUniqueConstraint_ShouldReturnTrue() {
        var exception = new Exception("UNIQUE constraint failed: Users.Email");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.True(result);
    }

    /// <summary>
    /// 测试唯一键冲突检测 - 消息包含 UNIQUE 关键字
    /// <para>场景：异常消息包含 UNIQUE</para>
    /// <para>预期：检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithUniqueKeywordInMessage_ShouldReturnTrue() {
        var exception = new Exception("UNIQUE constraint violation on column 'Code'");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.True(result);
    }

    /// <summary>
    /// 测试唯一键冲突检测 - 非唯一键冲突
    /// <para>场景：普通异常</para>
    /// <para>预期：未检测到唯一键冲突</para>
    /// </summary>
    [Fact]
    public void IsUniqueKeyViolation_WithNonUniqueKeyException_ShouldReturnFalse() {
        var exception = new Exception("Some other error");

        var result = RepositoryExceptionHelper.IsUniqueKeyViolation(exception, out var fieldName);

        Assert.False(result);
        Assert.Equal(string.Empty, fieldName);
    }

    #endregion

    #region ForeignKeyViolation Tests

    /// <summary>
    /// 测试外键约束冲突检测 - SQL Server 错误码 547
    /// <para>场景：SQL Server 外键约束冲突</para>
    /// <para>预期：检测到外键约束冲突</para>
    /// </summary>
    [Fact]
    public void IsForeignKeyViolation_WithSqlServerErrorCode547_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(547, "The DELETE statement conflicted with the REFERENCE constraint \"FK_UserRoles_Roles\"");

        var result = RepositoryExceptionHelper.IsForeignKeyViolation(exception, out var constraintInfo);

        Assert.True(result);
        Assert.NotEmpty(constraintInfo);
    }

    /// <summary>
    /// 测试外键约束冲突检测 - MySQL 错误码 1451
    /// <para>场景：MySQL 外键约束冲突（删除）</para>
    /// <para>预期：检测到外键约束冲突</para>
    /// </summary>
    [Fact]
    public void IsForeignKeyViolation_WithMySqlErrorCode1451_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(1451, "Cannot delete or update a parent row: a foreign key constraint fails");

        var result = RepositoryExceptionHelper.IsForeignKeyViolation(exception, out var constraintInfo);

        Assert.True(result);
    }

    /// <summary>
    /// 测试外键约束冲突检测 - MySQL 错误码 1452
    /// <para>场景：MySQL 外键约束冲突（插入）</para>
    /// <para>预期：检测到外键约束冲突</para>
    /// </summary>
    [Fact]
    public void IsForeignKeyViolation_WithMySqlErrorCode1452_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(1452, "Cannot add or update a child row: a foreign key constraint fails");

        var result = RepositoryExceptionHelper.IsForeignKeyViolation(exception, out var constraintInfo);

        Assert.True(result);
    }

    /// <summary>
    /// 测试外键约束冲突检测 - PostgreSQL 错误码 23503
    /// <para>场景：PostgreSQL 外键约束冲突</para>
    /// <para>预期：检测到外键约束冲突</para>
    /// </summary>
    [Fact]
    public void IsForeignKeyViolation_WithPostgreSqlErrorCode23503_ShouldReturnTrue() {
        var exception = CreateExceptionWithErrorCode(23503, "update or delete on table \"Roles\" violates foreign key constraint \"FK_UserRoles_Roles\" on table \"UserRoles\"");

        var result = RepositoryExceptionHelper.IsForeignKeyViolation(exception, out var constraintInfo);

        Assert.True(result);
    }

    /// <summary>
    /// 测试外键约束冲突检测 - 消息包含 FOREIGN KEY 关键字
    /// <para>场景：异常消息包含 FOREIGN KEY</para>
    /// <para>预期：检测到外键约束冲突</para>
    /// </summary>
    [Fact]
    public void IsForeignKeyViolation_WithForeignKeyKeywordInMessage_ShouldReturnTrue() {
        var exception = new Exception("FOREIGN KEY constraint violation");

        var result = RepositoryExceptionHelper.IsForeignKeyViolation(exception, out var constraintInfo);

        Assert.True(result);
    }

    /// <summary>
    /// 测试外键约束冲突检测 - 非外键约束冲突
    /// <para>场景：普通异常</para>
    /// <para>预期：未检测到外键约束冲突</para>
    /// </summary>
    [Fact]
    public void IsForeignKeyViolation_WithNonForeignKeyException_ShouldReturnFalse() {
        var exception = new Exception("Some other error");

        var result = RepositoryExceptionHelper.IsForeignKeyViolation(exception, out var constraintInfo);

        Assert.False(result);
        Assert.Equal(string.Empty, constraintInfo);
    }

    #endregion

    #region ConnectionError Tests

    /// <summary>
    /// 测试连接异常检测 - 消息包含 CONNECTION
    /// <para>场景：异常消息包含 CONNECTION</para>
    /// <para>预期：检测到连接异常</para>
    /// </summary>
    [Theory]
    [InlineData("CONNECTION failed")]
    [InlineData("Unable to CONNECT to server")]
    [InlineData("Network TIMEOUT")]
    [InlineData("Host UNREACHABLE")]
    public void IsConnectionError_WithConnectionRelatedMessage_ShouldReturnTrue(string message) {
        var exception = new Exception(message);

        var result = RepositoryExceptionHelper.IsConnectionError(exception);

        Assert.True(result);
    }

    /// <summary>
    /// 测试连接异常检测 - TimeoutException
    /// <para>场景：TimeoutException 类型</para>
    /// <para>预期：检测到连接异常</para>
    /// </summary>
    [Fact]
    public void IsConnectionError_WithTimeoutException_ShouldReturnTrue() {
        var exception = new TimeoutException();

        var result = RepositoryExceptionHelper.IsConnectionError(exception);

        Assert.True(result);
    }

    /// <summary>
    /// 测试连接异常检测 - 内部异常包含连接信息
    /// <para>场景：内部异常消息包含 CONNECTION</para>
    /// <para>预期：检测到连接异常</para>
    /// </summary>
    [Fact]
    public void IsConnectionError_WithInnerExceptionConnectionMessage_ShouldReturnTrue() {
        var innerException = new Exception("CONNECTION failed");
        var exception = new Exception("Outer exception", innerException);

        var result = RepositoryExceptionHelper.IsConnectionError(exception);

        Assert.True(result);
    }

    /// <summary>
    /// 测试连接异常检测 - 非连接异常
    /// <para>场景：普通异常</para>
    /// <para>预期：未检测到连接异常</para>
    /// </summary>
    [Fact]
    public void IsConnectionError_WithNonConnectionException_ShouldReturnFalse() {
        var exception = new Exception("Some other error");

        var result = RepositoryExceptionHelper.IsConnectionError(exception);

        Assert.False(result);
    }

    #endregion

    #region HandleException Integration Tests

    /// <summary>
    /// 测试处理唯一键冲突异常
    /// <para>场景：唯一键冲突异常</para>
    /// <para>预期：返回包含字段名的错误消息</para>
    /// </summary>
    [Fact]
    public void HandleException_WithUniqueKeyViolation_ShouldReturnMessageWithFieldName() {
        var exception = CreateExceptionWithErrorCode(2601, "Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_Email'");

        var result = RepositoryExceptionHelper.HandleException(exception, "插入", "用户");

        Assert.Contains("唯一键冲突", result.Message);
        Assert.Contains("已存在", result.Message);
    }

    /// <summary>
    /// 测试处理外键约束冲突异常
    /// <para>场景：外键约束冲突异常</para>
    /// <para>预期：返回包含约束信息的错误消息</para>
    /// </summary>
    [Fact]
    public void HandleException_WithForeignKeyViolation_ShouldReturnMessageWithConstraintInfo() {
        var exception = CreateExceptionWithErrorCode(547, "The DELETE statement conflicted with the REFERENCE constraint \"FK_UserRoles_Roles\"");

        var result = RepositoryExceptionHelper.HandleException(exception, "删除", "角色");

        Assert.Contains("外键约束冲突", result.Message);
    }

    /// <summary>
    /// 测试处理连接异常
    /// <para>场景：连接异常</para>
    /// <para>预期：返回包含连接失败信息的错误消息</para>
    /// </summary>
    [Fact]
    public void HandleException_WithConnectionError_ShouldReturnMessageWithConnectionInfo() {
        var exception = new Exception("CONNECTION failed");

        var result = RepositoryExceptionHelper.HandleException(exception, "查询", "用户");

        Assert.Contains("数据库连接失败", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    /// <summary>
    /// 测试处理未知异常
    /// <para>场景：未知类型的异常</para>
    /// <para>预期：返回包含原始异常信息的错误消息</para>
    /// </summary>
    [Fact]
    public void HandleException_WithUnknownException_ShouldReturnMessageWithOriginalMessage() {
        var exception = new Exception("Unknown error occurred");

        var result = RepositoryExceptionHelper.HandleException(exception, "操作", "实体");

        Assert.Contains("操作实体失败", result.Message);
        Assert.Contains("Unknown error occurred", result.Message);
        Assert.Equal(exception, result.InnerException);
    }

    #endregion

    #region HandleExceptionWithLog Tests

    /// <summary>
    /// 测试处理异常并记录日志
    /// <para>场景：处理异常并调用日志记录</para>
    /// <para>预期：返回异常并调用日志记录器</para>
    /// </summary>
    [Fact]
    public void HandleExceptionWithLog_ShouldCallLogger() {
        var exception = new Exception("Test error");
        var loggerCalled = false;

        var result = RepositoryExceptionHelper.HandleExceptionWithLog(
            exception,
            "插入",
            "用户",
            (message, args) => loggerCalled = true);

        Assert.True(loggerCalled);
        Assert.IsType<InvalidOperationException>(result);
    }

    /// <summary>
    /// 测试处理异常并记录日志 - null 日志记录器
    /// <para>场景：传入 null 日志记录器</para>
    /// <para>预期：不抛出异常</para>
    /// </summary>
    [Fact]
    public void HandleExceptionWithLog_WithNullLogger_ShouldNotThrow() {
        var exception = new Exception("Test error");

        var result = RepositoryExceptionHelper.HandleExceptionWithLog(
            exception,
            "插入",
            "用户",
            null);

        Assert.IsType<InvalidOperationException>(result);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 创建带有错误码的异常
    /// </summary>
    private static Exception CreateExceptionWithErrorCode(int errorCode, string message) {
        var exceptionType = typeof(Exception);
        var exception = new Exception(message);

        var numberProperty = exceptionType.GetProperty("Number");
        if (numberProperty != null) {
            var derivedType = CreateDerivedExceptionType();
            var derivedException = Activator.CreateInstance(derivedType, message) as Exception;
            numberProperty = derivedType.GetProperty("Number");
            numberProperty?.SetValue(derivedException, errorCode);
            return derivedException ?? exception;
        }

        return exception;
    }

    /// <summary>
    /// 创建派生的异常类型用于测试
    /// </summary>
    private static Type CreateDerivedExceptionType() {
        return typeof(SqlExceptionMock);
    }

    #endregion
}

/// <summary>
/// SQL 异常模拟类，用于测试
/// </summary>
file class SqlExceptionMock : Exception {
    public SqlExceptionMock(string message) : base(message) { }

    public int Number { get; set; }
}
