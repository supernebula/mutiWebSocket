using System;
using System.Threading.Tasks;

namespace DataSubscibe.Core.PublishSubscribe
{
    public interface ISubPubScheduler : IPublisher
    {
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
        bool AddSubscribe<T>(string @event, string subscriber, object subContext, Action<Subscribe<T>, object, IEventMessage<T>> method, string name = null);

        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <typeparam name="T">要订阅的数据对象的类型</typeparam>
        /// <param name="event">要订阅的事件</param>
        /// <param name="subscriber">订阅者Id</param>
        /// <param name="method">订阅的事件触发（即广播）时的回调方法</param>
        /// <param name="name">友好名称</param>
        /// <returns></returns>
        bool AddSubscribe<T>(string @event, string subscriber, Action<Subscribe<T>, object, IEventMessage<T>> method, string name = null);

        /// <summary>
        /// 移除订阅
        /// </summary>
        /// <param name="event"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        bool RemoveSubscribe(string @event, string subscriber);

        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscribe"></param>
        /// <returns></returns>

        bool AddSubscribe<T>(Subscribe<T> subscribe);

        /// <summary>
        /// 判断订阅者是否已经订阅了事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <param name="subscriber">订阅者Id</param>
        /// <returns></returns>
        bool IsExistedSubscribe(string @event, string subscriber);

    }
}
