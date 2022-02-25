namespace Meadow.Logging
{
    public class ConsoleLogProvider : ILogProvider
    {
        public void Log(Loglevel level, string message)
        {
            System.Console.WriteLine($"{level.ToString().ToUpper()}: {message}");
        }
    }
}