using Kyna.Logging;
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
KLogger.LogDebug("testing Klogger Debug", scope,context,processId);
KLogger.LogInformation("testing Klogger", scope, context, processId);
KLogger.LogWarning("testing Klogger Warning", scope, context, processId);
KLogger.LogError("testing Klogger Error", scope, context, processId);
KLogger.LogCritical("testing Klogger Critical", scope, context, processId);
KLogger.LogCritical(new ArgumentNullException("fake"), scope, context, processId);
KLogger.LogEvent(900, "Application Ended", context, processId);

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