using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DataSubscibe.Core.PublishSubscribe;

namespace DataSubscibe.Core.ServiceIntegration
{
    public class TimelineSocketSourceSample
    {
        private static TimelineSocketSourceSample _instance;

        public static TimelineSocketSourceSample Instance
        {
            get { return _instance ?? (_instance = new TimelineSocketSourceSample()); }
        }



        private Task _longTask;

        private CancellationTokenSource _stopTokenSource;

        public void Stop()
        {
            _stopTokenSource.Cancel();
        }

        public Task Start(IPublisher publisher)
        {
            _stopTokenSource = new CancellationTokenSource();
            return Start(publisher, _stopTokenSource);
        }

        public Task Start(IPublisher publisher, CancellationTokenSource cancelTokenSource)
        {
            //已取消或异常，丢弃
            if (_longTask != null &&
                (_longTask.Status == TaskStatus.RanToCompletion 
                || _longTask.Status == TaskStatus.Canceled 
                || _longTask.Status == TaskStatus.Faulted))
            {
                _longTask.Dispose();
                _longTask = null;
            }
            else if (_longTask != null) //正在运行(或将要运行的)返回
                return _longTask;

            var time = DateTime.Now;
            _longTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (cancelTokenSource.Token.IsCancellationRequested)
                    {
                        cancelTokenSource.Dispose();
                        cancelTokenSource = null;
                        break;
                    }
                    
                    Thread.Sleep(100);
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

                    var @event = "timeline";
                    //var @event = PushEvent.TimeLine;
                    var eventMessage = new SocketEventMessage<Timeline>(@event, item);
                    publisher.Publish(eventMessage).ContinueWith(t =>
                    {
                        if (!t.Result && cancelTokenSource != null) //发布失败，取消发布，并停止数据流
                            cancelTokenSource.Cancel();

                    });
                    time = time.AddDays(1);
                }
            }, cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return _longTask;
        }
    }
}