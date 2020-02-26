using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Utils
{
    public static class SemaphoreSlimExtensions
    {
        public static async Task<IDisposable> UseWaitAsync(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return Disposable.Create(() => semaphore.Release());
        }
    }
}