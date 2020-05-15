using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;

namespace Trivia_Client
{
    class Communicator
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 8998;

        public Communicator()
        {
            /*
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, SERVER_PORT);

            _serverSocket = new Socket(ipAddress.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);*/

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] iPs = Dns.GetHostAddresses(SERVER_IP);

            try
            {
                _serverSocket.Connect(iPs, SERVER_PORT);
            }
            catch (Exception)
            {
                throw new Exception("Faild to connect to server.\n Do you wish to try again?");
            }
        }

        ~Communicator()
        {
            _serverSocket.Close();
        }

        private Socket _serverSocket;
    }
}
