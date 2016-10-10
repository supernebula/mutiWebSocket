
namespace DataSubscibe.Core.PublishSubscribe
{
    /// <summary>
    /// 订阅接口
    /// </summary>
    public interface ISubscribe
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        string Event { get;}

        /// <summary>
        /// 订阅者
        /// </summary>
        string Subscriber { get; }

        /// <summary>
        /// 订阅的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 订阅是否取消
        /// </summary>
        bool IsCanceled { get; }

        void Cancel();


        /// <summary>
        /// 响应发布事件的方法
        /// </summary>
        /// <param name="eventMessage"></param>
        void OnPublish(IEventMessage eventMessage);


    }
}
