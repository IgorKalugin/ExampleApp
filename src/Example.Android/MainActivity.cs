using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Example.Services.LoggingService;
using Plugin.Permissions;
using Splat;
using Xamarin.Forms.Platform.Android;

namespace Example.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            base.OnCreate(savedInstanceState);

            AndroidEnvironment.UnhandledExceptionRaiser += async (sender, args) =>
            {
                var loggingService = Locator.Current.GetService<ILoggingService>();
                var logger = loggingService.GetLogger<MainActivity>();
                logger.Error($"Android unhandled exception: {args.Exception}");
                await loggingService.FlushAsync();
            };

            // Crashlytics (https://github.com/xamarin/XamarinComponents/tree/master/Android/Crashlytics):
            // 1. Besides Xamarin.Android.Crashlytics package I had to add the following packages to make crashlytics work
            // (like in this fix: https://github.com/xamarin/XamarinComponents/issues/460#issuecomment-457541221): 
            // - Xamarin.Android.Fabric
            // - Xamarin.Android.Crashlytics.Answers
            // - Xamarin.Android.Crashlytics.Beta
            // - Xamarin.Android.Crashlytics.Core
            // 2. ReactiveUI swallows command exceptions https://github.com/reactiveui/ReactiveUI/issues/1859
            #if !DEBUG
            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();
            #endif
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}