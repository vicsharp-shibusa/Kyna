using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Kyna.Infrastructure")]
namespace Kyna.Logging;

internal class LogItem
{
    public LogItem(
        LogLevel logLevel,
        EventId eventId,
        string? message,
        Exception? exception,
        string? scope)
    {
        if (string.IsNullOrWhiteSpace(message) && exception == null && eventId == default)
        {
            throw new ArgumentException($"Either {nameof(message)}, {nameof(exception)}, or {nameof(eventId)} is required.");
        }

        LogLevel = logLevel;
        EventId = eventId;
        Message = message;
        Exception = exception;
        Scope = scope;
        UtcTimestamp = DateTime.UtcNow;
    }

    public LogLevel LogLevel { get; }
    public EventId EventId { get; }
    public string? Message { get; }
    public Exception? Exception { get; }
    public string? Scope { get; }
    public DateTime UtcTimestamp { get; }

    public override string ToString()
    {
        return Exception?.Message ?? Message ?? "Log Item";
    }
}