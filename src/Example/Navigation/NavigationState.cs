using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Example.Navigation
{
    /// <summary>
    /// Represent hierarchical navigation inside the app. It should be a singleton
    /// </summary>
    public class NavigationState : ReactiveObject, INavigationState
    {
        private readonly ObservableCollectionExtended<INavigationViewModel> navigationStack = new ObservableCollectionExtended<INavigationViewModel>();
        
        public NavigationState()
        {
            Setup();
        }

        public ObservableCollection<INavigationViewModel> NavigationStack => navigationStack;
        public IObservable<IChangeSet<INavigationViewModel>> NavigationChanged { get; private set; }
        public INavigationViewModel CurrentViewModel => NavigationStack.LastOrDefault();
        
        public void NavigateBack()
        {
            if (NavigationStack.Count > 0)
            {
                navigationStack.RemoveAt(NavigationStack.Count - 1);
            }
        }

        public void Navigate(INavigationViewModel vm)
        {
            navigationStack.Add(vm);
        }

        public void NavigateAndReset(INavigationViewModel vm)
        {
            using (navigationStack.SuspendNotifications())
            {
                navigationStack.Clear();
                Navigate(vm);
            }
        }

        private void Setup()
        {
            NavigationChanged = navigationStack.ToObservableChangeSet();
            
            NavigationChanged
                .Select(_ => CurrentViewModel)
                .Scan(new { Previous = (INavigationViewModel)null, Current = (INavigationViewModel)null },
                (acc, current) => new { Previous = acc.Current, Current = current })
                .Subscribe(pair =>
                {
                    if (pair.Previous != null)
                    {
                        pair.Previous.Activator.IsActive = false;
                    }

                    if (pair.Current != null)
                    {
                        pair.Current.Activator.IsActive = true;
                    }
                });
        }
    }
}