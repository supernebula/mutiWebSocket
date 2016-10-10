using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DataSubscibe.Core.PublishSubscribe;
using DataSubscibe.SocketHandlers;
using Fleck;

namespace DataSubscibe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private WebSocketServer _webSocketServer;
        private List<IWebSocketConnection> _allSockets;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            _webSocketServer = ConfigureWebSocket();
        }

        public WebSocketServer ConfigureWebSocket()
        {
            var server = new WebSocketServer("ws://192.168.1.62:99/");
            FleckLog.Level = LogLevel.Debug;
            _allSockets = new List<IWebSocketConnection>();
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    _allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    PubSubScheduler.Instance.RemoveSubscribe(socket.ConnectionInfo.Id);
                    _allSockets.Remove(socket);
                    Debug.WriteLine("Close!");
                    
                };
                socket.OnError = (ex) =>
                {
                    PubSubScheduler.Instance.RemoveSubscribe(socket.ConnectionInfo.Id);
                    _allSockets.Remove(socket);
                    Debug.WriteLine(ex.Message);
                };

                socket.OnPing = (bytes) =>
                {
                    
                };

                socket.OnPong = (bytes) =>
                {

                };

                socket.OnMessage = message =>
                {
                    Debug.WriteLine(message);
                    var path = socket.ConnectionInfo.Path;
                    if (path.Equals("/dynamicline/time", StringComparison.CurrentCultureIgnoreCase)) //最终采用路由方式
                    {
                        var handler = new DynamicLineHandler(socket)
                        {
                            WebSocketConnection = socket 
                        };
                        handler.ByTime(message);
                    }
                };
            });

            return server;
        }
    }
}
