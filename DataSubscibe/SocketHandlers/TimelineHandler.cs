using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataSubscibe.Core;
using DataSubscibe.Core.PublishSubscribe;
using DataSubscibe.Core.PushEntrys;
using Fleck;
using Newtonsoft.Json;

namespace DataSubscibe.SocketHandlers
{
    [WebSocketRoute(Path = "dynamicline")]
    public class DynamicLineHandler : SocketMessageHandler
    {
#if NoIOC
        public DynamicLineHandler(IWebSocketConnection webSocketConnection)
            : base(webSocketConnection)
        {
            TimelinePubEntry = new TimelinePushEntry(PubSubScheduler.Instance);
        }

#endif
        /// <summary>
        /// Ioc
        /// </summary>
        public TimelinePushEntry TimelinePubEntry { get; set; }

        [WebSocketRoute(Path = "dynamicline/time")]
        public Task ByTime(string message)
        {
            if (message == "launch")
            {
                TimelinePubEntry.PushTimeline(ClientIdString, WebSocketContext,
                    (subscribe, webSocContext, data) =>
                    {
                        var context = (WebSocketContext) webSocContext;
                        var result = SocketHandResult.FromStringSocketContent(data.Message);
                        context.WebSocketConnection.Send(result);
                    });
            }

            if (message == "stop")
            {
                TimelinePubEntry.CancelPushTaskTimeline(ClientIdString);
            }
            

            return Task.FromResult(1);
        }
    }
}