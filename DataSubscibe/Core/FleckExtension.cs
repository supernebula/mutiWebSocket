using System.Threading.Tasks;
using DataSubscibe.SocketHandlers;
using Fleck;
using Newtonsoft.Json;

namespace DataSubscibe.Core
{
    public static class FleckExtension
    {
        public static Task Send(this IWebSocketConnection webSocketConnection, SocketHandResult handResult)
        {
            var message = JsonConvert.SerializeObject(handResult);
            return webSocketConnection.Send(message);
        }
    }
}