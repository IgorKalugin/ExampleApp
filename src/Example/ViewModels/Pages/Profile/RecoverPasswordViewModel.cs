using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Example.Services.AuthService;
using Example.Services.LoggingService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Example.ViewModels.Pages.Profile
{
    public class RecoverPasswordViewModel : PageViewModel
    {
        private readonly IReadonlyDependencyResolver dr;
        private readonly IAuthService authService;
        
        public RecoverPasswordViewModel(IReadonlyDependencyResolver dr) : base(dr)
        {
            this.dr = dr;
            authService = dr.GetService<IAuthService>();
            LoginCommand = ReactiveCommand.Create(Login).LogInvocation(nameof(LoginCommand), Logger, dr);
            RecoverCommand = ReactiveCommand.CreateFromTask(Recover, CanRecover()).LogInvocation(nameof(RecoverCommand), Logger, dr);
            
            var timeScheduler = dr.GetService<IScheduler>() ?? DefaultScheduler.Instance;
            this.LogPropertyChangeThrottling(vm => vm.Email, nameof(Email), scheduler: timeScheduler);
        }
        
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        [Reactive] public string Email { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        
        // we set error text to " " and not to string.Empty or null, otherwise its Height is 0 and when it appears everything goes up a little bit
        [Reactive] public string Error { get; private set; } = " ";
        
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> RecoverCommand { get; }

        private void Login()
        {
            NavigationState.NavigateAndReset(new LoginViewModel(dr));
        }

        private async Task Recover()
        {
            Error = " ";
            
            var ok = await authService.Recover(Email);
            if (!ok)
            {
                Logger.Value.Debug("Password recovery failed");
                Error = "Something went wrong. Please try again.";
                return;
            }
            
            NavigationState.NavigateAndReset(new LoginViewModel(dr, "We have sent you an email with a password reset link. Please reset the password and come back to the app to log in."));
        }

        private IObservable<bool> CanRecover() =>
            this.WhenAnyValue(vm => vm.Email, email => !string.IsNullOrWhiteSpace(email));
    }
}