using System.IO;
using Example.Services.SettingsService;
using Splat;

namespace Example.ViewModels.Pages
{
    public class WebViewModel : PageViewModel
    {
        public WebViewModel(string url, IReadonlyDependencyResolver dr) : base(dr)
        {
            var settingsService = dr.GetService<ISettingsService>();
            Url = Path.Combine(settingsService.BaseUrl, url);
        }
        
        public string Url { get; }
    }
}