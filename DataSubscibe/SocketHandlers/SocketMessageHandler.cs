using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataSubscibe.Core;
using Fleck;

namespace DataSubscibe.SocketHandlers
{
    public abstract class SocketMessageHandler
    {
#if NoIOC
        public SocketMessageHandler()
        {
            Publisher = PubSubScheduler.Instance;
        }

#endif

        protected Guid ClientId {
            get
            {
                return WebSocketContext.ConnectionInfo.Id;
            }
        }

        protected IWebSocketConnection WebSocketContext { get; set; }

        protected IPublisher Publisher { get; set; }

        protected ISubScheduler SubScheduler { get; set; }


        public bool AddSubscribe<T>(string @event, Action<Subscribe<T>, object, IEventMessage<T>> method, string name = null)
        {
            return SubScheduler.AddSubscribe<T>(
                    @event,
                    ClientId.ToString().ToLower(),
                    method,
                    new WebSocketContext() { WebSocketConnection = WebSocketContext },
                    name ?? "订阅了事件:" + @event
                );
        }

        public bool RemoveSubscribe(string @event)
        {
            return SubScheduler.RemoveSubscribe(@event, ClientId.ToString().ToLower());
        }
        
    }
}