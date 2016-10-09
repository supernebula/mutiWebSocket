using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DataSubscibe.Core;
using DataSubscibe.Core.PushEntrys;
using Newtonsoft.Json;

namespace DataSubscibe.SocketHandlers
{
    [WebSocketRoute(Path = "dynamicline")]
    public class DynamicLineHandler : SocketMessageHandler
    {
#if NoIOC
        public DynamicLineHandler()
        {
            TimelinePubEntry = new TimelinePublishEntry();
        }

#endif

        public TimelinePublishEntry TimelinePubEntry { get; set; }

        [WebSocketRoute(Path = "dynamicline/simple")]
        public Task ByYear(string message, string @event = "timeline")
        {
            if (message == "launch")
            {
                TimelinePubEntry.StartPublishTimeline(Publisher);

                AddSubscribe<Timeline>(
                    @event,
                    async (subscribe, subContext, msg) =>
                    {
                        var context = (WebSocketContext)subContext;
                        var result = SocketHandResult.FromStringSocketContent(msg.Message);
                        await context.WebSocketConnection.Send(result);
                    }
                );
            }

            if (message == "stop")
            {
                TimelinePubEntry.StopPublishTimeline();
                RemoveSubscribe(@event);
            }
            

            return Task.FromResult(1);
        }
    }
}