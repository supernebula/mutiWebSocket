using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataSubscibe.SocketHandlers
{
    public class SocketHandResult
    {
        public bool IsSuccessStatus { get; set; }

        public string ReasonPhrase { get; set; }

        public int StatusCode { get; set; }

        public SocketContent Content { get; set; }

        public static SocketHandResult FromStringSocketContent(object message)
        {
            var result = new SocketHandResult()
            {
                IsSuccessStatus = true,
                ReasonPhrase = String.Empty,
                StatusCode = 200,
                Content = new StringSocketContent(message)
            };
            return result;
        }
    }
}