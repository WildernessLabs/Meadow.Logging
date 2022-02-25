namespace Meadow.Logging
{
    public class Logger
    {
        private LogProviderCollection _providers = new LogProviderCollection();
        public Loglevel Loglevel { get; set; } = Loglevel.Error;

        public Logger(params ILogProvider[] providers)
        {
            foreach (var p in providers)
            {
                AddProvider(p);
            }
        }

        public Logger(ILogProvider provider)
        {
            AddProvider(provider);
        }

        public void AddProvider(ILogProvider provider)
        {
            lock (_providers)
            {
                _providers.Add(provider);
            }
        }

        public void Debug(string message)
        {
            Log(Loglevel.Debug, message);
        }

        public void DebugIf(bool condition, string message)
        {
            if (condition) Log(Loglevel.Debug, message);
        }

        public void Info(string message)
        {
            Log(Loglevel.Info, message);
        }

        public void InfoIf(bool condition, string message)
        {
            if (condition) Log(Loglevel.Info, message);
        }

        public void Warn(string message)
        {
            Log(Loglevel.Warning, message);
        }

        public void WarnIf(bool condition, string message)
        {
            if (condition) Log(Loglevel.Warning, message);
        }

        public void Error(string message)
        {
            Log(Loglevel.Error, message);
        }

        public void ErrorIf(bool condition, string message)
        {
            if (condition) Log(Loglevel.Error, message);
        }

        private void Log(Loglevel level, string message)
        {
            if (Loglevel < level) return;

            lock (_providers)
            {
                foreach (var p in _providers)
                {
                    p.Log(level, message);
                }
            }
        }
    }
}