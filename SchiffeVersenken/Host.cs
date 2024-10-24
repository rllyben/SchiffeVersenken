using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Host
    {
        private static long ipAddress;
        private static int port;

        public static void HostSetup()
        {
            ipAddress = long.Parse(Utils.GetLocalIPAddress());
            port = Utils.GetAvailablePort();
            IPEndPoint iPEndPoint = new(ipAddress, port);
            using Socket host = new(
            iPEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        }

        public static void 

    }

}
