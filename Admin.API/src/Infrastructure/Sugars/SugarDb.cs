using SqlSugar;
using Infrastructure.Shared.Options;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Infrastructure.Sugars;

/// <summary>
/// 数据库上下文，提供 SqlSugar 客户端并管理数据库连接
/// </summary>
public class SugarDb {
    private readonly ILogger<SugarDb>? _logger;
    private readonly Stopwatch? _stopwatch;
    private readonly DatabaseOption _options;

    /// <summary>
    /// 初始化数据库上下文
    /// </summary>
    /// <param name="options">数据库配置选项</param>
    /// <param name="logger">日志记录器</param>
    public SugarDb(DatabaseOption options, ILogger<SugarDb>? logger = null) {
        _options = options;
        _logger = logger;

        if (_options.EnableLog || _options.EnablePerformanceAnalyze) {
            _stopwatch = new Stopwatch();
        }
    }

    /// <summary>
    /// 获取 SqlSugar 客户端实例
    /// </summary>
    /// <returns>SqlSugar 客户端</returns>
    public ISqlSugarClient GetClient() {
        var config = BuildConnectionConfig();
        var db = new SqlSugarScope(config, db => {
            ConfigureAop(db);
            ConfigureGlobalFilters(db);
        });
        return db;
    }

    private ConnectionConfig BuildConnectionConfig() {
        var config = new ConnectionConfig {
            ConnectionString = _options.ConnectionString,
            DbType = _options.DbTypeEnum,
            IsAutoCloseConnection = _options.IsAutoCloseConnection,
            ConfigureExternalServices = new ConfigureExternalServices {
                EntityService = (property, column) => {
                    if (column.IsPrimarykey && property.PropertyType == typeof(Guid)) {
                        column.DataType = "uniqueidentifier";
                    }
                }
            }
        };

        if (_options.EnableReadWriteSplit && _options.SlaveConnectionStrings != null && _options.SlaveConnectionStrings.Count > 0) {
            config.SlaveConnectionConfigs = _options.SlaveConnectionStrings.Select(s => new SlaveConnectionConfig {
                ConnectionString = s.ConnectionString,
                HitRate = s.HitRate
            }).ToList();
        }

        return config;
    }

    private void ConfigureAop(ISqlSugarClient db) {
        db.Aop.OnLogExecuting = (sql, pars) => {
            _stopwatch?.Restart();
            if (_options.EnableLog && _logger != null) {
                _logger.LogDebug("SQL 执行 | {Sql}", sql);
            }
        };

        db.Aop.OnLogExecuted = (sql, pars) => {
            _stopwatch?.Stop();
            if (_options.EnablePerformanceAnalyze && _stopwatch?.ElapsedMilliseconds > _options.PerformanceAnalyzeThreshold) {
                _logger?.LogWarning("慢查询 | {Elapsed}ms | {Sql}", _stopwatch.ElapsedMilliseconds, sql);
            }
        };

        db.Aop.OnError = e => {
            _logger?.LogError(e, "SQL 错误 | {Sql}", e.Sql);
        };
    }

    private static void ConfigureGlobalFilters(ISqlSugarClient db) {
        _ = db.QueryFilter.Add(new TableFilterItem<Domain.Shared.Entities.AggregateBase>(it => it.IsDeleted == false));
    }
}
