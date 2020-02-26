using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using Splat;

namespace Example.Utils.MemoryLeaks
{
    public static class MemoryLeaksHunter
    {
        private static readonly Dictionary<Type, int> counters = new Dictionary<Type, int>();
        private static readonly List<MemoryLeakActivationTracker> trackers = new List<MemoryLeakActivationTracker>();
        
        public static bool EnableLog { get; set; } = true;
        
        [Conditional("DEBUG")]
        public static void TrackActivation<T>(T element, CompositeDisposable compositeDisposable)
        {
            if (ModeDetector.InUnitTestRunner())
            {
                return;
            }
            
            var tracker = GetTracker(element);
            tracker.DisposeWith(compositeDisposable);
            tracker.Activate();
        }
        
        public static void Report()
        {
            Debug.WriteLine("==========================");
            var activeTrackers = GetActiveTrackers();
            Debug.WriteLine($"MemoryLeakHunter report has {activeTrackers.Count} items:");
            foreach (var tracker in activeTrackers)
            {
                Debug.WriteLine(tracker.ToString());
            }
            Debug.WriteLine("==========================");
        }
        
        private static MemoryLeakActivationTracker GetTracker<T>(T element)
        {
            for (var i = 0; i < trackers.Count; ++i)
            {
                var tracker = trackers[i];
                if (!tracker.ElementRef.IsAlive && tracker.ActivateCounter <= 0)
                {
                    // cleaning trackers
                    trackers.RemoveAt(i);
                }

                if (ReferenceEquals(tracker.ElementRef.Target, element))
                {
                    return tracker;
                }
            }

            var newTracker = CreateTracker(element);
            trackers.Add(newTracker);
            return newTracker;
        }

        private static MemoryLeakActivationTracker CreateTracker<T>(T element)
        {
            if (!counters.TryGetValue(typeof(T), out var counter))
            {
                counter = 0;
            }
            
            var tracker = new MemoryLeakActivationTracker(++counter, element);
            counters[typeof(T)] = counter;
            
            return tracker;
        }

        private static List<MemoryLeakActivationTracker> GetActiveTrackers()
        {
            var activeTrackers = new List<MemoryLeakActivationTracker>();
            for (var i = 0; i < trackers.Count; ++i)
            {
                var tracker = trackers[i];
                if (!tracker.ElementRef.IsAlive && tracker.ActivateCounter <= 0)
                {
                    // cleaning trackers
                    trackers.RemoveAt(i);
                    continue;
                }

                if (tracker.ActivateCounter > 0)
                {
                    activeTrackers.Add(tracker);
                }
            }

            return activeTrackers;
        }
    }
}