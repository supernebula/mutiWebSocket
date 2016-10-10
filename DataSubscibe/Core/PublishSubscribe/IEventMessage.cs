
namespace DataSubscibe.Core.PublishSubscribe
{
    public interface IEventMessage<out T> : IEventMessage
    {
        /// <summary>
        /// 要传递的事件消息
        /// </summary>
        T Message { get; }
    }

    public interface IEventMessage
    {
        /// <summary>
        /// 事件
        /// </summary>
        string Event { get; }
    }
}