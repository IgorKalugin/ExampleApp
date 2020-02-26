using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Services.AnalyticsInfrastructure
{
    public interface IAnalyticsServer
    {
        void LogEvent(string eventId);
        void LogEvent(string eventId, string paramName, string value);
        void LogEvent(string eventId, IDictionary<string, string> parameters);
    }
}