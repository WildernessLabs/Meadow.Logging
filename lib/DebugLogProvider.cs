namespace Meadow.Logging
{
    public class DebugLogProvider : ILogProvider
    {
        public void Log(Loglevel level, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{level.ToString().ToUpper()}: {message}");
        }
    }
}