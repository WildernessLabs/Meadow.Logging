using System;
using System.IO;
using System.Linq;

namespace Meadow.Logging;

/// <summary>
/// A Meadow Logging LogProvider that outputs log information to a local file
/// </summary>
public class FileLogger : ILogProvider
{
    /// <summary>
    /// Gets the path to the current log file
    /// </summary>
    public string LogFilePath { get; }

    /// <summary>
    /// Only log messages at or above this level
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; }

    /// <summary>
    /// Creates a FileLogger instance
    /// </summary>
    /// <param name="minimumLogLevel">Only log messages at or above the provided level</param>
    public FileLogger(LogLevel minimumLogLevel = LogLevel.Warning)
    {
        MinimumLogLevel = minimumLogLevel;

        LogFilePath = Path.Combine(Resolver.Device.PlatformOS.FileSystem.DocumentsDirectory, "meadow.log");
        if (!File.Exists(LogFilePath))
        {
            File.Create(LogFilePath).Close();
        }
    }

    /// <inheritdoc/>
    public void Log(LogLevel level, string message)
    {
        if (level != LogLevel.None && level >= MinimumLogLevel)
        {

            LogToFile(message);
        }
    }

    private void LogToFile(string message)
    {
        if (message.EndsWith(Environment.NewLine))
        {
            File.AppendAllText(LogFilePath, message);
        }
        else
        {
            File.AppendAllText(LogFilePath, message + Environment.NewLine);
        }
    }

    /// <summary>
    /// Reads all lines in the current log file
    /// </summary>
    /// <returns></returns>
    public string[] GetLogContents()
    {
        if (!File.Exists(LogFilePath))
        {
            return new string[0];
        }
        return File.ReadLines(LogFilePath).ToArray();
    }

    /// <summary>
    /// Erases all data in the current log file
    /// </summary>
    public void TruncateLog()
    {
        File.Create(LogFilePath).Close();
    }
}