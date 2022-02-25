using System.Collections;
using System.Collections.Generic;

namespace Meadow.Logging
{
    public class LogProviderCollection : IEnumerable<ILogProvider>
    {
        private List<ILogProvider> Providers { get; set; } = new List<ILogProvider>();

        internal LogProviderCollection()
        {
        }

        public void Add(ILogProvider provider)
        {
            Providers.Add(provider);
        }

        public IEnumerator<ILogProvider> GetEnumerator()
        {
            return Providers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}