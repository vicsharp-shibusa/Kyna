using Microsoft.Extensions.Logging;

namespace Kyna.ApplicationServices
{
    public static class LoggerFactory
    {
        public static ILogger<T> CreatePostgreSqlLogger<T>(
            string? connectionString,
            LogLevel minLogLevel = LogLevel.Trace,
            Func<string, LogLevel, bool>? filter = null)
        {
            var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new Infrastructure.Logging.PostgreSQL.LoggerProvider(connectionString, filter));
                builder.SetMinimumLevel(minLogLevel);
            });

            var logger = loggerFactory.CreateLogger<T>();

            return logger;
        }
    }
}
