using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DataSubscibe.SocketHandlers
{
    public class MessageHandlerProvider
    {
        private List<Type> FindMessageHandler(Assembly assembly)
        {
            Func<Type, bool> filter = type => type.IsPublic && !type.IsAbstract && type.IsClass && type.IsAssignableFrom(typeof(SocketMessageHandler));
            var impls = assembly.GetTypes().Where(filter).ToList();
            return impls;
        }

        public Dictionary<string, Type> FindHandlerAndRoute(Assembly assembly)
        {
            var routeTypeDic = new Dictionary<string, Type>();
            var handlerTypes = FindMessageHandler(assembly);
            foreach (var type in handlerTypes)
            {
                var attrs = type.GetCustomAttributes(typeof(WebSocketRouteAttribute), true).Cast<WebSocketRouteAttribute>();
                var methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    var  routeAttr = method.GetCustomAttribute(typeof(WebSocketRouteAttribute), true) as WebSocketRouteAttribute;
                    //routeTypeDic

                    //routeAttr.RoutePath;
                }

            }

            return routeTypeDic;
        }
    }
}