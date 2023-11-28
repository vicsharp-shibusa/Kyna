namespace Kyna.Infrastructure.Sql;

internal partial class SqlBuilder
{
    public string InsertLog => engine switch
    {
        DatabaseEngine.PostgreSql => @"
INSERT INTO public.logs (utc_timestamp, log_level, message, exception, log_scope, process_id, context)
VALUES (@UtcTimestamp, @LogLevel, @Message, @Exception, @Scope, @ProcessId, @Context)",
        _ => NotImplemented
    };

    public string InsertLogEvent => engine switch
    {
        DatabaseEngine.PostgreSql => @"
INSERT INTO public.log_events (utc_timestamp, event_id, event_name, process_id, context)
VALUES (@UtcTimestamp, @EventId, @EventName, @ProcessId, @Context)",
        _ => NotImplemented
    };

    public string FetchLogs => engine switch
    {
        DatabaseEngine.PostgreSql => @"
SELECT utc_timestamp, log_level, message, exception, log_scope, process_id, context
FROM public.logs",
        _ => NotImplemented
    };

    public string FetchLogEvents => engine switch
    {
        DatabaseEngine.PostgreSql => @"
SELECT utc_timestamp, event_id, event_name, process_id, context
FROM public.log_events",
        _ => NotImplemented
    };
}
