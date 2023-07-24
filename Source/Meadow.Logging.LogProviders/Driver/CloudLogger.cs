using System;
using System.IO;
using System.Linq;
using System.Threading;
using Meadow.Cloud;

namespace Meadow.Logging;

public class CloudLogger : ILogProvider
{
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
            File.Create(LogFilePath).Close();
        }
    }

    public string LogFilePath { get; protected set; }
    public LogLevel MinLevel { get; protected set; }
    private char delim = '|';
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    public async void Log(LogLevel level, string message)
    {
        if (level >= MinLevel)
        {
            Resolver.Log.Debug($"cloudlogger.log called");
            
            var cloudLog = new CloudLog()
            {
                Level = level.ToString(),
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            
            var connected = Resolver.Device.NetworkAdapters.Any(a => a.IsConnected);

            if (connected)
            {
                await semaphoreSlim.WaitAsync();

                try
                {
                    // send messages that were stored offline
                    var lines = File.ReadAllLines(LogFilePath);
                    if (lines.Length > 0)
                    {
                        Resolver.Log.Debug($"processing {lines.Length} stored cloud logs");
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }

                            var parsed = ParseLine(line);
                            if (parsed != null)
                            {
                                await Resolver.MeadowCloudService.SendLog(parsed);
                            }
                        }

                        File.Create(LogFilePath).Close();
                        Resolver.Log.Debug($"cleared stored cloud logs");
                    }

                    // send current message
                    Resolver.Log.Debug($"sending log");
                    await Resolver.MeadowCloudService.SendLog(cloudLog);
                }
                catch (Exception ex)
                {
                    Resolver.Log.Debug($"error sending cloud log: {ex.Message}");
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
            else
            {
                Resolver.Log.Debug("writing cloud log to store");
                File.AppendAllLines(LogFilePath, 
                    new[] { $"{cloudLog.Level}{delim}{cloudLog.Timestamp}{delim}{cloudLog.Message}" });
            }
        }
    }

    private CloudLog? ParseLine(string line)
    {
        var cloudLog = new CloudLog();
        
        var parts = line.Split(delim);
        if (parts.Length >= 3)
        {
            cloudLog.Level = parts[0];
            cloudLog.Timestamp = DateTime.Parse(parts[1]);
            cloudLog.Message = string.Join(delim, parts.Skip(2).Take(parts.Length - 2));
            return cloudLog;
        }

        return null;
    }
}