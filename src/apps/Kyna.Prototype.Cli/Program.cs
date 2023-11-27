using Kyna.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

ILogger<Program>? logger = null;
IConfiguration? configuration;

Configure();

Stopwatch timer = Stopwatch.StartNew();

KLogger.LogInformation("testing Klogger");

await Task.Delay(1000);

//if (logger is not null)
//{
//    using (logger.BeginScope("LogItem Test"))
//    {
//        Guid processId = Guid.NewGuid();
//        for (int i = 0; i < 10; i++)
//        {
//            var logItem = new LogItem("some information message",
//                logLevel: LogLevel.Warning,
//                context: "making YouTube videos",
//                processId: processId);
//            logger.Log<LogItem>(logItem.LogLevel, default, logItem, null,
//                (LogItem logItem, Exception? exc) =>
//                {
//                    return logItem.Message ?? exc?.Message ?? string.Empty;
//                });

//            //logger.LogDebug($"Test at {DateTime.Now}");

//            if (i > 1 && i % 2 == 0)
//            {
//                //logger.LogCritical(new Exception("arbitrary exception"), "message");
//                logItem = new LogItem(new Exception("BAD"),
//                context: nameof(Program),
//                processId: processId);

//                logger.Log<LogItem>(logItem.LogLevel, new EventId(100, "Even Number"), logItem, null,
//                    (LogItem logItem, Exception? exc) =>
//                    {
//                        return logItem.Message ?? exc?.Message ?? string.Empty;
//                    });
//                await Task.Delay(500);
//            }
//        }
//    }
//}

timer.Stop();

Console.WriteLine(timer.ElapsedMilliseconds);
Console.WriteLine("done");

void Configure()
{
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);

    configuration = builder.Build();

    logger = Kyna.ApplicationServices.LoggerFactory.CreatePostgreSqlLogger<Program>(configuration.GetConnectionString("Logs"));

    KLogger.SetLogger(logger);
}

//void ConfigureServices()
//{
//    var services = new ServiceCollection();


//    //logger = Kyna.ApplicationServices.LoggerFactory.CreatePostgreSqlLogger("Prototype")
//    //string sourceName = "Prototype";

//    //var loggerFactory = LoggerFactory.Create(builder =>
//    //{
//    //    builder.ClearProviders();
//    //    builder.AddProvider(new )
//    //    builder.AddConsole();
//    //    builder.AddFilter("Program", LogLevel.Trace);
//    //    builder.SetMinimumLevel(LogLevel.Trace);
//    //});

//    //logger = loggerFactory.CreateLogger<Program>();
//    //_ = logger.BeginScope(sourceName);

//}