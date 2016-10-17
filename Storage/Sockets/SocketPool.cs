using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Storage.Sockets
{
    public static class SocketPool
    {
        private static ConcurrentDictionary<string, KeyValuePair<CancellationTokenSource, Socket>> _socketCllection = new ConcurrentDictionary<string, KeyValuePair<CancellationTokenSource, Socket>>();

        public static bool TryAdd(string key, KeyValuePair<CancellationTokenSource, Socket> value)
        {
            return _socketCllection.TryAdd(key, value);
        }

        public static bool TryGetValue(string key, out KeyValuePair<CancellationTokenSource, Socket> value)
        {
            return _socketCllection.TryGetValue(key, out value);
        }

        public static bool TryRemove(string key, out KeyValuePair<CancellationTokenSource, Socket> value)
        {
            return _socketCllection.TryRemove(key, out value);
        }

    }
}
