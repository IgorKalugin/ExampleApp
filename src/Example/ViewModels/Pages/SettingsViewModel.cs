using System.Reactive;
using System.Threading.Tasks;
using Example.Services.AuthService;
using Example.Services.LoggingService;
using Example.ViewModels.Pages.Profile;
using ReactiveUI;
using Splat;

namespace Example.ViewModels.Pages
{
    public class SettingsViewModel : PageViewModel
    {
        private readonly IReadonlyDependencyResolver dr;
        private readonly IAuthService authService;
        
        public SettingsViewModel(IReadonlyDependencyResolver dr) : base(dr)
        {
            this.dr = dr;
            authService = dr.GetService<IAuthService>();

            Email = authService.CurrentUser.Email;

            LogoutCommand = ReactiveCommand.CreateFromTask(Logout).LogInvocation(nameof(LogoutCommand), Logger, dr);
        }

        public string Email { get; }
        
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        
        private async Task Logout()
        {
            await authService.Logout();
            NavigationState.Navigate(new CreateProfileViewModel(dr));
        }
    }
}