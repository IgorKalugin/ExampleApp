using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

namespace Example.Services.AnalyticsInfrastructure
{
    public class AppCenterAnalyticsServer : BaseAnalyticsServer
    {

        public override void LogEvent(string eventId, IDictionary<string, string> parameters)
        {
            eventId = FixEventId(eventId);
            Analytics.TrackEvent(eventId, parameters);
        }
    }
}