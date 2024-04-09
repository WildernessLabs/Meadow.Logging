using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Meadow.Logging
{
    /// <summary>
    /// A Meadow Logging LogProvider that outputs log information over UDP broadcast
    /// </summary>
    public class UdpLogger : ILogProvider, IDisposable
    {
        private bool _isDisposed;
        private readonly int _port;
        private readonly UdpClient _client;
        private readonly IPEndPoint _broadcast;
        private readonly char _delimiter;

        /// <summary>
        /// Creates a UdpLogger instance
        /// </summary>
        /// <param name="port">The UDP port to broadcast data on</param>
        /// <param name="delimiter">The delimiter used between the log level and the log information</param>
        public UdpLogger(int port = 5100, char delimiter = '\t')
        {
            _port = port;
            _delimiter = delimiter;
            _client = new UdpClient();
            _client.Client.Bind(new IPEndPoint(IPAddress.Any, _port));
            _broadcast = new IPEndPoint(IPAddress.Broadcast, _port);
        }

        /// <inheritdoc/>
        public void Log(LogLevel level, string message, string? _)
        {
            var payload = Encoding.UTF8.GetBytes($"{level}{_delimiter}{message}\n");
            try
            {
                _client.Send(payload, payload.Length, _broadcast);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UDP ERR: {ex.Message}");
                // TODO: ignore exceptions ?
            }
        }

        /// <summary>
        /// Releases resources associated with the logger instance
        /// </summary>
        /// <param name="disposing">Is this being called from Dispose?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Releases resources associated with the logger instance
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}