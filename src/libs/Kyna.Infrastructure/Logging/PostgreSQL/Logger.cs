using Kyna.Logging;
using Microsoft.Extensions.Logging;

namespace Kyna.Infrastructure.Logging.PostgreSQL;

internal sealed class Logger : ILogger
{
    private LogScope? scope = null;
    private readonly Func<string, LogLevel, bool>? filter;
    private readonly string categoryName;
    private readonly LoggerProvider loggerProvider;

    public Logger(LoggerProvider loggerProvider,
        string categoryName,
        Func<string, LogLevel, bool>? filter = null)
    {
        this.loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        this.categoryName = string.IsNullOrWhiteSpace(categoryName) ? throw new ArgumentNullException(nameof(categoryName)) : categoryName;
        this.filter = filter;
    }

    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        scope = new LogScope(state ?? new object());
        return scope;
    }

    public bool IsEnabled(LogLevel logLevel) => filter == null || filter(categoryName, logLevel);

    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState? state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) { return; }

        if (state is null && exception is null)
        {
            throw new ArgumentException($"{nameof(state)} and {nameof(exception)} can not both be null.");
        }

        LogItem? logItem = state is not null && state is LogItem ? state as LogItem : null;
        EventId evId = logItem is not null && !logItem.EventId.Equals(default) ? logItem.EventId : eventId;

        Sql.DataAccessObjects.Log? logDao;
        Sql.DataAccessObjects.EventLog? eventLogDao = null;

        // TODO: not sure this logic is best; I think this writes logs when we may only want to write event logs.
        if (logItem is null)
        {
            logDao = new()
            {
                LogLevel = logLevel.ToString(),
                Message = exception?.Message ?? state?.ToString() ?? "None",
                Exception = exception?.ToString(),
                Scope = scope?.ScopeMessage
            };
        }
        else
        {
            logDao = new()
            {
                Message = logItem.Message,
                Exception = logItem.Exception?.ToString(),
                LogLevel = logItem.LogLevel.ToString(),
                Scope = logItem.Scope ?? scope?.ScopeMessage,
                ProcessId = logItem.ProcessId,
                Context = logItem.Context
            };
        }

        if (!evId.Equals(default))
        {
            eventLogDao = new()
            {
                EventId = evId.Id,
                EventName = evId.Name,
                ProcessId = logItem?.ProcessId,
                Context = logItem?.Context
            };
        }

        if (logDao is not null)
        {
            loggerProvider.PreserveLog(logDao);
        }

        if (eventLogDao is not null)
        {
            loggerProvider.PreserveLog(eventLogDao);
        }
    }

    private class LogScope : IDisposable
    {
        private bool disposed = false;

        public LogScope(object? state)
        {
            ScopeMessage = state?.ToString();
        }

        public string? ScopeMessage { get; protected set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ScopeMessage = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}