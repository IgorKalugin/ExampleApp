using System.Reactive.Disposables;
using Example.Navigation;
using Example.Utils.MemoryLeaks;
using ReactiveUI;

namespace Example.Pages
{
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();

            this.WhenViewModelActivated(d =>
            {
                MemoryLeaksHunter.TrackActivation(this, d);
                
                this.Bind(ViewModel, vm => vm.Email, view => view.emailEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Password, view => view.passwordEntry.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Description, view => view.descriptionLabel.Text).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.LoginCommand, view => view.loginBtn).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RecoverPasswordCommand, view => view.recoverPasswordTgr).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CreateCommand, view => view.createTgr).DisposeWith(d);
                
                ViewModel.LoginCommand.IsExecuting.BindTo(spinner, s => s.IsSpinning).DisposeWith(d);
                
                this.Bind(ViewModel, vm => vm.Error, view => view.errorLabel.Text).DisposeWith(d);
            });
        }
    }
}