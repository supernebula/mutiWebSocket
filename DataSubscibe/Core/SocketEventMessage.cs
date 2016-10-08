using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSubscibe.Core
{
    public class SocketEventMessage<T> : IEventMessage<T>
    {
        public SocketEventMessage(string @event, T message)
        {
            Event = @event;
            Message = message;
        }

        public string Event { get; private set; }

        public T Message { get; private set; }

        public bool IsCanceled { get; private set; }
    }
}