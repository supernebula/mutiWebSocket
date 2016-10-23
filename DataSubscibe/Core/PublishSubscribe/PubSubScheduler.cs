using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGrease.Css.Extensions;

namespace DataSubscibe.Core.PublishSubscribe
{
    public class PubSubScheduler : ISubPubScheduler //IPublisher
    {
        private static PubSubScheduler _instance;

        public static PubSubScheduler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PubSubScheduler();
                return _instance;
            }
        }

        /// <summary>
        /// 订阅列表
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentDictionary<string, ISubscribe>> _eventSubsList =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, ISubscribe>>();

        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <typeparam name="T">要订阅的数据对象的类型</typeparam>
        /// <param name="event">要订阅的事件</param>
        /// <param name="subscriber">订阅者Id</param>
        /// <param name="subContext">订阅时的上下文对象</param>
        /// <param name="method">订阅的事件触发（即广播）时的回调方法</param>
        /// <param name="name">友好名称</param>
        /// <returns></returns>
        public bool AddSubscribe<T>(string @event, string subscriber, object subContext, Action<Subscribe<T>, object, IEventMessage<T>> method,  string name = null)
        {
            var subscribe = new Subscribe<T>()
            {
                Event = @event,
                Subscriber = subscriber,
                Name = name,
                OnPublishFunc = method,
                SubContext = subContext
            };

            return AddSubscribe(subscribe);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">要订阅的数据对象的类型</typeparam>
        /// <param name="event">要订阅的事件</param>
        /// <param name="subscriber">订阅者Id</param>
        /// <param name="method">订阅的事件触发（即广播）时响应事件的回调方法</param>
        /// <param name="name">友好名称</param>
        /// <returns></returns>
        public bool AddSubscribe<T>(string @event, string subscriber, Action<Subscribe<T>, object, IEventMessage<T>> method, string name = null)
        {
            return AddSubscribe<T>(@event, subscriber, default(object), method, name);
        }



        public bool AddSubscribe<T>(Subscribe<T> subscribe)
        {
            if (subscribe == null)
                return false;
            if (IsExistedSubscribe(subscribe.Event, subscribe.Subscriber)) //已存在的有效订阅，不会重复订阅
                return true;

            ConcurrentDictionary<string, ISubscribe> subDic;
            if (!_eventSubsList.ContainsKey(subscribe.Event))
            {
                subDic = new ConcurrentDictionary<string, ISubscribe>();
                if (_eventSubsList.TryAdd(subscribe.Event, subDic))
                    return false;
            }
            if (!_eventSubsList.TryGetValue(subscribe.Event, out subDic) || subDic == null)
                return false;
            return subDic.TryAdd(subscribe.Subscriber, subscribe);
        }

        public bool IsExistedSubscribe(string @event, string subscriber)
        {
            ConcurrentDictionary<string, ISubscribe> subDic;
            if (!_eventSubsList.TryGetValue(@event, out subDic) || subDic == null || subDic.IsEmpty)
                return false;
            ISubscribe subscribe;
            return (subDic.TryGetValue(subscriber, out subscribe) && subscribe != null && !subscribe.IsCanceled);
        }

        public bool RemoveSubscribe(string @event, string subscriber)
        {
            ConcurrentDictionary<string, ISubscribe> subDic;
            if (!_eventSubsList.TryGetValue(@event, out subDic))
                return false;
            ISubscribe sub;
            if (!subDic.TryGetValue(subscriber, out sub))
                return false;
            if (sub == null)
                return false;
            sub.Cancel();

            ISubscribe sub2;
            return subDic.TryRemove(sub.Subscriber, out sub2);
        }

        public void RemoveSubscribe(Guid subscriber)
        {
            RemoveSubscribe(subscriber.ToString().ToLower());
        }

        public void RemoveSubscribe(string subscriber)
        {
            var deleteEvents = new List<string>();
            foreach (var subDic in _eventSubsList.Values)
            {
                if (subDic.Values.All(e => e.Subscriber != subscriber))
                    continue;
                ISubscribe subscribe, subscribe2;
                if (subDic.TryGetValue(subscriber, out subscribe))
                    subscribe.Cancel();
                subDic.TryRemove(subscriber, out subscribe2);
                if (!subDic.Any() && subscribe != null)
                {
                    ConcurrentDictionary<string, ISubscribe> temp;
                    _eventSubsList.TryRemove(subscribe.Event, out temp);
                }
            }
        }

        public Task<bool> Bloadcast<T>(IEventMessage<T> message)
        {
            if (!_eventSubsList.ContainsKey(message.Event))
                return Task.FromResult(false);
            ConcurrentDictionary<string, ISubscribe> subDic;
            if (!_eventSubsList.TryGetValue(message.Event, out subDic) || subDic == null || subDic.IsEmpty)
                return Task.FromResult(false);

            Task.Run(() =>
            {
                try
                {
                    //广播
                    subDic.Values.ForEach(s =>
                    {
                        s.OnPublish(message);
                    });
                }
                catch (Exception)
                {
                    //log
                }
            });
            return Task.FromResult(true);
        }
    }


}