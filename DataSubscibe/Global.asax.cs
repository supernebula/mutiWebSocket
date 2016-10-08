using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DataSubscibe.Core;
using Fleck;
using Newtonsoft.Json;

namespace DataSubscibe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private WebSocketServer _webSocketServer;
        private Task _longTask;
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
            var server = new WebSocketServer("ws://127.0.0.1:99/");
            FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
            var launchedSockets = new List<IWebSocketConnection>();
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine(message);
                    //allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                    var path = socket.ConnectionInfo.Path;
                    if (message == "launch")
                    {
                        if (!launchedSockets.Contains(socket))
                            launchedSockets.Add(socket);
                        PubSubScheduler.Instance.AddSubscribe<Timeline>(
                            "timeline",
                            socket.ConnectionInfo.Id.ToString().ToLower(),
                            (subscribe, subContext, msg) =>
                            {
                                var context = (WebSocketContext)subContext;
                                var content = JsonConvert.SerializeObject(msg.Message);

                                context.WebSocketConnection.Send(content);
                            },
                            new WebSocketContext() { WebSocketConnection = socket },
                            "有新消息"
                        );
                    }

                    if (message == "shutoff")
                    {
                        PubSubScheduler.Instance.RemoveSubscribe("timeline", socket.ConnectionInfo.Id.ToString().ToLower());
                        launchedSockets.Remove(socket);
                    }
                };
            });

            StartPipe();
            return server;
        }

        public void StartPipe()
        {
            var pipeSwitch = new CancellationTokenSource();
            EventMessageFactory.Start(PubSubScheduler.Instance, pipeSwitch.Token);
        }
    }
}
