﻿using Kyna.ApplicationServices.Helpers;
using Kyna.Common;
using Kyna.Common.Abstractions;
using Kyna.Common.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

ILogger<Program>? logger = null;
IConfiguration? configuration;

int exitCode = -1;
string defaultScope = Assembly.GetExecutingAssembly().GetName().Name ?? nameof(Program);
string? defaultContext = defaultScope;
Guid processId = Guid.NewGuid();

Stopwatch timer = Stopwatch.StartNew();

Config? config = null;

try
{
    Configure();

    config = HandleArguments(args);

    Debug.Assert(config is not null);

    if (config.ShowHelp)
    {
        ShowHelp();
        exitCode = 0;
    }
    else
    {
        KLogger.LogEvent(EventIdRepository.GetAppStartEvent(config), defaultContext, processId);
        Communicate($"verbose  = {config.Verbose}", true);
        Communicate($"showHelp = {config.ShowHelp}", true);
        exitCode = 0;
    }

}
catch (ArgumentException exc)
{
#if DEBUG
    Communicate(exc.ToString(), true);
#else
    Communicate(exc.Message, true);
#endif
}
catch (Exception exc)
{
#if DEBUG
    Communicate(exc.ToString(), true);
#else
    Communicate(exc.Message, true);
    KLogger.LogCritical(exc);
#endif

    exitCode = 1;
}
finally
{
    if (!(config?.ShowHelp ?? false))
    {
        KLogger.LogEvent(EventIdRepository.GetAppFinishEvent(config!), defaultContext, processId);
    }

    timer.Stop();

    Communicate($"Completed in {timer.Elapsed.ConvertToText()}");
    Communicate("Waiting ...");
    await Task.Delay(2000);

    Environment.Exit(exitCode);
}

void Communicate(string message, bool force = false, LogLevel logLevel = LogLevel.None,
    string? scope = null, string? context = null)
{
    if (force || (config?.Verbose ?? false))
    {
        Console.WriteLine(message);
    }

    KLogger.Log(logLevel, message, scope ?? defaultScope, context ?? defaultContext, processId);
}

void ShowHelp()
{
    CliArg[] args = CliHelper.GetDefaultArgDescriptions().Union(new CliArg[] { }).ToArray();

    Communicate($"{config.AppName} {config.AppVersion}".Trim(), true);
    Communicate("", true);
    if (!string.IsNullOrWhiteSpace(config.Description))
    {
        Communicate(config.Description, true);
        Communicate("", true);
    }
    Communicate(CliHelper.FormatArguments(args), true);
}

Config HandleArguments(string[] args)
{
    var config = new Config(Assembly.GetExecutingAssembly().GetName().Name ?? nameof(Program), "v1",
        "CLI for testing various things; a throw-away app.");

    args = CliHelper.HydrateDefaultAppConfig(args, config);

    for (int i = 0; i < args.Length; i++)
    {
        string argument = args[i].ToLower();

        switch (argument)
        {
            default:
                throw new Exception($"Unknown argument: {args[i]}");
        }
    }

    return config;
}

void Configure()
{
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: false);

    configuration = builder.Build();

    var logsConnString = configuration.GetConnectionString(CliHelper.Keys.Logs);
    var logEngineSection = configuration.GetSection($"DatabaseEngines:{CliHelper.Keys.Logs}");

    Kyna.Infrastructure.Sql.DatabaseEngine logsEngine = Kyna.Infrastructure.Sql.DatabaseEngine.None;

    if (Enum.TryParse<Kyna.Infrastructure.Sql.DatabaseEngine>(logEngineSection.Value, out var databaseEngine))
    {
        logsEngine = Enum.Parse<Kyna.Infrastructure.Sql.DatabaseEngine>(databaseEngine.ToString());
    }

    if (logsEngine == Kyna.Infrastructure.Sql.DatabaseEngine.PostgreSql)
    {
        logger = Kyna.ApplicationServices.LoggerFactory.CreatePostgreSqlLogger<Program>(
            configuration.GetConnectionString(CliHelper.Keys.Logs));
    }

    KLogger.SetLogger(logger);
}

class Config : IAppConfig
{
    public Config(string appName, string appVersion, string? description = null)
    {
        AppName = appName;
        AppVersion = appVersion;
        Description = description;
    }

    public string AppName { get; }
    public string AppVersion { get; }
    public string? Description { get; }
    public bool Verbose { get; set; }
    public bool ShowHelp { get; set; }
}