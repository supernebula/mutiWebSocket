using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Storage.Models;
using Storage.Sockets;

namespace Storage.QueryEntries
{
    public class FreqLevelPushEntry
    {

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// onDataCallback说明:
        /// </remarks>
        /// <param name="taskId"></param>
        /// <param name="onDataCallback">
        /// 每接受完一次Socket数据，要执行的回调方法。
        /// KeyValuePair的<para>string</para>值是事件名称<see cref="EventMessageType"/>, 
        /// <para>Item</para>时当前方法接受的Socket数据解析得到的实体对象，
        /// 返回值<para>bool</para>决定Socket是否继续接受数据
        /// </param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public Task PushOrStopItemAsync(string taskId, Func<KeyValuePair<string, FreqLevelItem>, Task<bool>> onDataCallback, bool stop = false)
        {
            var @event = "sfdf";
            KeyValuePair<CancellationTokenSource, Socket> tokenSocket;
            SocketPool.TryGetValue(@event, out tokenSocket);

            if (onDataCallback == null || stop)
            {
                KeyValuePair<CancellationTokenSource, Socket> tSocket;
                SocketPool.TryRemove(@event, out tSocket);
                if (tSocket.Value == null)
                    return Task.FromResult(false);
                tSocket.Key.Cancel();
                tSocket.Value.Shutdown(SocketShutdown.Both);
                tSocket.Value.Close();
                tSocket.Value.Dispose();
                tSocket.Key.Dispose();
            }

            KeyValuePair<CancellationTokenSource, Socket> tokenSocket2;
            SocketPool.TryRemove(@event, out tokenSocket2);
            tokenSocket2.Key.Cancel();
            tokenSocket2.Value.Shutdown(SocketShutdown.Both);
            tokenSocket2.Value.Close();
            tokenSocket2.Value.Dispose();
            tokenSocket2.Key.Dispose();

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var cancelTokenSource = new CancellationTokenSource();
            SocketPool.TryAdd(@event, new KeyValuePair<CancellationTokenSource, Socket>(cancelTokenSource, clientSocket));

            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                var port = 8885;
                clientSocket.Connect(new IPEndPoint(ip, port)); // 配置服务器IP与端口
                Console.WriteLine("连接服务器成功");
            }
            catch(Exception ex)
            {
                Debug.WriteLine("连接服务器失败，请按回车键退出！");
            }

            var cancelokenSource = new CancellationTokenSource();
            var buffer = new byte[1024];
            clientSocket.Send(Encoding.ASCII.GetBytes("launch"));
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((asyncResult) =>
            {
                AsyncLoopReceiveData(asyncResult);
            }), new Tuple<Socket, CancellationTokenSource, Func<string, Task<bool>>>(
                clientSocket, 
                cancelokenSource, 
                (msg) => {
                        return onDataCallback.Invoke(new KeyValuePair<string, FreqLevelItem>(@event, new FreqLevelItem(/*msg*/)));
                    }
                ));

            #region OLD

            //var buffer = new byte[1024];
            //clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((asyncResult) =>
            //{

            //    var tokenSource = asyncResult as CancellationTokenSource;
            //    if (tokenSource.IsCancellationRequested)
            //    {
            //        return;
            //    }

            //    var length = clientSocket.EndReceive(asyncResult);
            //    var message = Encoding.UTF8.GetString(buffer, 0, length);
            //    if(onDataCallback != null)
            //    onDataCallback.Invoke(new KeyValuePair<string, Item>(@event, new Item())).ContinueWith(t =>
            //    {
            //        if(!t.Result)
            //            tokenSource.Cancel();
            //    });

            //    //callback(message);
            //    Console.WriteLine(message);
            //}), cancelokenSource);
            #endregion
            return Task.FromResult(1);
        }

        private static void AsyncLoopReceiveData(IAsyncResult ar)
        {
            var tupleState = ar.AsyncState as Tuple<Socket, CancellationTokenSource, Func<string, Task<bool>>>;
            if (tupleState == null)
                return;
            var socket = tupleState.Item1;
            CancellationTokenSource cancelSource = tupleState.Item2;
            var onDataCallBack = tupleState.Item3;
            if (cancelSource.Token.IsCancellationRequested)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
                return;
            }

            if (socket == null)
                return;
            if (!socket.Connected)
                return;
            if (cancelSource.Token.IsCancellationRequested)
                return;
            var buffer = new byte[1024];
            var length = socket.EndReceive(ar);
            var message = Encoding.UTF8.GetString(buffer, 0, length);
            onDataCallBack.Invoke(message).ContinueWith((t) => {
                    if (!t.Result && !cancelSource.IsCancellationRequested && cancelSource.Token.CanBeCanceled)
                        cancelSource.Cancel();
                 });
#if DEBUG
            Console.WriteLine(message + (new Random(Guid.NewGuid().GetHashCode())).Next(100000, 999999));
#endif
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(AsyncLoopReceiveData), tupleState);
        }
    }
}
