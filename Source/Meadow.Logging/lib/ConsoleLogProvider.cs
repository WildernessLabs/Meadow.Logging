using System;

namespace Meadow.Logging
{
    /// <summary>
    /// A Log Provider that outputs to the System Console
    /// </summary>
    public class ConsoleLogProvider : ILogProvider
    {
        /// <summary>
        /// When true, the current log level will be prefixed to all logged messages
        /// </summary>
        public bool ShowLogLevel { get; set; } = false;

        /// <summary>
        /// When true, the current message group will be prefixed to all logged messages
        /// </summary>
        public bool ShowMessageGroup { get; set; } = false;

        /// <inheritdoc/>
        public void Log(LogLevel level, string message, string? messageGroup)
        {
            if (ShowLogLevel)
            {
                if (ShowMessageGroup)
                {
                    Console.WriteLine($"({messageGroup}) {level.ToString().ToUpper()}: {message}");
                }
                else
                {
                    Console.WriteLine($"{level.ToString().ToUpper()}: {message}");
                }
            }
            else
            {
                if (ShowMessageGroup)
                {
                    Console.WriteLine($"({messageGroup}) {message}");
                }
                else
                {
                    Console.WriteLine($"{message}");
                }
            }
        }
    }
}