using System;
using System.Collections.Generic;
using Example.Pages;
using Example.ViewModels.Pages;
using Example.ViewModels.Pages.Profile;
using ReactiveUI;

namespace Example
{
    public class ExampleViewLocator : IViewLocator
    {
        private readonly Dictionary<Type, Func<IViewFor>> factories = new Dictionary<Type, Func<IViewFor>>
        {

			{ typeof(CreateProfileViewModel), () => new CreateProfilePage()},
            { typeof(LoginViewModel), () => new LoginPage()},
            
            { typeof(WebViewModel), () => new WebPage()},
        };
        
        public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class
        {
            var view = factories[viewModel.GetType()]();
            view.ViewModel = viewModel;
            return view;
        }
    }
}