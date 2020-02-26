using Xamarin.Essentials;

namespace Example.Services.AppInformationService
{
    public class AppInformationService : IAppInformationService
    {
        public string Version => AppInfo.VersionString;

        public string VersionBuild => AppInfo.BuildString;
    }
}