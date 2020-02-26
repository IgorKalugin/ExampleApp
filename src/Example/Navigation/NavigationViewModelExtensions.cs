using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace Example.Navigation
{
    public static class NavigationViewModelExtensions
    {
        /// <summary>
        /// Executes <paramref name="block"/> when ViewModel is activated
        /// </summary>
        /// <remarks>ViewModel shouldn't outlive the <paramref name="view"/>, because this method subscribes to ViewModel change.
        /// We need that subscription because of ListView cells reuse
        /// </remarks>
        public static IDisposable WhenViewModelActivated<T>(this IViewFor<T> view, Action<CompositeDisposable> block) 
            where T : class, IHaveActivator, IReactiveObject
        {
            IDisposable activated = null; 
            
            var vmChange = view.WhenAnyValue(v => v.ViewModel)
                // ViewModel can be changed for ListView cells because of their reuse
                .DistinctUntilChanged()
                .Subscribe(vm =>
                {
                    activated?.Dispose();
                    activated = vm?.Activator.WhenActivated(block);
                });
            
            return Disposable.Create(() =>
            {
                activated?.Dispose();
                vmChange.Dispose();
            });
        }

        public static IObservable<IChangeSet<T>> HandleActivation<T>(this IObservable<IChangeSet<T>> observable)
            where T : IHaveActivator
        {
            return observable.OnItemRemoved(item => item.Activator.IsActive = false)
                .OnItemAdded(item => item.Activator.IsActive = true);
        }
        
        public static IObservable<IChangeSet<T, TKey>> HandleActivation<T, TKey>(this IObservable<IChangeSet<T, TKey>> observable)
            where T : IHaveActivator
        {
            return observable.OnItemRemoved(item => item.Activator.IsActive = false)
                .OnItemAdded(item => item.Activator.IsActive = true);
        }

        public static void HandleActivation(this IHaveActivator vm, params IHaveActivator[] children)
        {
            vm.Activator.WhenActivated(d =>
            {
                foreach (var child in children)
                {
                    child.Activator.IsActive = true;
                }

                Disposable.Create(children, childrenToDeactivate =>
                {
                    foreach (var child in childrenToDeactivate)
                    {
                        child.Activator.IsActive = false;
                    }
                }).DisposeWith(d);
            });
        }
    }
}