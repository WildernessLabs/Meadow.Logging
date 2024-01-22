namespace Meadow.Logging
{
    /// <summary>
    /// A Log Provider that outputs to the System Diagnostics Debug output
    /// </summary>
    public class DebugLogProvider : ILogProvider
    {
        /// <inheritdoc/>
        public void Log(LogLevel level, string message, string? messageGroup)
        {
            System.Diagnostics.Debug.WriteLine($"({messageGroup}){level.ToString().ToUpper()}: {message}");
        }
    }
}