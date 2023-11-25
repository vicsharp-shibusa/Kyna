using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

ILogger<Program>? logger = null;
IConfiguration? configuration = null;

Configure();

Stopwatch timer = Stopwatch.StartNew();

if (logger is not null)
{
    using (logger.BeginScope("Prototype"))
    {
        for (int i = 0; i < 10; i++)
        {
            logger.LogDebug($"Test at {DateTime.Now}");

            if (i > 1 && i % 2 == 0)
            {
                logger.LogCritical(new Exception("arbitrary exception"), "message");
                await Task.Delay(500);  
            }
        }
    }
}

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