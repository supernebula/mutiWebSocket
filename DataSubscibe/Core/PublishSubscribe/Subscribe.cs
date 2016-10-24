using System;

namespace DataSubscibe.Core.PublishSubscribe
{
    public class Subscribe<T> : ISubscribe
    {
        public string Event { get; set; }

        public string Name { get; set; }

        public object SubContext { get; set; }

        public Action<ISubscribeInfo, object, IEventMessage<T>> OnPublishFunc { get; set; }

        public void OnPublish(IEventMessage eventMessage)
        {
            
            if(OnPublishFunc == null)
                throw new NullReferenceException("OnPublishFunc 不能为NULL");
            var message = (IEventMessage<T>)eventMessage;
            OnPublishFunc.Invoke(this, SubContext, message);
        }


        public string Subscriber { get; set; }

        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }
}