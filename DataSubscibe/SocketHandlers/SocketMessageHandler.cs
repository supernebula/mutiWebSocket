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
        public SocketMessageHandler(IWebSocketConnection webSocketConnection)
        {
            WebSocketConnection = webSocketConnection;
        }
#endif

        public Guid ClientId {
            get
            {
                return WebSocketConnection.ConnectionInfo.Id;
            }
        }

        public string ClientIdString
        {
            get
            {
                return WebSocketConnection.ConnectionInfo.Id.ToString().ToLower();
            }
        }

        public IWebSocketConnection WebSocketConnection { get; set; }

        public WebSocketContext WebSocketContext
        {
            get
            {
                return new WebSocketContext()
                {
                    WebSocketConnection = WebSocketConnection
                };
            }

        }

        //protected IPublisher Publisher { get; set; }

        //protected ISubScheduler SubScheduler { get; set; }


        //public bool AddSubscribe<T>(string @event, Action<Subscribe<T>, object, IEventMessage<T>> method, string name = null)
        //{
        //    return SubScheduler.AddSubscribe<T>(
        //            @event,
        //            ClientId.ToString().ToLower(),
        //            method,
        //            WebSocketContext,
        //            name ?? "订阅了事件:" + @event
        //        );
        //}

        //public bool RemoveSubscribe(string @event)
        //{
        //    return SubScheduler.RemoveSubscribe(@event, ClientId.ToString().ToLower());
        //}
        
    }
}