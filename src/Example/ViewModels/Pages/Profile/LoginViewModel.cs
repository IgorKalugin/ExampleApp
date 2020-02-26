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
    public class LoginViewModel : PageViewModel
    {
        private readonly IReadonlyDependencyResolver dr;

        public LoginViewModel(IReadonlyDependencyResolver dr, string description = null) : base(dr)
        {
            this.dr = dr;
            Description = description;

            LoginCommand = ReactiveCommand.CreateFromTask(Login, CanLogin()).LogInvocation(nameof(LoginCommand), Logger, dr);
            RecoverPasswordCommand = ReactiveCommand.Create(Recover).LogInvocation(nameof(RecoverPasswordCommand), Logger, dr);
            CreateCommand = ReactiveCommand.Create(Create).LogInvocation(nameof(CreateCommand), Logger, dr);

            var timeScheduler = dr.GetService<IScheduler>() ?? DefaultScheduler.Instance;
            this.LogPropertyChangeThrottling(vm => vm.Email, nameof(Email), scheduler: timeScheduler);
            this.LogPropertyChangeThrottling(vm => vm.Password, nameof(Password), password => "***", timeScheduler);
        }

        [Reactive] public string Email { get; set; }
        [Reactive] public string Password { get; set; }
        
        // we set error text to " " and not to string.Empty or null, otherwise its Height is 0 and when it appears everything goes up a little bit
        [Reactive] public string Error { get; private set; } = " ";
        
        public string Description { get; }
        
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> RecoverPasswordCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }

        private async Task Login()
        {
            Error = " ";

            var authService = dr.GetService<IAuthService>();
            var ok = await authService.Login(Email, Password);
            if (!ok)
            {
                Logger.Value.Debug("Login failed");
                Error = @"Something went wrong. Please try again.";
                return;
            }

            // ki. Add alert
        }

        private IObservable<bool> CanLogin()
        {
            return this.WhenAnyValue(vm => vm.Email, vm => vm.Password,
                (email, password) => !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(Password));
        }

        private void Recover()
        {
            NavigationState.NavigateAndReset(new RecoverPasswordViewModel(dr));
        }
        
        private void Create()
        {
            NavigationState.NavigateAndReset(new CreateProfileViewModel(dr));
        }
    }
}