using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Khernet.Core.Utility
{
    public class NetworkHelper
    {
        /// <summary>
        /// Get a list of IP addresses with specified protocol.
        /// </summary>
        /// <param name="server">Hostname of IP address</param>
        /// <param name="protocol">The type of protocol</param>
        /// <returns>The list of IP addresses</returns>
        public static List<string> GetIPAddresses(string server, ProtocolFamily protocol)
        {


            try
            {
                // Get server related information.
                IPHostEntry heserver = Dns.GetHostEntry(server);

                List<string> addressList = null;

                // Loop on the AddressList
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    if (protocol.ToString() == curAdd.AddressFamily.ToString())
                    {
                        if (addressList == null)
                            addressList = new List<string>();
                        addressList.Add(curAdd.ToString());
                    }
                }
                return addressList;

            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                throw error;
            }
        }

        /// <summary>
        /// Get IP address of a WCF context
        /// </summary>
        /// <param name="context">The WCF context</param>
        /// <returns>True if connection success, otherwise false</returns>
        public static IPAddress GetIPAddressFromContext(OperationContext context)
        {
            MessageProperties properties = context.IncomingMessageProperties;
            if (context.IncomingMessageProperties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpoint != null)
                {
                    return IPAddress.Parse(endpoint.Address);
                }
            }
            return null;
        }

        /// <summary>
        /// Try to connect to a host with through a specified port
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <param name="port">The port to connect to</param>
        /// <returns>True if connection success, otherwise false</returns>
        public static bool TryConnectToHost(string hostname, int port)
        {
            //First try to connect using host name or IP address
            try
            {
                TcpClient tcpClient = new TcpClient(hostname, port);
                tcpClient.Close();

                return true;
            }
            catch (Exception error)
            {
                //LogDumper.WriteLog(error);
                return false;
            }
        }


        /// <summary>
        /// Try to connect to a host with and IP address and port
        /// </summary>
        /// <param name="ipAddres">The IP address of host</param>
        /// <param name="port">The port to connect to</param>
        /// <returns>True if connection success, otherwise false</returns>
        public static bool TryConnectToIP(string ipAddres, int port)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipAddres, out ip))
                return false;

            ip = IPAddress.Parse(ipAddres);

            //First try to connect using host name or IP address
            try
            {
                TcpClient tcpClient = new TcpClient(ip.AddressFamily);
                tcpClient.Connect(ip, port);
                tcpClient.Close();

                return true;
            }
            catch (Exception error)
            {
                //LogDumper.WriteLog(error);
                return false;
            }
        }

        public static bool IsIPAddress(string ipAddress)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress, out ip))
                return false;
            return true;
        }

        /// <summary>
        /// Try to connect to th available IP addresses of a host with the specified port
        /// </summary>
        /// <param name="ipAddress">The IP address of host</param>
        /// <param name="port">The port to connect to</param>
        /// <returns>True if connection success, otherwise false</returns>
        public static bool TryConnect(string ipAddress, int port)
        {
            IPHostEntry host = Dns.GetHostEntry(ipAddress);

            bool canConnect = false;

            //Try connection with Ipv4 address
            foreach (IPAddress tempIP in host.AddressList)
            {
                if (tempIP.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                {
                    try
                    {
                        IPEndPoint ipEndPoint = new IPEndPoint(tempIP, port);

                        TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                        tcpClient.Connect(ipEndPoint);
                        tcpClient.Close();

                        canConnect = true;
                        break;
                    }
                    catch (Exception error)
                    {
                        LogDumper.WriteLog(error);
                    }
                }
            }

            if (!canConnect)
            {
                //Try connection with Ipv6 address
                foreach (IPAddress tempIP in host.AddressList)
                {
                    if (tempIP.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                    {
                        try
                        {
                            IPEndPoint ipEndPoint = new IPEndPoint(tempIP, port);

                            TcpClient tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
                            tcpClient.Connect(ipEndPoint);
                            tcpClient.Close();

                            canConnect = true;
                            break;
                        }
                        catch (Exception error)
                        {
                            LogDumper.WriteLog(error);
                        }
                    }
                }
            }

            return canConnect;
        }
    }
}
