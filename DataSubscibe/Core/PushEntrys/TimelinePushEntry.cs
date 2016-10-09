using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataSubscibe.Core.PushEntrys
{
    public class TimelinePublishEntry
    {

        public async Task StartPublishTimeline(IPublisher publisher)
        {
            await TimelineSocketSourceSample.Instance.Start(publisher);
        }

        public Task StopPublishTimeline()
        {
            throw new NotImplementedException();
        }


    }
}