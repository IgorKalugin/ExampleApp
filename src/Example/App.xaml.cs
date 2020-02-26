using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.Collections;
using Example.Services.LoggingService;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Splat;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

[assembly: Preserve (AllMembers = true)]
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Example
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            // We use instance of bootstrapper because clicking on android notification creates new Activity and therefore new App
            // It leads to error while working with LiteDB
            var bootstrapper = AppBootstrapper.Instance;
            if (bootstrapper.NavigationState.NavigationStack.IsEmpty())
            {
                bootstrapper.InitRouter();
            }

            MainPage = AppBootstrapper.CreateMainPage();

            var logger = this.GetLogger(Locator.Current);
            var loggingService = Locator.Current.GetService<ILoggingService>();

            AppDomain.CurrentDomain.UnhandledException += async (sender, args) =>
            {
                if (args.ExceptionObject is Exception e)
                {
                    Crashes.TrackError(e, new Dictionary<string, string> { { "UnhandledException", "true" } });
                }
                else
                {
                    Crashes.TrackError(new Exception("Unknown exception causes application crash"),
                        new Dictionary<string, string> { { "UnhandledException", "true" } });
                }

                logger.Value.Error($"Unhandled exception: {args.ExceptionObject}");
                await loggingService.FlushAsync();
            };
            TaskScheduler.UnobservedTaskException += async (sender, args) =>
            {
                Crashes.TrackError(args.Exception, new Dictionary<string, string> { { "UnhandledException", "true" } });
                logger.Value.Error($"Unhandled task exception: {args.Exception}");
                await loggingService.FlushAsync();
            };
        }

        protected override void OnStart()
        {

            /*AppCenter.Start("android=key deleted;" +
                            "uwp={Your UWP App secret here};" +
                            "ios=key deleted;",
                typeof(Analytics), typeof(Crashes));*/

            var logger = this.GetLogger(Locator.Current);

            Crashes.SendingErrorReport += (sender, e) => logger.Value.Warn($"Preparing to send crash report: {e}");
            Crashes.SentErrorReport += (sender, e) => logger.Value.Warn($"Crash report sent to app center: {e}");
            Crashes.FailedToSendErrorReport += (sender, e) => logger.Value.Error($"Error occured while sending crash report with exception: {e}");
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
