﻿using Kyna.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

ILogger<Program>? logger = null;
IConfiguration? configuration;

Configure();

Stopwatch timer = Stopwatch.StartNew();

Guid processId = Guid.NewGuid();
string context = "kyna-prototype";
string scope = nameof(Program);

KLogger.LogEvent(100, "Application Started", context, processId);
KLogger.LogTrace("testing Klogger Trace", scope, context, processId);
KLogger.LogDebug("testing Klogger Debug", scope, context, processId);
KLogger.LogInformation("testing Klogger", scope, context, processId);
KLogger.LogWarning("testing Klogger Warning", scope, context, processId);
KLogger.LogError("testing Klogger Error", scope, context, processId);
KLogger.LogCritical("testing Klogger Critical", scope, context, processId);
KLogger.LogCritical(new ArgumentNullException("fake"), scope, context, processId);
KLogger.LogEvent(900, "Application Ended", context, processId);

await Task.Delay(1000);

timer.Stop();

Console.WriteLine(timer.ElapsedMilliseconds);
Console.WriteLine("done");

void Configure()
{
    const string logsKey = "Logs";

    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);

    configuration = builder.Build();

    var logsConnString = configuration.GetConnectionString(logsKey);
    var logEngineSection = configuration.GetSection($"DatabaseEngines:{logsKey}");

    Kyna.Infrastructure.Sql.DatabaseEngine logsEngine = Kyna.Infrastructure.Sql.DatabaseEngine.None;

    if (Enum.TryParse<Kyna.Infrastructure.Sql.DatabaseEngine>(logEngineSection.Value, out var databaseEngine))
    {
        logsEngine = Enum.Parse<Kyna.Infrastructure.Sql.DatabaseEngine>(databaseEngine.ToString());
    }

    if (logsEngine == Kyna.Infrastructure.Sql.DatabaseEngine.PostgreSql)
    {
        logger = Kyna.ApplicationServices.LoggerFactory.CreatePostgreSqlLogger<Program>(configuration.GetConnectionString("Logs"));
    }

    KLogger.SetLogger(logger);
}