using System;

namespace DataSubscibe.Core.PublishSubscribe
{
    public interface ISubScheduler
    {
        bool AddSubscribe<T>(string @event, string subscriber, Action<Subscribe<T>, object, IEventMessage<T>> method, object subContext,
            string name);

        bool AddSubscribe<T>(string @event, string subscriber, Action<Subscribe<T>, object, IEventMessage<T>> method, string name);

        bool RemoveSubscribe(string @event, string subscriber);

        bool AddSubscribe<T>(Subscribe<T> subscribe);

        bool IsExistedSubscribe(string @event, string subscriber);
    }
}
