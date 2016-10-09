using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSubscibe.SocketHandlers
{
    public abstract class SocketContent
    {
        public SocketMessageType Type { get; set; }

        public object Message { get; set; }
    }
}