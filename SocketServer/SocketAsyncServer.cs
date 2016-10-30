using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SocketServer
{
    public class SocketAsyncServer
    {
        public string ListenIp {get; private set;}
        public int Port { get; private set; }

        private Socket _serverSocket;

        private Task _longAcceptTask;

        private ManualResetEvent _loopAcceptLock = new ManualResetEvent(false);
        public ConcurrentBag<Socket> ClientCollection = new ConcurrentBag<Socket>();

        public SocketAsyncServer(string ip, int port)
        {
            ListenIp = ip;
            Port = port;
        }

        public void Start()
        {
            IPAddress ipAddr = IPAddress.Parse(ListenIp); 
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _serverSocket.Bind(new IPEndPoint(ipAddr, Port));
                _serverSocket.Listen(10);
                Console.WriteLine("启动监听{0}成功", _serverSocket.LocalEndPoint.ToString());
                //通过Clientsocket发送数据


                _longAcceptTask = Task.Factory.StartNew(Accept, _serverSocket , TaskCreationOptions.LongRunning); 
                //Thread myThread = new Thread(ListenClientConnect);
                //myThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常:{0}, \r\n {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                _serverSocket.Close();
                _serverSocket.Dispose();
                Console.WriteLine("停止服务端监听.");
            }
            

        }

        private void Accept(object socketState)
        {
            var servSocket = (Socket)socketState;
            try
            {
                while (true)
                {
                    _loopAcceptLock.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    servSocket.BeginAccept(new AsyncCallback(AcceptCallback), servSocket);
                    _loopAcceptLock.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _loopAcceptLock.Set();
            var servSocket = (Socket)ar.AsyncState;
            var hander = servSocket.EndAccept(ar);
            var state = new StateObject() { WorkSocket = hander };
            hander.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ResolveCallback), state);
        }


        private void ResolveCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;
            var readLength = handler.EndReceive(ar);
            if (readLength <= 0)
                return;
            var content = Encoding.UTF8.GetString(state.Buffer, 0, readLength);
            state.Sb.Append(content);
            var allContent = state.Sb.ToString();
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ResolveCallback), state);
            Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
            handler.Send(Encoding.UTF8.GetBytes("Copy:" + content));
            //if (content.IndexOf("<EOF>") > -1)
            //{
            //    //发现结尾字符，接受完成
            //    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
            //    //Echo to the client
            //}
            //else
            //{
            //    //未完成，继续接受
            //    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ResolveCallback), state);
            //}
        }

        private void Send(Socket handler, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            handler.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket)ar.AsyncState;
                var sendLength = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", sendLength);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception)
            {
                
                throw;
            }
        }



        ///// <summary>
        ///// 监听客户端连接
        ///// </summary>
        //private void ListenClientConnect()
        //{
        //    byte[] buffer = new byte[1024];
        //    var autoLock = new AutoResetEvent(true);

        //    while (true)
        //    {
        //        autoLock.Set();
        //        _serverSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((asyncResult) => {
        //            var serSocket = (Socket)asyncResult.AsyncState;
        //            var client = serSocket.EndAccept(asyncResult);
        //            autoLock.Reset();
        //        }), _serverSocket);

        //        autoLock.WaitOne();
        //    }

        //}

        public void Shutdown()
        { }
    }
}
