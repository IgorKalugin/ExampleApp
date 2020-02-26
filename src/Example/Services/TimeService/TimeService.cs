using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Example.Services.LoggingService;
using Example.Utils;
using Splat;

namespace Example.Services.TimeService
{
    public class TimeService : ITimeService
    {
        public TimeService(IReadonlyDependencyResolver dr)
        {
            var scheduler = dr.GetService<IScheduler>() ?? DefaultScheduler.Instance;
            var logger = this.GetLogger(dr);
        }

        public IObservable<Unit> NextDay { get; }
    }
}