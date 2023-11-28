using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Kyna.Infrastructure")]
namespace Kyna.Common.Logging;

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

    public LogItem(Exception exception, string? scope = null, string? context = null, Guid? processId = null,
        LogLevel logLevel = LogLevel.Critical)
        : this(exception.Message, logLevel, scope, context, processId)
    {
        Exception = exception;
    }

    public LogItem(int eventId, string? eventName = null, string? context = null, Guid? processId = null)
        : this(new EventId(eventId, eventName), context, processId) { }

    public LogItem(EventId eventId, string? context = null, Guid? processId = null)
    {
        EventId = eventId;
        Context = context;
        ProcessId = processId;
    }

    public LogLevel LogLevel { get; init; } = LogLevel.Information;
    public EventId EventId { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
    public string? Scope { get; init; }
    public DateTime UtcTimestamp { get; internal init; } = DateTime.UtcNow;
    public Guid? ProcessId { get; init; }
    public string? Context { get; init; }

    public override string ToString() => EventId.Equals(default)
            ? Exception?.Message ?? Message ?? "None"
            : EventId.ToString();
}