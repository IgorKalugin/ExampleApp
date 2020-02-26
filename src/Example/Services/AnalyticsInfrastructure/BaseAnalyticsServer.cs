using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Example.Services.AnalyticsInfrastructure
{
    public abstract class BaseAnalyticsServer : IAnalyticsServer
    {
        public virtual void LogEvent(string eventId)
        {
            LogEvent(eventId, null);
        }

        public virtual void LogEvent(string eventId, string paramName, string value)
        {
            LogEvent(eventId, new Dictionary<string, string>
            {
                {paramName, value}
            });
        }

        public abstract void LogEvent(string eventId, IDictionary<string, string> parameters);

        protected string FixEventId(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return "unknown";

            //remove unwanted characters
            eventId = Regex.Replace(eventId, @"[^a-zA-Z0-9_]+", "_", RegexOptions.Compiled);

            //trim to 40 if needed
            return eventId.Substring(0, Math.Min(256, eventId.Length));
        }
    }
}
