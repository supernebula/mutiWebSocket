
using System.Threading.Tasks;

namespace DataSubscibe.Core.PublishSubscribe
{
    public interface IPublisher
    {
        Task<bool> Bloadcast(IEventMessage message);
    }
}
