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

        Sql.DataAccessObjects.Log? logDao = null;
        Sql.DataAccessObjects.EventLog? eventLogDao = null;

        if (state == null)
        {
            if (exception == null) { throw new ArgumentException($"{nameof(state)} and {nameof(exception)} can not both be null."); }

            if (eventId.Equals(default))
            {
                logDao = new()
                {
                    LogLevel = logLevel.ToString(),
                    Message = exception?.Message,
                    Exception = exception?.ToString(),
                    Scope = scope?.ScopeMessage
                };
            }
            else
            {
                eventLogDao = new()
                {
                    EventId = eventId.Id,
                    EventName = eventId.Name
                };
            }
        }
        else
        {
            if (eventId.Equals(default))
            {
                if (state is LogItem)
                {
                    var item = state as LogItem;
                    logDao = new Sql.DataAccessObjects.Log()
                    {
                        Message = item!.Message,
                        Exception = item.Exception?.ToString(),
                        LogLevel = item.LogLevel.ToString(),
                        Scope = item.Scope
                    };
                }
                else
                {
                    if (formatter is null)
                    {
                        logDao = new Sql.DataAccessObjects.Log()
                        {
                            Message = state.ToString(),
                            Exception = exception?.ToString(),
                            LogLevel = logLevel.ToString()
                        };
                    }
                    else
                    {
                        logDao = new Sql.DataAccessObjects.Log()
                        {
                            Message = formatter(state, exception),
                            Exception = exception?.ToString(),
                            LogLevel = logLevel.ToString()
                        };
                    }
                }
            }
            else
            {
                eventLogDao = new()
                {
                    EventId = eventId.Id,
                    EventName = eventId.Name
                };
            }
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