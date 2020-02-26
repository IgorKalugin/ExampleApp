using Example.Droid.Services;
using Example.Services.SettingsService;
using Splat;
using Xamarin.Forms;

[assembly: Dependency(typeof(SettingsService))]
namespace Example.Droid.Services
{
    public class SettingsService : BaseSettingsService
    {
        public override int NotificationsCountLimit => 50;

        public override string BaseUrl => "file:///android_asset/";

        public SettingsService() : base(Locator.Current)
        {
        }
    }
}