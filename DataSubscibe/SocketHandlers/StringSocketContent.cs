using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSubscibe.SocketHandlers
{
    public class StringSocketContent : SocketContent
    {
        public StringSocketContent(object content)
        {
            base.Type = SocketMessageType.String;
            Message = content;
        }
    }
}