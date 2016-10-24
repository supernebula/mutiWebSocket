using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSocketClient
{
    class Program
    {
        private static byte[] result = new byte[1024];

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1"); //127.0.0.1
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); // 配置服务器IP与端口
                Console.WriteLine("连接服务器成功");


            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }

            int receiveLength = clientSocket.Receive(result);
            Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));
            // 通过clientSocket发送数据
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Thread.Sleep(1000);
                    string sendMessage = "client send Message Hello" + DateTime.Now;
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Console.WriteLine("向服务器发送消息：" + sendMessage);
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }

            }

            Console.WriteLine("发送完毕，启动订阅");
            StartReceive(clientSocket);
            Console.ReadLine();
        }

        private static byte[] buffer = new byte[1024];
        private static CancellationTokenSource _stopTokenSource = new CancellationTokenSource();
        private static void ReceiveMessage(IAsyncResult ar)
        {
            var tupleState = ar.AsyncState as Tuple<Socket, CancellationToken>;
            if(tupleState == null)
                return;
            var socket = tupleState.Item1;
            CancellationToken token = tupleState.Item2;
            if (token.IsCancellationRequested)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
                return;
            }
            
            if (socket == null)
                return;
            if(!socket.Connected)
                return;
            if (token.IsCancellationRequested)
                return;

            var length = socket.EndReceive(ar);
            var message = Encoding.UTF8.GetString(buffer, 0, length);

            //callback(message);
            Console.WriteLine(message + (new Random(Guid.NewGuid().GetHashCode())).Next(100000, 999999));

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), tupleState);
        }

        

        private static void StartReceive(Socket clientSocket)
        {
            clientSocket.Send(Encoding.ASCII.GetBytes("launch"));

            var tupleState = new Tuple<Socket, CancellationToken>(clientSocket, _stopTokenSource.Token);
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), tupleState);

        }


    }
}
