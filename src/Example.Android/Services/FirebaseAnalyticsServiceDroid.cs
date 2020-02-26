using Android.OS;
using Example.Droid.Services;
using Example.Services.AnalyticsInfrastructure;
using Firebase.Analytics;
using Plugin.CurrentActivity;
using System.Collections.Generic;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAnalyticsServiceDroid))]

namespace Example.Droid.Services
{
    public class FirebaseAnalyticsServiceDroid : BaseAnalyticsServer
    {
        public override void LogEvent(string eventId, IDictionary<string, string> parameters)
        {

            //utility method to fix eventId, you can skip it if you are sure to always pass valid eventIds
            eventId = FixEventId(eventId);

            var fireBaseAnalytics = FirebaseAnalytics.GetInstance(CrossCurrentActivity.Current.AppContext);

            if (parameters == null)
            {
                fireBaseAnalytics.LogEvent(eventId, null);
                return;
            }

            var bundle = new Bundle();

            foreach (var item in parameters)
            {
                bundle.PutString(item.Key, item.Value);
            }

            fireBaseAnalytics.LogEvent(eventId, bundle);
        }
    }
}