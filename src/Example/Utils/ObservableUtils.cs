using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace Example.Utils
{
    public static class ObservableUtils
    {
        /// <summary>
        /// Aggregates observables from <paramref name="collection"/> using selector for observables <paramref name="observablePropertySelector"/>
        /// and aggregator for values from observables <paramref name="aggregator"/>
        /// </summary>
        public static IObservable<TRes> AggregateLatest<T, TRes>(
            this ReadOnlyObservableCollection<T> collection, 
            Func<T, IObservable<TRes>> observablePropertySelector,
            Func<IList<TRes>, TRes> aggregator)
        {
            return Observable.Create<TRes>(observer =>
            {
                var aggregateLatestSubscription = collection.AggregateLatest(observablePropertySelector, aggregator, observer);
                var collectionChangesSubscription = collection.ObserveCollectionChanges().Subscribe(_ =>
                {
                    aggregateLatestSubscription.Dispose();
                    aggregateLatestSubscription = collection.AggregateLatest(observablePropertySelector, aggregator, observer);
                });
                
                return Disposable.Create(() =>
                {
                    collectionChangesSubscription.Dispose();
                    aggregateLatestSubscription.Dispose();
                });
            });
        }
        
        private static IDisposable AggregateLatest<T, TRes>(
            this ICollection<T> collection, 
            Func<T, IObservable<TRes>> observablePropertySelector,
            Func<IList<TRes>, TRes> aggregator,
            IObserver<TRes> observer)
        {
            if (collection.Count == 0)
            {
                observer.OnNext(default(TRes));
                return Disposable.Empty;
            }
            
            return collection
                .Select(observablePropertySelector)
                .CombineLatest()
                .Subscribe(values =>
                {
                    var aggregatedValue = aggregator(values);
                    observer.OnNext(aggregatedValue);
                });
        }

        /// <summary>
        /// Generates timer observable that updates every time when <paramref name="updateObservable"/> produces new value.
        /// It will produce values for intervals returned by <paramref name="timeSelector"/> (like in Observable.Generate).
        /// <paramref name="enableTimer"/> can be used to enable/disable timer after <paramref name="updateObservable"/> produces new value. 
        /// </summary>
        public static IObservable<Unit> GenerateTimer(Func<bool> enableTimer, Func<DateTime> timeSelector, IObservable<Unit> updateObservable, IScheduler scheduler)
        {
            return Observable.Create<Unit>(observer =>
            {
                IDisposable currentTimerDisposable = null;
                DateTime? nextTimer = null;
                var updateDisposable = updateObservable.Subscribe(__ =>
                {
                    currentTimerDisposable?.Dispose();
                    currentTimerDisposable = null;
                    
                    if (nextTimer.HasValue && nextTimer.Value <= scheduler.Now.LocalDateTime)
                    {
                        observer.OnNext(Unit.Default);
                    }

                    var enable = enableTimer();
                    if (!enable)
                    {
                        nextTimer = null;
                        return;
                    }
                    
                    currentTimerDisposable = Observable.Generate(
                        0L,
                        _ => true,
                        val => ++val,
                        val => val,
                        _ =>
                        {
                            nextTimer = timeSelector();
                            return nextTimer.Value;
                        },
                        scheduler)
                        .Subscribe(_ => observer.OnNext(Unit.Default));
                });

                return Disposable.Create(() =>
                {
                    currentTimerDisposable?.Dispose();
                    updateDisposable.Dispose();
                });
            });
        }
        
        /// <summary>
        /// Hack to make dynamic data return empty collection when it's empty by default
        /// </summary>
        /// <remarks>Don't forget to filter input items exactly as they are filtered in the dynamic data observable</remarks>
        public static IObservable<IReadOnlyCollection<T>> StartWithEmptyIfEmpty<T>(this IObservable<IReadOnlyCollection<T>> observable, int count)
        {
            return count > 0 ? observable : observable.StartWith(new List<T>().AsReadOnly());
        }
    }
}