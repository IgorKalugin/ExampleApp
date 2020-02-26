using Example.Droid.Services;
using Example.Services.CrashlyticsService;
using Xamarin.Forms;

[assembly: Dependency(typeof(CrashlyticsService))]
namespace Example.Droid.Services
{
    public class CrashlyticsService : ICrashlyticsService
    {
        public void Log(string message)
        {
            Crashlytics.Crashlytics.Log(message);
        }
    }
}