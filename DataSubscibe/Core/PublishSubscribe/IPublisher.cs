
namespace DataSubscibe.Core.PublishSubscribe
{
    public interface IPublisher
    {
        void Publish(IEventMessage message);
    }
}
