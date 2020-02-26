using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;
using Xamarin.Forms;

namespace Example.Utils
{
    public static class DisposableUtils
    {
        /// <summary>
        /// This can be used to control button.IsEnabled when command binding is being disposed.
        /// This is useful when command has CanExecute evaluation, but when command binding is disposed the button becomes enabled anyway.
        /// This extension ensures that IsEnabled is set to the correct value of CanExecute after the command binding disposed.
        /// </summary>
        public static IDisposable ControlButtonEnabledWhenDisposed<TView, TViewModel>(
            this IReactiveBinding<TView, TViewModel, ReactiveCommand<Unit, Unit>> binding, ICommand command, Button button)
            where TView : class, IViewFor
            where TViewModel : class
        {
            var control = Disposable.Create(new { command, button }, state => state.button.IsEnabled = state.command.CanExecute(null));
            return new CompositeDisposable(binding, control);
        }
    }
}