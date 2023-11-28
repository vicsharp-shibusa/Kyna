using Kyna.Infrastructure.Sql;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Kyna.Infrastructure.Logging.PostgreSQL;

internal class LoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<Sql.DataAccessObjects.Log> logQueue;
    private readonly ConcurrentQueue<Sql.DataAccessObjects.EventLog> logEventQueue;

    private bool disposedValue;

    private readonly Func<string, LogLevel, bool>? filter;
    private bool runQueue = true;

    private readonly DbContext dbContext;
    private readonly SqlBuilder sqlBuilder;

    public LoggerProvider(string? connectionString, Func<string, LogLevel, bool>? filter = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }

        dbContext = new DbContext(connectionString);
        sqlBuilder = new SqlBuilder(DatabaseEngine.PostgreSql);

        this.filter = filter;

        logQueue = new ConcurrentQueue<Sql.DataAccessObjects.Log>();
        logEventQueue = new ConcurrentQueue<Sql.DataAccessObjects.EventLog>();

        runQueue = true;
        RunLoggerDequeue();
    }

    public ILogger CreateLogger(string categoryName) => new Logger(this, categoryName, filter);

    internal void PreserveLog(Sql.DataAccessObjects.Log logDao)
    {
        if (logDao is not null)
        {
            logQueue.Enqueue(logDao);
        }
    }

    internal void PreserveLog(Sql.DataAccessObjects.EventLog eventLogDao)
    {
        if (eventLogDao is not null)
        {
            logEventQueue.Enqueue(eventLogDao);
        }
    }

    private void RunLoggerDequeue()
    {
        Task.Run(() =>
        {
            while (runQueue)
            {
                if (logQueue.TryDequeue(out Sql.DataAccessObjects.Log? logItem))
                {
                    if (logItem is not null)
                    {
                        dbContext.Execute(sqlBuilder.InsertLog, logItem);
                    }
                }

                if (logEventQueue.TryDequeue(out Sql.DataAccessObjects.EventLog? logEvent))
                {
                    if (logEvent is not null)
                    {
                        dbContext.Execute(sqlBuilder.InsertLogEvent, logEvent);
                    }
                }
            }
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                int i = 0;
                while (!logQueue.IsEmpty && i++ < 10)
                {
                    Thread.Sleep(200);
                }
                while (!logEventQueue.IsEmpty && i++ < 20)
                {
                    Thread.Sleep(200);
                }

                runQueue = false;
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
