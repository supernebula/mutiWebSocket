using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSubscibe.SocketHandlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method , AllowMultiple = false)]
    public class WebSocketRouteAttribute : Attribute
    {
        public string Path { get; set; }
    }
} 