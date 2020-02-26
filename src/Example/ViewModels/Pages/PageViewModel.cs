using System;
using System.Reactive;
using Example.Navigation;
using Example.Services.LoggingService;
using ReactiveUI;
using Splat;
using ILogger = Example.Services.LoggingService.ILogger;
using ViewModelActivator = Example.Navigation.ViewModelActivator;

namespace Example.ViewModels.Pages
{
    public class PageViewModel : ReactiveObject, INavigationViewModel
    {
        protected PageViewModel(IReadonlyDependencyResolver dr)
        {
            Logger = this.GetLogger(dr, GetType());
            Activator = new ViewModelActivator();
            NavigationState = dr.GetService<INavigationState>();
            NavigateBack = ReactiveCommand.Create(NavigationState.NavigateBack).LogInvocation(nameof(NavigationState.NavigateBack), Logger, dr);
        }
        
        public Lazy<ILogger> Logger { get; }

        public IViewModelActivator Activator { get; }

        protected INavigationState NavigationState { get; set; }
        
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }
    }
}