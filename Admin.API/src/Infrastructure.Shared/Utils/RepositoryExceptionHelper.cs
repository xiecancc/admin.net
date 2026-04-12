/*
 * 文件名称：RepositoryExceptionHelper.cs
 * 功能描述：仓储异常处理辅助类，提供 SqlSugar 异常识别和转换功能
 * 作者信息：谢灿软件 <492384481@qq.com>
 * 最近修订：2026-04-11
 */

using System.Text.RegularExpressions;
using System.Transactions;

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 仓储异常处理辅助类
/// <para>提供 SqlSugar 异常识别和转换功能，将数据库异常转换为业务异常</para>
/// </summary>
/// <remarks>
/// <para>支持的异常类型：</para>
/// <list type="bullet">
///   <item>唯一键冲突异常 → InvalidOperationException</item>
///   <item>外键约束异常 → InvalidOperationException</item>
///   <item>数据库连接异常 → InvalidOperationException（保留原始异常）</item>
///   <item>事务异常 → InvalidOperationException</item>
///   <item>超时异常 → InvalidOperationException</item>
/// </list>
/// <para>支持的数据库：</para>
/// <list type="bullet">
///   <item>SQL Server</item>
///   <item>MySQL</item>
///   <item>PostgreSQL</item>
///   <item>SQLite</item>
/// </list>
/// </remarks>
public static partial class RepositoryExceptionHelper {
    private const string UniqueKeyViolationMessage = "唯一键冲突";
    private const string ForeignKeyViolationMessage = "外键约束冲突";
    private const string ConnectionErrorMessage = "数据库连接失败";
    private const string TimeoutMessage = "操作超时";
    private const string TransactionMessage = "事务操作失败";

    [GeneratedRegex(@"'([^']+)'", RegexOptions.Compiled)]
    private static partial Regex SingleQuoteRegex();

