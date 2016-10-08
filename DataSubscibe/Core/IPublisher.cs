using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSubscibe.Core
{
    public interface IPublisher
    {
        void Publish(IEventMessage message);
    }
}
