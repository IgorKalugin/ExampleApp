using System;
using System.IO;
using Example.Data;
using Example.Navigation;
using Example.Services.AnalyticsInfrastructure;
using Example.Services.AppInformationService;
using Example.Services.AuthService;
using Example.Services.CrashlyticsService;
using Example.Services.EmailService;
using Example.Services.FileSystemService;
using Example.Services.LoggingService;
using Example.Services.SettingsService;
using Example.Services.TimeService;
using Example.Services.ZipService;
using Example.Utils.MemoryLeaks;
using Example.ViewModels.Pages;
using Example.ViewModels.Pages.Profile;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace Example
{
    public class AppBootstrapper : ReactiveObject
    {
        private const string MedicineDbFileName = "medicine.db";
        
        public static AppBootstrapper Instance { get; } = new AppBootstrapper();
        
        private AppBootstrapper()
        {
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();

            MemoryLeaksHunter.EnableLog = false;
            
            RegisterViews();
            RegisterDatabase();
            RegisterServices();
        }

        public NavigationState NavigationState { get; private set; }

        public void InitRouter()
        {
            var authService = Locator.Current.GetService<IAuthService>();
            if (authService.CurrentUser == null)
            {
                NavigationState.NavigateAndReset(new CreateProfileViewModel(Locator.Current));
            }
            else
            {
                NavigationState.NavigateAndReset(new LoginViewModel(Locator.Current));
            }
        }

        public static Page CreateMainPage()
        {
            return new NavigationViewHost(Locator.Current);
        }

        private void RegisterViews()
        {
            NavigationState = new NavigationState();
            Locator.CurrentMutable.RegisterConstant<INavigationState>(NavigationState);
            
            Locator.CurrentMutable.RegisterLazySingleton(() => new ExampleViewLocator(), typeof(IViewLocator));
        }
        
        private static void RegisterDatabase()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), MedicineDbFileName);
            var medicineDatabase = new Database(path, Locator.Current);
            Locator.CurrentMutable.RegisterConstant<IDatabase>(medicineDatabase);
        }
        
        private static void RegisterServices()
        {
            Locator.CurrentMutable.RegisterLazySingleton<ITimeService>(() => new TimeService(Locator.Current));
            Locator.CurrentMutable.RegisterLazySingleton(() => DependencyService.Get<ISettingsService>());
            Locator.CurrentMutable.RegisterLazySingleton<ILoggingService>(() => new LoggingService(Locator.Current));
            Locator.CurrentMutable.RegisterLazySingleton(() => DependencyService.Get<ICrashlyticsService>());
            Locator.CurrentMutable.RegisterLazySingleton<IAuthService>(() => new AuthService(Locator.Current));
            Locator.CurrentMutable.RegisterLazySingleton<IEmailService>(() => new EmailService(Locator.Current));
            Locator.CurrentMutable.RegisterLazySingleton<IAppInformationService>(() => new AppInformationService());
            Locator.CurrentMutable.RegisterLazySingleton<IZipService>(() => new ZipService());
            Locator.CurrentMutable.RegisterLazySingleton<IFileSystemService>(() => new FileSystemService());
            

            // Register NotificationsService
            // we've separated NotificationsService logic into different classes and interfaces to test them independently in unit tests

            // The first one is for interface implementation in application core (app center instance)
            Locator.CurrentMutable.RegisterLazySingleton<IAnalyticsServer>(() => new AppCenterAnalyticsServer());
            // The second for implementation on the platform side
            Locator.CurrentMutable.RegisterLazySingleton(() => DependencyService.Get<IAnalyticsServer>());
        }
    }
}