    [GeneratedRegex(@"\""([^\""]+)\""", RegexOptions.Compiled)]
    private static partial Regex DoubleQuoteRegex();

    [GeneratedRegex(@"constraint\s+[""']?(\w+)[""']?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ConstraintRegex();

    [GeneratedRegex(@"key\s+[""']?(\w+)[""']?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex KeyRegex();

    [GeneratedRegex(@"table\s+[""']?(\w+)[""']?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex TableRegex();

    [GeneratedRegex(@"column\s+[""']?(\w+)[""']?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ColumnRegex();

    [GeneratedRegex(@"Duplicate entry\s+['""]([^'""]+)['""]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MySqlDuplicateEntryRegex();

    [GeneratedRegex(@"for key\s+['""]?([^'""]\S+)['""]?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MySqlKeyRegex();

    [GeneratedRegex(@"Key\s+\(([^)]+)\)=\(([^)]+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PostgresKeyDetailRegex();

    [GeneratedRegex(@"relation\s+[""']?(\w+)[""']?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PostgresRelationRegex();

    /// <summary>
    /// 处理仓储操作异常
    /// </summary>
    /// <param name="exception">原始异常</param>
    /// <param name="operation">操作名称（如：插入、更新、删除）</param>
    /// <param name="entityName">实体名称</param>
    /// <returns>转换后的业务异常</returns>
    /// <example>
    /// <code>
    /// try {
    ///     await _client.Insertable(entity).ExecuteCommandAsync();
    /// }
    /// catch (Exception ex) {
    ///     throw RepositoryExceptionHelper.HandleException(ex, "插入", "用户");
    /// }
    /// </code>
    /// </example>
    public static InvalidOperationException HandleException(Exception exception, string operation, string entityName) {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityName);

        var (message, innerException) = AnalyzeException(exception, operation, entityName);
        return new InvalidOperationException(message, innerException);
    }

    /// <summary>
    /// 处理仓储操作异常并记录日志
    /// </summary>
    /// <param name="exception">原始异常</param>
    /// <param name="operation">操作名称</param>
    /// <param name="entityName">实体名称</param>
    /// <param name="logger">日志记录器</param>
    /// <returns>转换后的业务异常</returns>
    public static InvalidOperationException HandleExceptionWithLog(
        Exception exception,
        string operation,
        string entityName,
        Action<string, object?[]>? logger) {
        var result = HandleException(exception, operation, entityName);
        logger?.Invoke($"{operation}实体失败: {entityName}", [entityName, exception.Message]);
        return result;
    }

    private static (string Message, Exception? InnerException) AnalyzeException(
        Exception exception,
        string operation,
        string entityName) {
        return exception switch {
            TimeoutException => ($"{operation}{entityName}失败：{TimeoutMessage}，请稍后重试", exception),
            TransactionException => ($"{operation}{entityName}失败：{TransactionMessage}", exception),
            _ when IsUniqueKeyViolation(exception, out var fieldName) =>
                ($"{operation}{entityName}失败：{UniqueKeyViolationMessage}，字段 [{fieldName}] 的值已存在", null),
            _ when IsForeignKeyViolation(exception, out var constraintInfo) =>
                ($"{operation}{entityName}失败：{ForeignKeyViolationMessage}，{constraintInfo}", null),
            _ when IsConnectionError(exception) =>
                ($"{operation}{entityName}失败：{ConnectionErrorMessage}，请检查数据库连接", exception),
            _ => ($"{operation}{entityName}失败：{exception.Message}", exception)
        };
    }

    /// <summary>
    /// 判断是否为唯一键冲突异常
    /// </summary>
    /// <param name="exception">异常对象</param>
    /// <param name="fieldName">冲突的字段名称</param>
    /// <returns>如果是唯一键冲突返回 true，否则返回 false</returns>
    public static bool IsUniqueKeyViolation(Exception exception, out string fieldName) {
        fieldName = string.Empty;

        var message = exception.Message;
        var errorCode = GetErrorCode(exception);

        if (IsSqlServerUniqueKeyViolation(errorCode, message, ref fieldName) ||
            IsMySqlUniqueKeyViolation(errorCode, message, ref fieldName) ||
            IsPostgreSqlUniqueKeyViolation(errorCode, message, ref fieldName) ||
            IsSqliteUniqueKeyViolation(message, ref fieldName)) {
            return true;
        }

        if (message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("DUPLICATE", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("重复", StringComparison.OrdinalIgnoreCase)) {
            fieldName = ExtractFieldName(message);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断是否为外键约束异常
    /// </summary>
    /// <param name="exception">异常对象</param>
    /// <param name="constraintInfo">约束信息</param>
    /// <returns>如果是外键约束异常返回 true，否则返回 false</returns>
    public static bool IsForeignKeyViolation(Exception exception, out string constraintInfo) {
        constraintInfo = string.Empty;

        var message = exception.Message;
        var errorCode = GetErrorCode(exception);

        if (IsSqlServerForeignKeyViolation(errorCode, message, ref constraintInfo) ||
            IsMySqlForeignKeyViolation(errorCode, message, ref constraintInfo) ||
            IsPostgreSqlForeignKeyViolation(errorCode, message, ref constraintInfo)) {
            return true;
        }

        if (message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("外键", StringComparison.OrdinalIgnoreCase)) {
            constraintInfo = ExtractConstraintInfo(message);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断是否为数据库连接异常
    /// </summary>
    /// <param name="exception">异常对象</param>
    /// <returns>如果是连接异常返回 true，否则返回 false</returns>
    public static bool IsConnectionError(Exception exception) {
        var message = exception.Message.ToUpperInvariant();
        var innerMessage = exception.InnerException?.Message.ToUpperInvariant();

        return message.Contains("CONNECTION") ||
               message.Contains("CONNECT") ||
               message.Contains("TIMEOUT") ||
               message.Contains("NETWORK") ||
               message.Contains("UNREACHABLE") ||
               innerMessage?.Contains("CONNECTION") == true ||
               innerMessage?.Contains("CONNECT") == true ||
               exception is TimeoutException;
    }

    private static int GetErrorCode(Exception exception) {
        var exType = exception.GetType();

        var numberProperty = exType.GetProperty("Number");
        if (numberProperty != null) {
            var number = numberProperty.GetValue(exception);
            if (number is int errorCode) {
                return errorCode;
            }
        }

        var errorCodeProperty = exType.GetProperty("ErrorCode");
        if (errorCodeProperty != null) {
            var code = errorCodeProperty.GetValue(exception);
            if (code is int intCode) {
                return intCode;
            }
            if (code is uint uintCode) {
                return (int)uintCode;
            }
        }

        var sqlStateProperty = exType.GetProperty("SqlState");
        if (sqlStateProperty != null) {
            var sqlState = sqlStateProperty.GetValue(exception);
            if (sqlState is string state && state.StartsWith("23", StringComparison.Ordinal)) {
                return 2601;
            }
        }

        return 0;
    }

    private static bool IsSqlServerUniqueKeyViolation(int errorCode, string message, ref string fieldName) {
        if (errorCode is not (2601 or 2627)) {
            return false;
        }

        fieldName = ExtractFieldName(message);
        return true;
    }

    private static bool IsMySqlUniqueKeyViolation(int errorCode, string message, ref string fieldName) {
        if (errorCode is not (1062 or 1022 or 1169)) {
            return false;
        }

        var duplicateMatch = MySqlDuplicateEntryRegex().Match(message);
        if (duplicateMatch.Success) {
            fieldName = duplicateMatch.Groups[1].Value;
            return true;
        }

        var keyMatch = MySqlKeyRegex().Match(message);
        if (keyMatch.Success) {
            fieldName = keyMatch.Groups[1].Value;
            return true;
        }

        fieldName = ExtractFieldName(message);
        return true;
    }

    private static bool IsPostgreSqlUniqueKeyViolation(int errorCode, string message, ref string fieldName) {
        if (errorCode != 23505 && !message.Contains("23505", StringComparison.Ordinal)) {
            return false;
        }

        var keyDetailMatch = PostgresKeyDetailRegex().Match(message);
        if (keyDetailMatch.Success) {
            fieldName = keyDetailMatch.Groups[1].Value;
            return true;
        }

        fieldName = ExtractFieldName(message);
        return true;
    }

    private static bool IsSqliteUniqueKeyViolation(string message, ref string fieldName) {
        if (!message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)) {
            return false;
        }

        fieldName = ExtractFieldName(message);
        return true;
    }

    private static bool IsSqlServerForeignKeyViolation(int errorCode, string message, ref string constraintInfo) {
        if (errorCode is not (547 or 546)) {
            return false;
        }

        constraintInfo = ExtractConstraintInfo(message);
        return true;
    }

    private static bool IsMySqlForeignKeyViolation(int errorCode, string message, ref string constraintInfo) {
        if (errorCode is not (1451 or 1452 or 1217 or 1216)) {
            return false;
        }

        constraintInfo = ExtractConstraintInfo(message);
        return true;
    }

    private static bool IsPostgreSqlForeignKeyViolation(int errorCode, string message, ref string constraintInfo) {
        if (errorCode != 23503 && !message.Contains("23503", StringComparison.Ordinal)) {
            return false;
        }

        constraintInfo = ExtractConstraintInfo(message);
        return true;
    }

    private static string ExtractFieldName(string message) {
        var constraintMatch = ConstraintRegex().Match(message);
        if (constraintMatch.Success) {
            return constraintMatch.Groups[1].Value;
        }

        var keyMatch = KeyRegex().Match(message);
        if (keyMatch.Success) {
            return keyMatch.Groups[1].Value;
        }

        var singleQuoteMatch = SingleQuoteRegex().Match(message);
        if (singleQuoteMatch.Success) {
            return singleQuoteMatch.Groups[1].Value;
        }

        var doubleQuoteMatch = DoubleQuoteRegex().Match(message);
        if (doubleQuoteMatch.Success) {
            return doubleQuoteMatch.Groups[1].Value;
        }

        return "未知字段";
    }

    private static string ExtractConstraintInfo(string message) {
        var tableMatch = TableRegex().Match(message);
        var columnMatch = ColumnRegex().Match(message);
        var constraintMatch = ConstraintRegex().Match(message);
        var relationMatch = PostgresRelationRegex().Match(message);

        var parts = new List<string>();

        if (constraintMatch.Success) {
            parts.Add($"约束 [{constraintMatch.Groups[1].Value}]");
        }

        if (tableMatch.Success) {
            parts.Add($"表 [{tableMatch.Groups[1].Value}]");
        }

        if (relationMatch.Success) {
            parts.Add($"关联表 [{relationMatch.Groups[1].Value}]");
        }

        if (columnMatch.Success) {
            parts.Add($"列 [{columnMatch.Groups[1].Value}]");
        }

        return parts.Count > 0 ? string.Join("，", parts) : "存在关联数据";
    }
}
