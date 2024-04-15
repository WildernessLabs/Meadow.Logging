using Meadow.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Meadow.Logging;

public class CloudLogger : ILogProvider
{
    /// <summary>
    /// Instantiate a new CloudLogger
    /// </summary>
    /// <param name="level">Minimum level to log (cannot be lower than Information)</param>
    /// <exception cref="ArgumentException"></exception>
    public CloudLogger(LogLevel level = LogLevel.Information)
    {
        if (level <= LogLevel.Debug)
        {
            // there is debug logging in the method that sends the logs to the cloud. If we allowed
            // cloud logging at the level (or below) we'd end up in an infinite loop.
            throw new ArgumentException("Minimum level for CloudLogger is Information");
        }

        MinLevel = level;

        LogFilePath = Path.Combine(Resolver.Device.PlatformOS.FileSystem.DocumentsDirectory, "cloud.log");
        if (!File.Exists(LogFilePath))
        {
            using FileStream fs = File.Create(LogFilePath);
            fs.Close();
        }

        EventFilePath = Path.Combine(Resolver.Device.PlatformOS.FileSystem.DocumentsDirectory, "events.log");
        if (!File.Exists(EventFilePath))
        {
            using FileStream fs = File.Create(EventFilePath);
            fs.Close();
        }
    }

    /// <summary>
    /// Path to the log file
    /// </summary>
    public string LogFilePath { get; protected set; }
    /// <summary>
    /// Path to the event file
    /// </summary>
    public string EventFilePath { get; protected set; }
    /// <summary>
    /// Current minimum level for the CloudLogger
    /// </summary>
    public LogLevel MinLevel { get; protected set; }

    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    /// <inheritdoc/>
    public void Log(LogLevel level, string message, string? _)
    {
        if (level >= MinLevel)
        {
            var log = new CloudLog()
            {
                Severity = level.ToString(),
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            Resolver.MeadowCloudService.SendLog(log);
        }
    }

    /// <summary>
    /// Log an exception.
    /// </summary>
    /// <param name="ex"></param>
    public void LogException(Exception ex)
    {
        var log = new CloudLog()
        {
            Severity = LogLevel.Error.ToString(),
            Exception = ex.StackTrace,
            Message = ex.Message,
            Timestamp = DateTime.UtcNow
        };

        Resolver.MeadowCloudService.SendLog(log);
    }

    /// <summary>
    /// Log an event.
    /// </summary>
    /// <param name="eventId">id used for a set of events.</param>
    /// <param name="description">Description of the event.</param>
    /// <param name="measurements">Dynamic payload of measurements to be recorded.</param>
    public void LogEvent(int eventId, string description, Dictionary<string, object> measurements)
    {
        var cloudEvent = new CloudEvent()
        {
            EventId = eventId,
            Description = description,
            Measurements = measurements,
            Timestamp = DateTime.UtcNow
        };

        Resolver.MeadowCloudService.SendEvent(cloudEvent);
    }
}