using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    class Program
    {
        static SocketAsyncServer _socketServer;
        static void Main(string[] args)
        {
            _socketServer = new SocketAsyncServer("127.0.0.1", 5000);
            _socketServer.Start();
            Console.ReadLine();
        }
    }
}
