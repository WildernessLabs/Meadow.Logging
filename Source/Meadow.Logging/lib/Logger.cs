using System;
using System.Collections.Generic;

namespace Meadow.Logging;

/// <summary>
/// Class encapsulating logging providers and functions
/// </summary>
public class Logger
{
    /// <summary>
    /// Defines message groups for categorizing log messages.
    /// </summary>
    public static class MessageGroup
    {
        /// <summary>
        /// Represents all log messages.
        /// </summary>
        public const string All = "all";

        /// <summary>
        /// Represents the operating system-related log messages.
        /// </summary>
        public const string OS = "os";

        /// <summary>
        /// Represents the core system-related log messages.
        /// </summary>
        public const string Core = "core";

        /// <summary>
        /// Represents the application-specific log messages.
        /// </summary>
        public const string Application = "application";
    }

    private readonly LogProviderCollection _providers = new();

    private readonly int _startupTicks;

    /// <summary>
    /// Gets the list of groups used for displaying log messages.
    /// </summary>
    /// <remarks>
    /// To show message from all groups, either use `MessageGroup.All` <i>or</i> clear the ShowsGroups list
    /// </remarks>
    public List<string> ShowGroups { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to show ticks in log messages
    /// </summary>
    public bool ShowTicks { get; set; } = false;

    /// <summary>
    /// Gets or sets the current log level
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Error;

    /// <summary>
    /// Creates a Logger instance
    /// </summary>
    /// <param name="providers">A collection of providers to add to the Logger</param>
    public Logger(params ILogProvider[] providers)
    {
        ShowGroups.Add(MessageGroup.Application);

        foreach (var p in providers)
        {
            AddProvider(p);
        }
    }

    /// <summary>
    /// Creates a Logger instance
    /// </summary>
    /// <param name="provider">A provider to add to the Logger</param>
    public Logger(ILogProvider provider)
    {
        AddProvider(provider);
    }

    /// <summary>
    /// Creates a Logger instance
    /// </summary>
    public Logger()
    {
        _startupTicks = Environment.TickCount;
    }

    /// <summary>
    /// Adds an ILogProvider to the providers collection
    /// </summary>
    /// <param name="provider"></param>
    public void AddProvider(ILogProvider provider)
    {
        lock (_providers)
        {
            _providers.Add(provider);
        }
    }

    /// <summary>
    /// Sends a Trace-level message to all ILogProviders
    /// </summary>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void Trace(string message, string? messageGroup = null)
    {
        Log(LogLevel.Trace, message, messageGroup);
    }

    /// <summary>
    /// Conditionally sends a Trace-level message to all ILogProviders
    /// </summary>
    /// <param name="condition">The message will be sent to Providers only when this is true</param>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void TraceIf(bool condition, string message, string? messageGroup = null)
    {
        if (condition) Log(LogLevel.Trace, message, messageGroup);
    }

    /// <summary>
    /// Sends a Debug-level message to all ILogProviders
    /// </summary>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void Debug(string message, string? messageGroup = null)
    {
        Log(LogLevel.Debug, message, messageGroup);
    }

    /// <summary>
    /// Conditionally sends a Debug-level message to all ILogProviders
    /// </summary>
    /// <param name="condition">The message will be sent to Providers only when this is true</param>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void DebugIf(bool condition, string message, string? messageGroup = null)
    {
        if (condition) Log(LogLevel.Debug, message, messageGroup);
    }

    /// <summary>
    /// Sends an Info-level message to all ILogProviders
    /// </summary>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void Info(string message, string? messageGroup = null)
    {
        Log(LogLevel.Information, message, messageGroup);
    }

    /// <summary>
    /// Conditionally sends a Info-level message to all ILogProviders
    /// </summary>
    /// <param name="condition">The message will be sent to Providers only when this is true</param>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void InfoIf(bool condition, string message, string? messageGroup = null)
    {
        if (condition) Log(LogLevel.Information, message, messageGroup);
    }

    /// <summary>
    /// Sends a Warn-level message to all ILogProviders
    /// </summary>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void Warn(string message, string? messageGroup = null)
    {
        Log(LogLevel.Warning, message, messageGroup);
    }

    /// <summary>
    /// Conditionally sends a Warn-level message to all ILogProviders
    /// </summary>
    /// <param name="condition">The message will be sent to Providers only when this is true</param>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void WarnIf(bool condition, string message, string? messageGroup = null)
    {
        if (condition) Log(LogLevel.Warning, message, messageGroup);
    }

    /// <summary>
    /// Sends a Error-level message to all ILogProviders
    /// </summary>
    /// <param name="exception">The exception to translate and send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void Error(Exception exception, string? messageGroup = null)
    {
        Log(LogLevel.Error, exception.ToString(), messageGroup);
    }

    /// <summary>
    /// Sends a Error-level message to all ILogProviders
    /// </summary>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void Error(string message, string? messageGroup = null)
    {
        Log(LogLevel.Error, message, messageGroup);
    }

    /// <summary>
    /// Conditionally sends a Error-level message to all ILogProviders
    /// </summary>
    /// <param name="condition">The message will be sent to Providers only when this is true</param>
    /// <param name="message">The message to send to Providers</param>
    /// <param name="messageGroup">The (optional) message group</param>
    public void ErrorIf(bool condition, string message, string? messageGroup = null)
    {
        if (condition) Log(LogLevel.Error, message, messageGroup);
    }

    private void Log(LogLevel level, string message, string? messageGroup)
    {
        if (LogLevel > level) return;

        if (messageGroup == null) messageGroup = MessageGroup.Application;

        if (ShowGroups.Count > 0)
        {
            if (!ShowGroups.Contains(MessageGroup.All)
                && !ShowGroups.Contains(messageGroup))
            {
                return;
            }
        }

        TimeSpan? now = null;

        lock (_providers)
        {
            if (ShowTicks)
            {
                now = TimeSpan.FromMilliseconds(Environment.TickCount - _startupTicks);
            }

            foreach (var p in _providers)
            {
                if (now != null)
                {
                    message = $"[+{now:h\\:m\\:ss\\.FFF}] {message}";
                }

                p.Log(level, message, messageGroup);
            }
        }
    }
}