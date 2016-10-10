using System;
using System.Collections.Concurrent;
using WebGrease.Css.Extensions;

namespace DataSubscibe.Core.PublishSubscribe
{
    public class PubSubScheduler : IPublisher, ISubScheduler
    {
        private static PubSubScheduler _instance;
        public static PubSubScheduler Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new PubSubScheduler();
                return _instance;
            }
        }

        /// <summary>
        /// 订阅列表
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentDictionary<string, ISubscribe>> _eventSubsList = new ConcurrentDictionary<string, ConcurrentDictionary<string, ISubscribe>>();
        public bool AddSubscribe<T>(string @event, string subscriber, Action<Subscribe<T>, object, IEventMessage<T>> method, object subContext, string name)
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

        public bool AddSubscribe<T>(string @event, string subscriber, Action<Subscribe<T>, object, IEventMessage<T>> method, string name)
        {
            return AddSubscribe(@event, subscriber, method, default(object), name);
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
            if(!subDic.TryGetValue(subscriber, out sub))
                return false;
            if(sub == null)
                return false;
            sub.Cancel();

            ISubscribe sub2;
            return subDic.TryRemove(sub.Subscriber, out sub2);
        }

        

        public void Publish(IEventMessage message)
        {
            if (!_eventSubsList.ContainsKey(message.Event))
                return;
            ConcurrentDictionary<string, ISubscribe> subDic;
            if (!_eventSubsList.TryGetValue(message.Event, out subDic) || subDic == null || subDic.IsEmpty)
                return;

            //广播
            subDic.Values.ForEach(s =>
            {
                s.OnPublish(message);
            });
        }
    }
}