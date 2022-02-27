namespace Meadow.Logging
{
    /// <summary>
    /// A Log Provider that outputs to the System Diagnostics Debug output
    /// </summary>
    public class DebugLogProvider : ILogProvider
    {
        /// <summary>
        /// Called when the associated Logger has a message call
        /// </summary>
        /// <param name="level">The LogLevel for the message</param>
        /// <param name="message">The message to log</param>
        public void Log(Loglevel level, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{level.ToString().ToUpper()}: {message}");
        }
    }
}