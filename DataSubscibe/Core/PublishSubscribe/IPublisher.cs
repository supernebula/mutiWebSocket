
using System.Threading.Tasks;

namespace DataSubscibe.Core.PublishSubscribe
{
    public interface IPublisher
    {
        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> Bloadcast<T>(IEventMessage<T> message);
    }
}
