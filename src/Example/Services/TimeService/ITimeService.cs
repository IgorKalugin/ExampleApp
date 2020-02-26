using System;
using System.Reactive;

namespace Example.Services.TimeService
{
    public interface ITimeService
    {
        IObservable<Unit> NextDay { get; }
    }
}