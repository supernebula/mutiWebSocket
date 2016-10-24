using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataSubscibe.Core;
using DataSubscibe.Core.PublishSubscribe;
using Storage.QueryEntries;
using Storage.Models;
using DataSubscibe.Core.PushEntrys;
using Fleck;
using Newtonsoft.Json;

namespace DataSubscibe.SocketHandlers
{
    public class FreqLevelHandler : SocketMessageHandler
    {
#if NoIOC
        public FreqLevelHandler(IWebSocketConnection webSocketConnection, ISubPubScheduler subPubScheduler)
            : base(webSocketConnection)
        {
            FreqLevelPushEntry = new FreqLevelPushEntry();
        }

#endif

        public FreqLevelPushEntry FreqLevelPushEntry { get; set; }
        public ISubPubScheduler SubPubScheduler { get; set; }

        [WebSocketRoute(Path = "/freqlevel/item")]
        public async Task ItemAsync(string taskId, string @event = "freqlev")
        {
            SubPubScheduler.AddSubscribe<FreqLevelItem>(
            @event,
            ClientIdString,
            WebSocketContext,
            (subInfo, webSocContext, data) =>
            {
                var context = (WebSocketContext)webSocContext;
                var result = SocketHandResult.FromStringSocketContent(data.Message);
                context.WebSocketConnection.Send(result);
            }
 );
            await FreqLevelPushEntry.PushOrStopItemAsync(taskId, (kv) => {
                return SubPubScheduler.Bloadcast(new SocketEventMessage<FreqLevelItem>(kv.Key, kv.Value));
            });
        }
    }
}