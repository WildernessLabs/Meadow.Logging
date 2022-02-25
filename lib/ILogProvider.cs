namespace Meadow.Logging
{
    public interface ILogProvider
    {
        void Log(Loglevel level, string message);
    }
}