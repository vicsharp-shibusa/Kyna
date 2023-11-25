namespace Kyna.Infrastructure.Sql.DataAccessObjects;

internal record Log
{
    public DateTime UtcTimestamp { get; init; } = DateTime.UtcNow;
    public string? LogLevel { get; init; }
    public string? Message { get; init; }
    public string? Exception { get; init; }
    public string? Scope { get; init; }
    public Guid? ProcessId { get; init; }
    public string? Context { get; init; }
}

internal record EventLog
{
    public DateTime UtcTimestamp { get; init; } = DateTime.UtcNow;
    public int? EventId { get; init; }
    public string? EventName { get; init; }
    public Guid? ProcessId { get; init; }
    public string? Context { get; init; }
}