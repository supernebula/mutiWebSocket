using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fleck;

namespace DataSubscibe.Core
{
    public class WebSocketContext
    {
        public IWebSocketConnection WebSocketConnection { get; set; }
    }
}