using Meadow.Cloud;
using Meadow.Foundation.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Logging;

/// <summary>
/// Provides functionality to log messages, exceptions, and events to the cloud.
/// Messages are sent immediately if a network connection is available, or stored locally and sent when the connection is restored.
/// </summary>
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

    private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    /// <inheritdoc/>
    public async void Log(LogLevel level, string message, string? _)
    {
        if (level >= MinLevel)
        {
            var cloudLog = new CloudLog()
            {
                Severity = level.ToString(),
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await Send(LogFilePath, cloudLog, Resolver.MeadowCloudService.SendLog);
        }
    }

    /// <summary>
    /// Log an exception.
    /// </summary>
    /// <param name="ex"></param>
    public async void LogException(Exception ex)
    {
        var log = new CloudLog()
        {
            Severity = LogLevel.Error.ToString(),
            Exception = ex.StackTrace,
            Message = ex.Message,
            Timestamp = DateTime.UtcNow
        };

        await Send(LogFilePath, log, Resolver.MeadowCloudService.SendLog);
    }

    /// <summary>
    /// Log an event.
    /// </summary>
    /// <param name="eventId">id used for a set of events.</param>
    /// <param name="description">Description of the event.</param>
    /// <param name="measurements">Dynamic payload of measurements to be recorded.</param>
    public async void LogEvent(int eventId, string description, Dictionary<string, object> measurements)
    {
        var cloudEvent = new CloudEvent()
        {
            EventId = eventId,
            Description = description,
            Measurements = measurements,
            Timestamp = DateTime.UtcNow
        };

        await Send(EventFilePath, cloudEvent, Resolver.MeadowCloudService.SendEvent);
    }

    private async Task Send<T>(string file, T item, Func<T, Task> sendFunc)
    {
        var networkConnected = Resolver.Device.NetworkAdapters.Any(a => a.IsConnected);
        var cloudConnected = Resolver.MeadowCloudService.ConnectionState == CloudConnectionState.Connected;

        if (networkConnected && cloudConnected)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                // send messages that were stored offline
                var lines = File.ReadAllLines(file);
                if (lines.Length > 0)
                {
                    Resolver.Log.Debug($"processing {lines.Length} stored {typeof(T)}");
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        var o = MicroJson.Deserialize<T>(line);

                        if (o != null)
                        {
                            await sendFunc(o);
                        }
                    }

                    using FileStream fs = File.Create(file);
                    fs.Close();
                    Resolver.Log.Debug($"cleared stored {typeof(T)}");
                }

                // send current message
                Resolver.Log.Debug($"sending {typeof(T)}");
                await sendFunc(item);
            }
            catch (Exception ex)
            {
                Resolver.Log.Debug($"error sending {typeof(T)}: {ex.Message}");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        else
        {
            var json = MicroJson.Serialize(item!);

            File.AppendAllLines(file, new[] { json });
            Resolver.Log.Debug($"saved cloud log to local store {json}");
        }
    }
}