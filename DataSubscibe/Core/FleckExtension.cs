using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataSubscibe.SocketHandlers;
using Fleck;
using Newtonsoft.Json;

namespace DataSubscibe.Core
{
    public static class FleckExtension
    {
        public static async Task Send(this IWebSocketConnection webSocketConnection, SocketHandResult handResult)
        {
            var message = JsonConvert.SerializeObject(handResult);
            await webSocketConnection.Send(message);
        }
    }
}