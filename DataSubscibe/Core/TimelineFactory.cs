using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataSubscibe.Core
{
    public class TimelineSocketSourceSample
    {
        private static TimelineSocketSourceSample _instance;

        public static TimelineSocketSourceSample Instance
        {
            get { return _instance ?? (_instance = new TimelineSocketSourceSample()
            {
                _stopTokenSource = new CancellationTokenSource()
            }); }
        }



        private Task _longTask;

        private CancellationTokenSource _stopTokenSource;

        public void Stop()
        {
            _stopTokenSource.Cancel();
        }

        public Task Start(IPublisher publisher)
        {
            return Start(publisher, _stopTokenSource.Token);
        }

        public async Task Start(IPublisher publisher, CancellationToken cancelToken)
        {
            var time = DateTime.Now;
            _longTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                        break;
                    Thread.Sleep(50);
                    var item = new Timeline()
                    {
                        Name = time.ToString(CultureInfo.InvariantCulture),
                        Value = new string[2]
                    };
                    item.Value[0] = string.Format("{0}/{1}/{2}", time.Year, time.Month, time.Day);

                    var dayRadom = (new Random(Guid.NewGuid().GetHashCode()).Next(0, 50)) + 10;
                    var mRadom = (new Random(Guid.NewGuid().GetHashCode())).Next(0, 20) + 100 * (time.Month % 12);
                    var y = time.Year % 3 * 1000;
                    item.Value[1] = (mRadom + dayRadom + y).ToString();
                    var eventMessage = new SocketEventMessage<Timeline>("timeline", item);
                    publisher.Publish(eventMessage);
                    time = time.AddDays(1);
                }
            }, cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            await _longTask;
        }
    }
}