using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Example.Navigation
{
    public interface IViewModelActivator : IReactiveObject
    {
        bool IsActive { get; set; }
        
        IDisposable WhenActivated(Action<CompositeDisposable> block);
    }
}