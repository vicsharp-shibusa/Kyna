namespace Kyna.Infrastructure.Sql.DataAccessObjects;

internal sealed record Log : LogBase
{
    public string? LogLevel { get; init; }
    public string? Message { get; init; }
    public string? Exception { get; init; }
    public string? Scope { get; init; }
}

internal sealed record EventLog : LogBase
{
    public int? EventId { get; init; }
    public string? EventName { get; init; }
}

internal abstract record LogBase
{
    public DateTime UtcTimestamp { get; init; } = DateTime.UtcNow;
    public Guid? ProcessId { get; init; }
    public string? Context { get; init; }
}