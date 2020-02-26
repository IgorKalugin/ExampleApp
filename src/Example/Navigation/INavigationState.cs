using System;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;

namespace Example.Navigation
{
    public interface INavigationState : IReactiveObject
    {
        ObservableCollection<INavigationViewModel> NavigationStack { get; }
        
        IObservable<IChangeSet<INavigationViewModel>> NavigationChanged { get; }
        
        INavigationViewModel CurrentViewModel { get; }
        
        void NavigateBack();

        void Navigate(INavigationViewModel vm);

        void NavigateAndReset(INavigationViewModel vm);
    }
}