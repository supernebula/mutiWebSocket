using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSocketServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static int myPort = 8885;
        static Socket serverSocket;
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1"); //
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myPort));
            serverSocket.Listen(10);
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsocket发送数据
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();

        }

        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));

                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }

        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    // 通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);
                    var message = Encoding.ASCII.GetString(result, 0, receiveNumber);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), message);
                    if (message == "launch")
                    {
                        StartPublish(() =>
                        {
                            var msg = "client sub launched" + DateTime.Now;
                            myClientSocket.Send(Encoding.ASCII.GetBytes(msg));
                            Console.WriteLine(msg);
                        });
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }

            }

        }

        private static Task _longTask;

        private static CancellationTokenSource _stopTokenSource = new CancellationTokenSource();

        private static Task StartPublish(Action action)
        {
            _longTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_stopTokenSource.Token.IsCancellationRequested)
                    {
                        _stopTokenSource.Dispose();
                        _stopTokenSource = null;
                        break;
                    }

                    Thread.Sleep(400);
                   action.Invoke();
                }
            }, _stopTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return _longTask;
        }
    }
}
