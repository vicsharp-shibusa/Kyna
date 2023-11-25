using Kyna.Infrastructure.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kyna.Infrastructure.Tests;

public class SqlBuilderIntegrationTests
{
    private IConfiguration? configuration = null;

    public SqlBuilderIntegrationTests()
    {
        Configure();
    }

    private void Configure()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);

        configuration = builder.Build();
    }

    [Fact]
    public async Task InsertAndFetchLogsAsync()
    {
        var db = new DbContext(configuration?.GetConnectionString("Logs") ?? "");
        var sqlBuilder = new SqlBuilder(DatabaseEngine.PostgreSql);

        Guid processId = Guid.NewGuid();

        await db.ExecuteAsync(sqlBuilder.InsertLog, new Sql.DataAccessObjects.Log()
        {
            Context = nameof(InsertAndFetchLogsAsync),
            Exception = null,
            LogLevel = LogLevel.Debug.ToString(),
            Message = "Testing insertion of log.",
            ProcessId = processId,
            Scope = nameof(SqlBuilderIntegrationTests)
        });

        var querySql = @$"{sqlBuilder.FetchLogs}
WHERE process_id = @ProcessId
ORDER BY utc_timestamp";

        var results = await db.QueryAsync<Sql.DataAccessObjects.Log>(querySql, new { processId });

        Assert.NotNull(results);
        Assert.NotEmpty(results);

    }

    [Fact]
    public async Task InsertAndFetchLogEventsAsync()
    {
        var db = new DbContext(configuration?.GetConnectionString("Logs") ?? "");
        var sqlBuilder = new SqlBuilder(DatabaseEngine.PostgreSql);

        Guid processId = Guid.NewGuid();

        await db.ExecuteAsync(sqlBuilder.InsertLogEvent, new Sql.DataAccessObjects.EventLog()
        {
            Context = nameof(InsertAndFetchLogEventsAsync),
            ProcessId = processId,
            EventId = 1900,
            EventName = "Nineteen hundred"
        });

        var querySql = @$"{sqlBuilder.FetchLogEvents}
WHERE process_id = @ProcessId
ORDER BY utc_timestamp";

        var results = await db.QueryAsync<Sql.DataAccessObjects.Log>(querySql, new { processId });

        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

}
