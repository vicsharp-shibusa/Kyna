using Microsoft.Extensions.Logging;

namespace Kyna.Common.Logging;

public static class KLogger
{
    private static ILogger? _logger = null;

    public static void SetLogger(ILogger? logger)
    {
        _logger = logger;
    }

    public static void LogEvent(int eventId, string? eventName, string? context = null, Guid? processId = null) =>
        Log(new EventId(eventId, eventName), context, processId);

    public static void LogEvent(EventId eventId, string? context = null, Guid? processId = null) =>
        Log(eventId, context, processId);

    public static void LogTrace(string message, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(LogLevel.Trace, message, scope, context, processId);

    public static void LogDebug(string message, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(LogLevel.Debug, message, scope, context, processId);

    public static void LogInformation(string message, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(LogLevel.Information, message, scope, context, processId);

    public static void LogWarning(string message, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(LogLevel.Warning, message, scope, context, processId);

    public static void LogError(string message, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(LogLevel.Error, message, scope, context, processId);

    public static void LogError(Exception exception, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(exception, LogLevel.Error, scope, context, processId);

    public static void LogCritical(string message, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(LogLevel.Critical, message, scope, context, processId);

    public static void LogCritical(Exception exception, string? scope = null, string? context = null, Guid? processId = null) =>
        Log(exception, LogLevel.Critical, scope, context, processId);

    public static void Log(LogLevel logLevel, string message,
        string? scope = null, string? context = null, Guid? processId = null)
    {
        if (_logger is not null && _logger.IsEnabled(logLevel))
        {
            _logger.Log(logLevel, default, CreateLogItem(message, logLevel, scope, context, processId), null, FormatMessage);
        }
    }

    private static void Log(Exception exception, LogLevel logLevel = LogLevel.Critical,
        string? scope = null, string? context = null, Guid? processId = null)
    {
        if (_logger is not null && _logger.IsEnabled(logLevel))
        {
            _logger.Log(logLevel, default, CreateLogItem(exception, logLevel, scope, context, processId), null, FormatMessage);
        }
    }

    private static void Log(EventId eventId, string? context = null, Guid? processId = null)
    {
        _logger?.Log(LogLevel.Information, eventId, CreateLogItem(eventId.Id, eventId.Name,
                context, processId), null, FormatMessage);
    }

    private static string FormatMessage(LogItem logItem, Exception? exc) => logItem.ToString();

    private static LogItem CreateLogItem(string? message, LogLevel logLevel,
        string? scope = null, string? context = null, Guid? processId = null) =>
            new(message, logLevel, scope, context, processId);

    private static LogItem CreateLogItem(Exception exception, LogLevel logLevel = LogLevel.Critical,
        string? scope = null, string? context = null, Guid? processId = null) =>
            new(exception, scope, context, processId, logLevel);

    private static LogItem CreateLogItem(int eventId, string? eventName = null,
        string? context = null, Guid? processId = null) =>
            new(eventId, eventName, context, processId);

}
