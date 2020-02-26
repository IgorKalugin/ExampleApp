using System;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace Example.Navigation
{
    public class NavigationViewHost : NavigationPage
    {
        private readonly INavigationState navigationState;
        private readonly IViewLocator viewLocator;

        public NavigationViewHost(IReadonlyDependencyResolver dr)
        {
            navigationState = dr.GetService<INavigationState>();
            viewLocator = dr.GetService<IViewLocator>();
            Setup();
        }

        private void Setup()
        {
            // The NavigationViewHost should be a singleton as it doesn't unsubscribe from navigation state changes
            // In the future we can extend it and add disposing when it is disappearing
            this.WhenAnyObservable(view => view.navigationState.NavigationChanged)
                .Subscribe(changes =>
                {
                    var currentViewModel = navigationState.CurrentViewModel;
                    if (changes.TotalChanges == 1)
                    {
                        if (changes.Adds == 1)
                        {
                            var view = viewLocator.ResolveView(currentViewModel);
                            view.ViewModel = currentViewModel;
                            
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            var page = (Page)view;
                            PushAsync(page, true);
                            return;
                        }

                        if (changes.Removes == 1)
                        {
                            PopAsync(true);
                            return;
                        }
                    }

                    if (navigationState.NavigationStack.Count == 1)
                    {
                        var view = viewLocator.ResolveView(currentViewModel);
                        view.ViewModel = currentViewModel;
                        
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        var page = (Page)view;
                        Navigation.InsertPageBefore(page, Navigation.NavigationStack[0]);
                        PopToRootAsync(true);
                        return;
                    }
                    
                    throw new NotSupportedException("This type of navigation is not supported");
                });
        }
    }
}