using System;
using System.Diagnostics;

namespace Example.Utils.MemoryLeaks
{
    public class MemoryLeakActivationTracker : IDisposable
    {
        private readonly int number;
        private readonly Type type;

        public MemoryLeakActivationTracker(int number, object element)
        {
            this.number = number;
            type = element.GetType();
            ElementRef = new WeakReference(element);
        }

        public WeakReference ElementRef { get; }

        public int ActivateCounter { get; private set; }

        public void Activate()
        {
            ++ActivateCounter;
            if (MemoryLeaksHunter.EnableLog)
            {
                Debug.WriteLine($"Watcher for type {type.Name} #{number} has been Activated, ActiveCounter: {ActivateCounter}");
            }
        }

        public void Dispose()
        {
            --ActivateCounter;
            if (MemoryLeaksHunter.EnableLog)
            {
                Debug.WriteLine($"Watcher for type {type.Name} #{number} has been Disposed, ActiveCounter: {ActivateCounter}");
            }
        }

        public override string ToString()
        {
            return $"Watcher for type {type.Name} #{number} was Activated without disposing {ActivateCounter} times";
        }
    }
}