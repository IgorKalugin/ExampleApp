using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Example.Navigation;
using Example.Utils.MemoryLeaks;
using ReactiveUI;
using Xamarin.Forms;

namespace Example.Pages
{
    public partial class WebPage
    {
        public WebPage()
        {
            InitializeComponent();

            this.WhenViewModelActivated(d =>
            {
                MemoryLeaksHunter.TrackActivation(this, d);

                this.BindCommand(ViewModel, vm => vm.NavigateBack, view => view.backBtn).DisposeWith(d);
                
                webView.Source = new UrlWebViewSource { Url = ViewModel.Url };

                webView.Events().Navigating.Skip(1).Subscribe(args => args.Cancel = true).DisposeWith(d);
            });
        }
    }
}