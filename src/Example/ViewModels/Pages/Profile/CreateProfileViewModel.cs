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
    public class CreateProfileViewModel : PageViewModel
    {
        private readonly IReadonlyDependencyResolver dr;

        public CreateProfileViewModel(IReadonlyDependencyResolver dr) : base(dr)
        {
            this.dr = dr;

            CreateCommand = ReactiveCommand.CreateFromTask(Create, CanCreate()).LogInvocation(nameof(CreateCommand), Logger, dr);
            LoginCommand = ReactiveCommand.Create(Login).LogInvocation(nameof(LoginCommand), Logger, dr);
            ShowUserTermsCommand = ReactiveCommand.Create(ShowUserTerms).LogInvocation(nameof(ShowUserTermsCommand), Logger, dr);
            ShowPrivacyPolicyCommand = ReactiveCommand.Create(ShowPrivacyPolicy).LogInvocation(nameof(ShowPrivacyPolicyCommand), Logger, dr);

            var timeScheduler = dr.GetService<IScheduler>() ?? DefaultScheduler.Instance;
            this.LogPropertyChangeThrottling(vm => vm.Name, nameof(Name), scheduler: timeScheduler);
            this.LogPropertyChangeThrottling(vm => vm.Email, nameof(Email), scheduler: timeScheduler);
            this.LogPropertyChangeThrottling(vm => vm.Password, nameof(Password), password => "***", timeScheduler);
            this.LogPropertyChange(vm => vm.PrivacyPolicyAgreed, nameof(PrivacyPolicyAgreed));
            this.LogPropertyChange(vm => vm.UserTermsAgreed, nameof(UserTermsAgreed));
        }

        [Reactive] public string Name { get; set; }
        [Reactive] public string Email { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public bool UserTermsAgreed { get; set; }
        [Reactive] public bool PrivacyPolicyAgreed { get; set; }
        
        // we set error text to " " and not to string.Empty or null, otherwise its Height is 0 and when it appears everything goes up a little bit
        [Reactive] public string Error { get; private set; } = " ";

        public ReactiveCommand<Unit, Unit> CreateCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowUserTermsCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowPrivacyPolicyCommand { get; }
        
        private async Task Create()
        {
            Error = " ";

            var authService = dr.GetService<IAuthService>();
            var registered = await authService.Register(Name, Email, Password);
            if (!registered)
            {
                Logger.Value.Error("Registration has failed");
                Error = @"Something went wrong. Please try again.";
                return;
            }

            var authenticated = await authService.Login(Email, Password);
            if (!authenticated)
            {
                Logger.Value.Error("Login has failed. Redirecting to login page");
                // ki.
                NavigationState.NavigateAndReset(new RecoverPasswordViewModel(dr));
                return;
            }

            NavigationState.NavigateAndReset(new WebViewModel("test_page.htm", dr));
        }

        private IObservable<bool> CanCreate()
        {
            return this.WhenAnyValue(vm => vm.Name, vm => vm.Email, vm => vm.Password, vm => vm.UserTermsAgreed, vm => vm.PrivacyPolicyAgreed,
                (name, email, password, userTermsAgreed, privacyPolicyAgreed) =>
                    !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && userTermsAgreed &&
                    privacyPolicyAgreed);
        }


        private void Login()
        {
            // ki.
            // NavigationState.NavigateAndReset(new RecoverPasswordViewModel(dr));
            try
            {
                NavigationState.Navigate(new LoginViewModel(dr));
            }
            catch (UnhandledErrorException e)
            {

            }
        }

        private void ShowUserTerms()
        {
            NavigationState.Navigate(new WebViewModel("test_page.htm", dr));
        }
        
        private void ShowPrivacyPolicy()
        {
            NavigationState.Navigate(new WebViewModel("test_page.htm", dr));
        }
    }
}