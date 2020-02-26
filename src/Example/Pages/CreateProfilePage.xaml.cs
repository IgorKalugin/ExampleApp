using System.Reactive.Disposables;
using Example.Navigation;
using Example.Utils;
using Example.Utils.MemoryLeaks;
using ReactiveUI;

namespace Example.Pages
{
    public partial class CreateProfilePage
    {
        public CreateProfilePage()
        {
            InitializeComponent();
            
            this.WhenViewModelActivated(d =>
            {
                MemoryLeaksHunter.TrackActivation(this, d);

                this.Bind(ViewModel, vm => vm.Name, view => view.nameEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Email, view => view.emailEntry.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Password, view => view.passwordEntry.Text).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CreateCommand, view => view.createBtn)
                    .ControlButtonEnabledWhenDisposed(ViewModel.CreateCommand, createBtn)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoginCommand, view => view.loginTgr).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.UserTermsAgreed, view => view.userTermsCheckBox.IsChecked).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ShowUserTermsCommand, view => view.showUserTermsTgr).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.PrivacyPolicyAgreed, view => view.privacyPolicyCheckBox.IsChecked).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ShowPrivacyPolicyCommand, view => view.showPrivacyPolicyTgr).DisposeWith(d);
                
                ViewModel.CreateCommand.IsExecuting.BindTo(spinner, s => s.IsSpinning).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Error, view => view.errorLabel.Text).DisposeWith(d);
            });
        }
    }
}