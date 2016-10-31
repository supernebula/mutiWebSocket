using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    public class SocketAsyncClient
    {
        public string ServerIp { get; private set; }
        public int ServerPort { get; private set; }

        private Socket _client;

        private ManualResetEvent _connectLock = new ManualResetEvent(false);
        private ManualResetEvent _sendLock = new ManualResetEvent(false);
        private ManualResetEvent _reveiveLock = new ManualResetEvent(false);
        private string _response = String.Empty;
        private Task _longAcceptTask;
        public SocketAsyncClient(string servIp, int servPort)
        {
            ServerIp = servIp;
            ServerPort = servPort;
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            var servAddress = IPAddress.Parse(ServerIp);
            var servEndPoint = new IPEndPoint(servAddress, ServerPort);

            _client.BeginConnect(servEndPoint, new AsyncCallback(ConnectCallback), _client);
            _connectLock.WaitOne();

            _longAcceptTask = Task.Factory.StartNew((state) => {
                var workSocket = (Socket)state;
                Send(_client, "This is a test<EOF>");
                Receive(_client);
                var a = new object();
            }, _client, TaskCreationOptions.LongRunning); 
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                _connectLock.Set();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Send(Socket workClient, string data)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                workClient.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(SendCallback), workClient);
                _sendLock.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        private void SendCallback(IAsyncResult ar)
        {
            var workSocket = (Socket)ar.AsyncState;
            var sendByteLength = workSocket.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", sendByteLength);
            _sendLock.Set();
        }

        private void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.WorkSocket = client;
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                _reveiveLock.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {    
                var state = (StateObject)ar.AsyncState;
                var workClient = state.WorkSocket;
                int bytesReadLength = workClient.EndReceive(ar);
                if (bytesReadLength > 0)
                {
                    state.Sb.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesReadLength));    
                    workClient.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                { 
                    if (state.Sb.Length > 1)
                    {
                        _response = state.Sb.ToString();
                    }
                    _reveiveLock.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void Close()
        {
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }

        public void SendMessage(string data)
        {
            Send(_client, data);
        }
    }
}
