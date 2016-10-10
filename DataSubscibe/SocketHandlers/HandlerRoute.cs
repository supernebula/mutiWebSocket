using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSubscibe.SocketHandlers
{
    public class HandlerRoute
    {
        public string Path { get; set; }

        public string Handler { get; set; }

        public string Method { get; set; }
    }
}