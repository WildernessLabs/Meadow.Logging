using System.Collections;
using System.Collections.Generic;

namespace Meadow.Logging
{
    /// <summary>
    /// A collection of ILogProviders
    /// </summary>
    public class LogProviderCollection : IEnumerable<ILogProvider>
    {
        private List<ILogProvider> Providers { get; set; } = new List<ILogProvider>();

        internal LogProviderCollection()
        {
        }

        /// <summary>
        /// Adds a Provider to the collection
        /// </summary>
        /// <param name="provider"></param>
        public void Add(ILogProvider provider)
        {
            lock (Providers)
            {
                Providers.Add(provider);
            }
        }

        /// <summary>
        /// Gets an Enumerator for the collection
        /// </summary>
        /// <returns></returns>
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