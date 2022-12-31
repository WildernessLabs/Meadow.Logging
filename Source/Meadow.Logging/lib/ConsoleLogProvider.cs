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
        public bool ShowLoglevel { get; set; } = false;

        /// <summary>
        /// Called when the associated Logger has a message call
        /// </summary>
        /// <param name="level">The LogLevel for the message</param>
        /// <param name="message">The message to log</param>
        public void Log(LogLevel level, string message)
        {
            if (ShowLoglevel)
            {
                Console.WriteLine($"{level.ToString().ToUpper()}: {message}");
            }
            else
            {
                Console.WriteLine($"{message}");
            }
        }
    }
}