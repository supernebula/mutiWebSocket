using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    class Program
    {
        static SocketAsyncClient client;
        static void Main(string[] args)
        {
            client = new SocketAsyncClient("127.0.0.1", 5000);
            client.Start();

            var i = 10;
            while (i > 0)
            {
                Thread.Sleep(1000);
                client.SendMessage("测试消息"+ DateTime.Now.ToString());
                i--;
            }

            //client.Close();
            Console.ReadLine();
        }
    }
}
