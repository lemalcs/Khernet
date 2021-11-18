using System;
using System.Net;
using System.Net.Sockets;

namespace Khernet.Core.Utility
{
    public static class DiscoveryHelper
    {
        public static Uri AvailableIPCBaseAddress
        {
            get
            {
                return new Uri(String.Format("net.pipe://{0}/{1}/", Environment.MachineName, Guid.NewGuid()));
            }
        }

        public static Uri AvailableTCPBaseAddress
        {
            get
            {
                return new Uri(String.Format("net.tcp://{0}:{1}", Environment.MachineName, GetAvailablePort()));
            }
        }

        public static Uri BuildTcpAddress(string hostname, int port)
        {
            return new Uri(String.Format("net.tcp://{0}:{1}", hostname, port));
        }

        /// <summary>
        /// Gets an available port number not used by current process only. Warning, other process can generate the same port number.
        /// </summary>
        /// <returns>The unused port number.</returns>
        private static int GetAvailablePort()
        {
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, "DiscoveryHelper");

            try
            {
                mutex.WaitOne();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Bind(endPoint);
                    IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
                    return localEndPoint.Port;
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

    }
}
