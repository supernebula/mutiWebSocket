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

namespace Storage.QueryEntries
{
    public class ItemPushEntry
    {
        public Task PushOrStopItemAsync(string taskId, Func<Item, Task<int>> onDataCallback, bool stop = false)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); // 配置服务器IP与端口
                Console.WriteLine("连接服务器成功");


            }
            catch(Exception ex)
            {
                Debug.WriteLine("连接服务器失败，请按回车键退出！");
            }

            clientSocket.Send(Encoding.ASCII.GetBytes("launch"));

            var buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((asyncResult) =>
            {
                var length = clientSocket.EndReceive(asyncResult);
                var message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine(message);
            }), null);
            return Task.FromResult(1);
        }
    }
}
