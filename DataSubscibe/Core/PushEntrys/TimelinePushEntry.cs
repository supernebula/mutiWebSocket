using System;
using System.Threading.Tasks;
using DataSubscibe.Core.Domain.Models;
using DataSubscibe.Core.PublishSubscribe;
using DataSubscibe.Core.ServiceIntegration;

namespace DataSubscibe.Core.PushEntrys
{
    public class TimelinePushEntry
    {
#if NoIOC
        public TimelinePushEntry(ISubScheduler subScheduler, IPublisher publisher)
        {
            SubScheduler = subScheduler;
            Publisher = publisher;
        }
#endif

        public ISubScheduler SubScheduler { get; set; }

        public IPublisher Publisher { get; set; }

        /// <summary>
        /// Timeline推送入口
        /// </summary>
        /// <param name="socketClientId"></param>
        /// <param name="webSocketContext"></param>
        /// <param name="onDataCallback"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task PushTimeline(string socketClientId,  WebSocketContext webSocketContext, Action<Subscribe<Timeline>, object, IEventMessage<Timeline>> onDataCallback, string name = null)
        {
           
            var @event = "timeline";
            //var @event = PushEvent.TimeLine;

            SubScheduler.AddSubscribe(
                    @event,
                    socketClientId,
                    onDataCallback,
                    webSocketContext,
                    name ?? "订阅了事件:" + @event
             );

            TimelineSocketSourceSample.Instance.Start(Publisher); //Demo,这里是本地Socket客户端与远程服务器连接并接受消息的入口， Start开始从Socket服务器端接受数据
            return Task.FromResult(1);
        }

        /// <summary>
        /// 取消Timeline推送
        /// </summary>
        /// <param name="socketClientId"></param>
        /// <returns></returns>
        public Task CancelPushTaskTimeline(string socketClientId)
        {
            var @event = "timeline";

            SubScheduler.RemoveSubscribe(@event, socketClientId);
            if (!SubScheduler.IsExistedSubscribe(@event, socketClientId))
            {
                TimelineSocketSourceSample.Instance.Stop();
            }
            return Task.FromResult(1);
        }
    }
}