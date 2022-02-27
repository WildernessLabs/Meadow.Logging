namespace Meadow.Logging
{
    /// <summary>
    /// Interface for Log Providers
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Called when the associated Logger has a message call
        /// </summary>
        /// <param name="level">The LogLevel for the message</param>
        /// <param name="message">The message to log</param>
        void Log(Loglevel level, string message);
    }
}