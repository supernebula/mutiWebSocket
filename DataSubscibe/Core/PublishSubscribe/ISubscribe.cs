
namespace DataSubscibe.Core.PublishSubscribe
{
    /// <summary>
    /// 订阅接口
    /// </summary>
    public interface ISubscribe : ISubscribeInfo
    {
        /// <summary>
        /// 订阅是否取消
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// 取消订阅
        /// </summary>
        void Cancel();


        /// <summary>
        /// 订阅的事件触发（即广播）时响应事件的回调方法
        /// </summary>
        /// <param name="eventMessage"></param>
        void OnPublish(IEventMessage eventMessage);
    }

    public interface ISubscribeInfo
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        string Event { get; }

        /// <summary>
        /// 订阅者Id
        /// </summary>
        string Subscriber { get; }

        /// <summary>
        /// 订阅的名称
        /// </summary>
        string Name { get; }
    }
}
