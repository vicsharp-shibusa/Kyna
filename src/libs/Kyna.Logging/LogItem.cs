using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Kyna.Infrastructure")]
namespace Kyna.Logging;

public class LogItem
{
    internal LogItem()
    {
    }

    public LogItem(string? message, LogLevel logLevel = LogLevel.Information,
        string? scope = null, string? context = null, Guid? processId = null) : this()
    {
        LogLevel = logLevel;
        Message = message;
        Scope = scope;
        Context = context;
        ProcessId = processId;
    }

    public LogItem(Exception exception, string? scope = null, string? context = null, Guid? processId = null)
        : this(exception.Message, LogLevel.Critical, scope, context, processId)
    {
        Exception = exception;
    }

    public LogLevel LogLevel { get; init; } = LogLevel.Information;
    public EventId EventId { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
    public string? Scope { get; init; }
    public DateTime UtcTimestamp { get; internal init; } = DateTime.UtcNow;
    public Guid? ProcessId { get; init; }
    public string? Context { get; init; }

    public override string ToString()
    {
        return Exception?.Message ?? Message ?? "Log Item";
    }
}